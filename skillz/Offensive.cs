using PenguinGame;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MyBot
{
    public static class Offensive
    {
        /// <summary>
        /// check if each individual iceberg can attack if so, fuck so
        /// </summary>
        /// <param name="game"></param>
        /// !need to fix IMPORTANT: track icebergs that already have been attacked!, fixed?
        public static void Attack(Game game)
        {
            var enemyIcebergs = game.GetEnemyIcebergs().ToList();
            game.GetNeutralIcebergs().ToList().ForEach(x => enemyIcebergs.Add(x));
            var orderedTargets = enemyIcebergs.OrderBy(x => Utils.AverageDistanceFromWall(game, x));
            foreach (var closest in orderedTargets)
            {
                if(GameInfo.IsAttackedByUs(closest)) //!note
                {
                    System.Console.WriteLine("already attacked by us");
                    continue;
                }
                for(int i= 2; i < 25;i++){
                    int turnsTillArrival = i;
                    int enemyAmountAtArrival = Utils.EnemyPenguinAmountAtArrival(game, closest, turnsTillArrival)
                    + Utils.WorstCaseEnemyReinforcment(game, closest, turnsTillArrival) + 1 -
                        turnsTillArrival * closest.PenguinsPerTurn;
                    var dataToSend = new List<(int,int)>();
                    dataToSend.Add((enemyAmountAtArrival,turnsTillArrival));
                    if(Utils.SendAmountWithTurnsLimit(game,closest,dataToSend))
                    {
                        GameInfo.UpdateAttackedIcebergsByUs(closest,true);
                        break;
                    }
                }
                /*foreach (var myIce in game.GetMyIcebergs()) 
                {
                    int turnsTillArrival = myIce.GetTurnsTillArrival(closest);
                    int enemyAmountAtArrival = Utils.EnemyPenguinAmountAtArrival(game, closest, turnsTillArrival)
                    + Utils.WorstCaseEnemyReinforcment(game, closest, myIce.GetTurnsTillArrival(closest)) + 1 -
                        turnsTillArrival * closest.PenguinsPerTurn;
                    int deltaPenguins = myIce.PenguinAmount - enemyAmountAtArrival;
                    //if(Utils.SendAmountWithTurnsLimit(game,closest,))
                    // System.Console.WriteLine($"delta {deltaPenguins} enemyarrival {enemyAmountAtArrival} turns is {turnsTillArrival}");
                    if (deltaPenguins > 1 && Utils.HelpIcebergData(game, myIce, enemyAmountAtArrival).Count() == 0)
                    {
                        //System.Console.WriteLine($"iceberg {myIce} amount {myIce.PenguinAmount} acted {myIce.AlreadyActed} send {enemyAmountAtArrival}");
                        if (myIce.CanSendPenguins(closest, enemyAmountAtArrival) && !GameInfo.UpgradedThisTurn(myIce.UniqueId))
                        {
                            //System.Console.WriteLine($"selected ice to attack is {closest}");
                            myIce.SendPenguins(closest, enemyAmountAtArrival);
                            GameInfo.UpdateEnemyAttackedIcebergs(closest,true);
                        }
                    }
                }*/
            }
        }


        /// <summary>
        /// check if can attack one icebergs with the walls
        /// </summary>
        /// <param name="game"></param>
        public static void MultiThreadedAttack(Game game) //! NEED TO FIX AGAINST RUDULF MURDER OUR ICEBEGS!
        {
            if (Defensive.GetWall(game).Length < 2) //!infinite loop?
            {
                Offensive.Attack(game);
            }
            var icesToAttack = game.GetEnemyIcebergs().ToList();
            game.GetNeutralIcebergs().ToList().ForEach(x => icesToAttack.Add(x));
            icesToAttack = icesToAttack.OrderBy(x => Utils.AverageDistanceFromWall(game, x)).ToList();

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
                //System.Console.WriteLine($"ice {iceToAttack} turn {worstTurnsUntilArrival} amount {amountOfEnemies}");
                if (amountOfEnemies < 0)
                {
                    System.Console.WriteLine("the f**k negative amount");
                    continue;
                }
                //System.Console.WriteLine($"each need to send {amountOfEnemies}");
                if (Utils.HelpIcebergData(game, walls[0], amountOfEnemies).Count() == 0 &&
                    Utils.HelpIcebergData(game, walls[1], amountOfEnemies).Count() == 0
                    &&
                    walls[0].CanSendPenguins(iceToAttack, amountOfEnemies) && walls[1].CanSendPenguins(iceToAttack, amountOfEnemies)
                    && !GameInfo.UpgradedThisTurn(walls[0].UniqueId) && !GameInfo.UpgradedThisTurn(walls[1].UniqueId))
                {

                    walls[0].SendPenguins(iceToAttack, amountOfEnemies);
                    walls[1].SendPenguins(iceToAttack, amountOfEnemies);
                }
                else
                {
                    System.Console.WriteLine($"cannot send a godman {amountOfEnemies}");
                }
            }

        }

        /// <summary>
        /// check if the enemy send reinforcment to attacked iceberg, return a list if so
        /// </summary>
        /// <param name="game"></param>
        /// <param name="enemyIceberg"></param>
        /// <param name="additon"></param>
        /// <param name="upgrade"></param>
        public static List<(int, int)> GetReinforcmentData(Game game, Iceberg enemyIceberg, int additon, bool upgrade = false)
        {
            var enemyPgToTarget = Utils.GetAttackingGroups(game, enemyIceberg, enemy: true);
            var myPgToTarget = Utils.GetAttackingGroups(game, enemyIceberg, enemy: false);
            var combinedData = new List<(int, int)>();
            var result = new List<(int, int)>();
            int myId = game.GetMyself().Id;
            int penguinPerTurnRate = enemyIceberg.PenguinsPerTurn;
            int enemyIcebergCounter = enemyIceberg.PenguinAmount;
            if (upgrade) { penguinPerTurnRate += enemyIceberg.UpgradeValue; enemyIcebergCounter -= enemyIceberg.UpgradeCost + 1; }
            enemyPgToTarget.ForEach(pg => combinedData.Add((pg.PenguinAmount, pg.TurnsTillArrival)));
            myPgToTarget.ForEach(pg => combinedData.Add((-pg.PenguinAmount, pg.TurnsTillArrival)));
            combinedData.Sort((u1, u2) => u1.Item2.CompareTo(u2.Item2));
            /*foreach (var n in combinedData)
            {
                System.Console.WriteLine($"{n.Item1}-{n.Item1}");
            }*/
            int sumCloseDistance = 0;
            enemyIcebergCounter -= additon;

            while (combinedData.Count() > 0)
            {
                int closestGroupDistance = combinedData.First().Item2;
                sumCloseDistance += closestGroupDistance;
                for (int i = 0; i < combinedData.Count(); i++)
                {
                    combinedData[i] = (combinedData[i].Item1, combinedData[i].Item2 - closestGroupDistance);
                }
                var arrived = (from pg in combinedData where pg.Item2 == 0 select pg.Item1).ToList();
                for (int i = arrived.Count(); i > 0; i--, combinedData.RemoveAt(0)) ;
                enemyIcebergCounter += closestGroupDistance * penguinPerTurnRate + arrived.Sum();
                if (enemyIcebergCounter >= 0)
                {
                    //System.Console.WriteLine($"COUNTER {enemyIcebergCounter}");
                    result.Add((enemyIcebergCounter + 1, sumCloseDistance));
                    //game.Debug($"need to save {iceberg} with {myIcebergCounter - 1}");
                }
            }
            //System.Console.WriteLine($"iceberg enemy {enemyIceberg} amount of {enemyIcebergCounter}");
            return result;
        }

        /// <summary>
        /// send the reinforcment
        /// </summary>
        /// <param name="game"></param>
        /// <param name="icebergToConqureData"></param>
        public static void SendReinforcment(Game game, (Iceberg, List<(int, int)>) icebergToConqureData)
        {
            Utils.SendAmountWithTurnsLimit(game, icebergToConqureData.Item1, icebergToConqureData.Item2);
        }

        /// <summary>
        /// actually send the reinforcment
        /// </summary>
        /// <param name="game"></param>
        public static void test1(Game game)
        {   
            var attackedEnemyIces = game.GetEnemyIcebergs().ToList();
            attackedEnemyIces = (from ice in attackedEnemyIces where Utils.GetAttackingGroups(game,ice,false).Count() > 0 select ice).ToList();
            foreach (var k in attackedEnemyIces)
            {
                var n = Offensive.GetReinforcmentData(game, k, 0);
                if (n.Count() > 0)
                {
                    System.Console.WriteLine($"reinforce {k}");
                    Offensive.SendReinforcment(game, (k, n));
                }
            }
        }

    }
}