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
            var closest = enemyIcebergs.OrderBy(x => Utils.AverageDistanceFromMyIcbergs(game, x)).First();
            System.Console.WriteLine($"selected ice to attack is {closest}");
            foreach (var myIce in game.GetMyIcebergs())
            {
                int turnsTillArrival = myIce.GetTurnsTillArrival(closest);
                int enemyAmountAtArrival = Utils.EnemyPenguinAmountAtArrival(game, closest, turnsTillArrival)
                + Utils.WorstCaseEnemyReinforcment(game, closest, myIce.GetTurnsTillArrival(closest)) + 1 -
                    turnsTillArrival * closest.PenguinsPerTurn; //! NOTE THIS SUBTITUTION
                int deltaPenguins = myIce.PenguinAmount - Utils.EnemyPenguinAmountAtArrival(game, closest, turnsTillArrival);
                System.Console.WriteLine($"delta {deltaPenguins} enemyarrival {enemyAmountAtArrival} turns is {turnsTillArrival}");
                if (deltaPenguins > 1 && Utils.HelpIcebergData(game, myIce, enemyAmountAtArrival).Count() == 0)
                {
                    System.Console.WriteLine($"iceberg {myIce} amount {myIce.PenguinAmount} acted {myIce.AlreadyActed} send {enemyAmountAtArrival}");
                    if (myIce.CanSendPenguins(closest, enemyAmountAtArrival))
                    {
                        System.Console.WriteLine("sentttt");
                        myIce.SendPenguins(closest, enemyAmountAtArrival);
                    }

                }
            }
        }

        public static void MultiThreadedAttack(Game game)
        {
            var icesToAttack = game.GetEnemyIcebergs().OrderBy(x => Utils.AverageDistanceFromMyIcbergs(game, x)).ToList();
            game.GetNeutralIcebergs().ToList().ForEach(x => icesToAttack.Add(x));

            foreach(var iceToAttack in icesToAttack)
            {
                var walls = Defensive.GetWall(game).ToList();
                var orderedWall = walls.OrderByDescending(x => x.GetTurnsTillArrival(iceToAttack)).ToList();
                int worstTurnsUntilArrival = orderedWall[0].GetTurnsTillArrival(iceToAttack);
                int amountOfEnemies = Utils.WorstCaseEnemyReinforcment(game, iceToAttack, worstTurnsUntilArrival) + Utils.EnemyPenguinAmountAtArrival(game,
                    iceToAttack, worstTurnsUntilArrival) ;
                amountOfEnemies -= worstTurnsUntilArrival*iceToAttack.PenguinsPerTurn;
                amountOfEnemies /= 2;
                amountOfEnemies += 1;
                if(walls[0].CanSendPenguins(iceToAttack, amountOfEnemies) && walls[1].CanSendPenguins(iceToAttack, amountOfEnemies))
                {

                    walls[0].SendPenguins(iceToAttack, amountOfEnemies);
                    walls[1].SendPenguins(iceToAttack, amountOfEnemies);
                    break;
                }
            }

        }
        public static void DoMultiThreadedAttack(Game game, Iceberg target, List<(int, int)> attackData)
        {
            //TODO: improve priority

            var iceToAttack = target;
            var defendeData = attackData;

            foreach (var data in defendeData)
            {
                int neededAmount = data.Item1;
                int timeToDeliver = data.Item2;
                var possibleDefenders = new List<Iceberg>();
                foreach (var myIceberg in Defensive.GetWall(game))
                {
                    if (myIceberg.GetTurnsTillArrival(iceToAttack) <= timeToDeliver)
                    {
                        //bruh
                        int sumEnemyGroups = game.GetEnemyPenguinGroups().Sum(pg => (pg.Destination.Equals(target) ? pg.PenguinAmount : 0));

                        if (myIceberg.PenguinAmount - sumEnemyGroups > 20)
                        {
                            possibleDefenders.Add(myIceberg);
                        }
                    }
                }
                if (possibleDefenders.Count() > 0)
                {
                    int sumDefenders = possibleDefenders.Sum(defender => defender.PenguinAmount);
                    if (sumDefenders >= neededAmount)
                    {
                        foreach (var ice in possibleDefenders)
                        {
                            double ratio = (double)ice.PenguinAmount / sumDefenders;
                            int amountToSend = (int)(ratio * neededAmount) + 1;
                            if (ice.PenguinAmount < amountToSend)
                            {
                                --amountToSend;
                            }
                            ice.SendPenguins(iceToAttack, amountToSend);
                        }
                    }
                }
            }
        }
    }
}