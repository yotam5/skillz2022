using PenguinGame;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MyBot
{
    public static class Offensive
    {
        public static void Attack(Game game)
        {
            var enemyIcebergs = game.GetEnemyIcebergs().ToList();
            game.GetNeutralIcebergs().ToList().ForEach(x => enemyIcebergs.Add(x));
            var closest = enemyIcebergs.OrderBy(x => Utils.AverageDistanceFromWall(game, x)).First();
            //System.Console.WriteLine($"selected ice to attack is {closest}");
            foreach (var myIce in game.GetMyIcebergs())
            {
                int turnsTillArrival = myIce.GetTurnsTillArrival(closest);
                int enemyAmountAtArrival = Utils.EnemyPenguinAmountAtArrival(game, closest, turnsTillArrival)
                + Utils.WorstCaseEnemyReinforcment(game, closest, myIce.GetTurnsTillArrival(closest)) + 1 -
                    turnsTillArrival * closest.PenguinsPerTurn; 
                int deltaPenguins = myIce.PenguinAmount - enemyAmountAtArrival;
                                                                                // System.Console.WriteLine($"delta {deltaPenguins} enemyarrival {enemyAmountAtArrival} turns is {turnsTillArrival}");
                if (deltaPenguins > 1 && Utils.HelpIcebergData(game, myIce, enemyAmountAtArrival).Count() == 0)
                {
                    //System.Console.WriteLine($"iceberg {myIce} amount {myIce.PenguinAmount} acted {myIce.AlreadyActed} send {enemyAmountAtArrival}");
                    if (myIce.CanSendPenguins(closest, enemyAmountAtArrival) && !GameInfo.UpgradedThisTurn(myIce.UniqueId))
                    {
                        myIce.SendPenguins(closest, enemyAmountAtArrival);
                    }

                }
            }
        }

        public static void MultiThreadedAttack(Game game)
        {
            var icesToAttack = game.GetEnemyIcebergs().ToList();
            game.GetNeutralIcebergs().ToList().ForEach(x => icesToAttack.Add(x));
            icesToAttack = icesToAttack.OrderBy(x => Utils.AverageDistanceFromWall(game, x)).ToList();
            bool attacked = false;
            foreach (var iceToAttack in icesToAttack)
            {
                var walls = Defensive.GetWall(game).ToList();
                var orderedWall = walls.OrderByDescending(x => x.GetTurnsTillArrival(iceToAttack)).ToList();
                int worstTurnsUntilArrival = orderedWall[0].GetTurnsTillArrival(iceToAttack);
                int amountOfEnemies = Utils.WorstCaseEnemyReinforcment(game, iceToAttack, worstTurnsUntilArrival) + Utils.EnemyPenguinAmountAtArrival(game,
                    iceToAttack, worstTurnsUntilArrival);
                amountOfEnemies -= worstTurnsUntilArrival * iceToAttack.PenguinsPerTurn;
                //System.Console.WriteLine($"to ice {iceToAttack}  worst { Utils.WorstCaseEnemyReinforcment(game, iceToAttack, worstTurnsUntilArrival)} arrival {Utils.EnemyPenguinAmountAtArrival(game,iceToAttack, worstTurnsUntilArrival)}");
                //System.Console.WriteLine($"DELTA {amountOfEnemies}");
                //System.Console.WriteLine($"amount of enemies at {iceToAttack} is {amountOfEnemies}");
                amountOfEnemies /= 2;
                amountOfEnemies += 1;
                if (amountOfEnemies < 0)
                {
                    continue;
                }
                //System.Console.WriteLine($"each need to send {amountOfEnemies}");
                if (walls[0].CanSendPenguins(iceToAttack, amountOfEnemies) && walls[1].CanSendPenguins(iceToAttack, amountOfEnemies)
                    && !GameInfo.UpgradedThisTurn(walls[0].UniqueId) && !GameInfo.UpgradedThisTurn(walls[1].UniqueId))
                {

                    walls[0].SendPenguins(iceToAttack, amountOfEnemies);
                    walls[1].SendPenguins(iceToAttack, amountOfEnemies);
                    attacked = true;
                }
                else
                {
                    //System.Console.WriteLine($"cannot send a godman {amountOfEnemies}");
                }
            }
            if (!attacked)
            {
                Offensive.Attack(game);
            }

        }

        public static List<(int, int)> GetReinforcmentData(Game game, Iceberg iceberg, int additon, bool upgrade = false)
        {
            var enemyPgToTarget = Utils.GetAttackingGroups(game, iceberg);
            var myPgToTarget = Utils.GetAttackingGroups(game, iceberg, false);
            var combinedData = new List<(int, int)>();
            var result = new List<(int, int)>();
            int myId = game.GetMyself().Id;
            int penguinPerTurnRate = iceberg.PenguinsPerTurn;
            int myIcebergCounter = iceberg.PenguinAmount;
            if (upgrade) { penguinPerTurnRate += iceberg.UpgradeValue; myIcebergCounter -= iceberg.UpgradeCost + 1; }
            enemyPgToTarget.ForEach(pg => combinedData.Add((pg.PenguinAmount, pg.TurnsTillArrival)));
            myPgToTarget.ForEach(pg => combinedData.Add((-pg.PenguinAmount, pg.TurnsTillArrival)));
            combinedData.Sort((u1, u2) => u1.Item2.CompareTo(u2.Item2));

            int sumCloseDistance = 0;
            myIcebergCounter -= additon;

            while (combinedData.Count() > 0)
            {
                int closest = combinedData.First().Item2;
                sumCloseDistance += closest;
                for (int i = 0; i < combinedData.Count(); i++)
                {
                    combinedData[i] = (combinedData[i].Item1, combinedData[i].Item2 - closest);
                }
                var arrived = (from pg in combinedData where pg.Item2 == 0 select pg.Item1).ToList();
                for (int i = arrived.Count(); i > 0; i--, combinedData.RemoveAt(0)) ;
                myIcebergCounter += closest * penguinPerTurnRate + arrived.Sum();
                if (myIcebergCounter <= 0)
                {
                    result.Add((-1 * myIcebergCounter + 1, sumCloseDistance));
                    //game.Debug($"need to save {iceberg} with {myIcebergCounter - 1}");
                }
            }
            return result;
        }
        public static void SendReinforcment(Game game,(Iceberg,List<(int,int)>) icebergInDangerData)
        {
   
                var iceToDefend = icebergInDangerData.Item1;
                var defendeData = icebergInDangerData.Item2;

                foreach (var data in defendeData)
                {
                    int neededAmount = data.Item1;
                    int timeToDeliver = data.Item2;
                    var possibleDefenders = new List<Iceberg>();
                    foreach (var myIceberg in Defensive.GetWall(game))
                    {
                        if (!myIceberg.Equals(iceToDefend) && iceToDefend.GetTurnsTillArrival(myIceberg) <= timeToDeliver && !GameInfo.UpgradedThisTurn(
                            myIceberg.UniqueId) && Utils.HelpIcebergData(game, myIceberg, 0).Count() == 0)
                        {
                            possibleDefenders.Add(myIceberg);
                        }
                    }
                    if (possibleDefenders.Count() > 0)
                    {
                        var protectors = new List<(Iceberg, int)>();
                        int sumDefenders = possibleDefenders.Sum(defender => defender.PenguinAmount);
                        if (sumDefenders >= neededAmount)
                        {
                            foreach (var ice in possibleDefenders)
                            {
                                double ratio = (double)ice.PenguinAmount / sumDefenders;
                                int amountToSend = (int)(ratio * neededAmount) + 1;
                                if (ice.PenguinAmount < amountToSend) { --amountToSend; }
                                bool safeToSend = Utils.HelpIcebergData(game, ice, amountToSend).Count() == 0;
                                while (!safeToSend && amountToSend > 0)
                                {
                                    --amountToSend;
                                    safeToSend = Utils.HelpIcebergData(game, ice, amountToSend).Count() == 0;
                                }
                                if (amountToSend > 0 && ice.CanSendPenguins(iceToDefend, amountToSend))
                                {
                                    protectors.Add((ice, amountToSend));
                                }
                                if (protectors.Sum(x => x.Item2) >= neededAmount)
                                {
                                    foreach (var protector in protectors)
                                    {
                                        protector.Item1.SendPenguins(iceToDefend, protector.Item2);
                                    }
                                }

                            }
                        }
                    }
                }
        }

        public static void test1(Game game)
        {
            var attackedNeutralIcebergs = game.GetEnemyIcebergs();
            foreach(var k in attackedNeutralIcebergs)
            {
                var n = Offensive.GetReinforcmentData(game,k,0);
                Offensive.SendReinforcment(game,(k,n));
            }
        }
  
    }
}