using System.Collections.Generic;
using System.Text;
using PenguinGame;
using System.Linq;

namespace MyBot
{
    /*
    deals with moves decisons
    */
    public static class Brain
    {

        /// <summary>
        /// execute brain
        /// </summary>
        /// <param name="game"></param>
        public static void execute(Game game)
        {
            foreach(var c in game.GetMyIcebergs()){
                System.Console.WriteLine("*********************************");
                Brain.BackupAtArrival(game,game.GetEnemyIcebergs()[0],c.GetTurnsTillArrival(game.GetEnemyIcebergs()[0]));
                System.Console.WriteLine("*********************************");
            }
            game.Debug($"turn number {game.Turn}\n");
            if (game.Turn == 1)
            {
                game.GetMyIcebergs()[0].Upgrade();
            }
            else if (game.Turn == 7)
            {
                game.GetMyIcebergs()[0].SendPenguins(game.GetNeutralIcebergs()[0], 11);
            }
            else if (game.Turn == 12)
            {
                game.GetMyIcebergs()[0].SendPenguins(game.GetNeutralIcebergs()[1], 11);
            }

            else if (game.Turn == 22)
            {
                var c = new Iceberg();
                foreach (var nc in game.GetNeutralIcebergs())
                {
                    c = nc;
                }
                game.GetMyIcebergs()[1].SendPenguins(c, 5);
            }
            else if (game.Turn > 22)
            {
                if (Brain.DeltaPenguinGeneration(game) < 0)
                {
                    foreach (var ice in game.GetMyIcebergs().OrderByDescending(x => x.Level)) //TODO: upgrade icbergs depending on flow potencial
                    {
                        if (!ice.AlreadyActed && Defensive.HelpIcebergData(game, ice, true).Count() == 0 && ice.CanUpgrade())
                        {
                            ice.Upgrade();
                            break;
                        }
                    }
                }
                var defended = Defensive.DefendeIcebergs(game);
                var bestMove = Offensive.BestCombination(game);
                if (bestMove.Item3 != -999 && !bestMove.Item1.AlreadyActed)
                {
                    System.Console.WriteLine($"best move is {bestMove.Item1} to {bestMove.Item2} amount of {bestMove.Item3}");

                    if (bestMove.Item1.CanSendPenguins(bestMove.Item2, System.Math.Abs(Offensive.EnemyPenguinsAtArrival(game, bestMove.Item1, bestMove.Item2)) + 1)) //! fix  this?
                    {
                        System.Console.WriteLine($"enem at arrival {Offensive.EnemyPenguinsAtArrival(game, bestMove.Item1, bestMove.Item2)}");
                        bestMove.Item1.SendPenguins(bestMove.Item2, System.Math.Abs(Offensive.EnemyPenguinsAtArrival(game, bestMove.Item1, bestMove.Item2)) + 1);
                    }
                    else
                    {
                        System.Console.WriteLine($"best move failed1");
                    }
                }
                else
                {
                    System.Console.WriteLine($"best move failed2");
                }

                var middleIceberg = Offensive.MiddleIceberg(game);
                System.Console.WriteLine($"MiddleIce is {middleIceberg}");

                foreach (var ice in game.GetMyIcebergs())
                {
                    if (!ice.Equals(middleIceberg) && !ice.AlreadyActed && Defensive.GetAttackingGroups(game, ice, enemy: true).Count() == 0)
                    {
                        if (ice.CanSendPenguins(middleIceberg, middleIceberg.PenguinsPerTurn))
                            ice.SendPenguins(middleIceberg, middleIceberg.PenguinsPerTurn);
                    }
                }

            }
        }

        public static int DeltaPenguinGeneration(Game game)
        {
            int enemyRateSum = game.GetEnemyIcebergs().Sum(iceberg => iceberg.PenguinsPerTurn);
            int myRateSum = game.GetMyIcebergs().Sum(iceberg => iceberg.PenguinsPerTurn);
            return myRateSum - enemyRateSum;
        }
        public static int BackupAtArrival(Game game, Iceberg destination, int turnsTillArrival)
        {
            var enemyPgToTarget = Defensive.GetAttackingGroups(game, destination); //enemy groups
            var myPgToTarget = Defensive.GetAttackingGroups(game, destination, false); //my groups
            var combinedGroups = new List<(int, int)>(); //item1 amount, item2 distance
            var result = new List<(int, int)>();
            foreach (var pg in enemyPgToTarget)
            {
                if (pg.TurnsTillArrival <= turnsTillArrival)
                {
                    combinedGroups.Add((-pg.PenguinAmount, pg.TurnsTillArrival));
                }
            }
            foreach (var pg in myPgToTarget)
            {
                if (pg.TurnsTillArrival <= turnsTillArrival)
                {
                    combinedGroups.Add((pg.PenguinAmount, pg.TurnsTillArrival));
                }
            }
            combinedGroups.OrderBy(pg => pg.Item2); //order by distance

            int sumDisGroups = 0;
            bool enemy = destination.Owner.Id == game.GetEnemy().Id;
            bool neutral = destination.Owner.Id == -1;
            int destinationAmount = destination.PenguinAmount;
            int penguinsPerTurn = destination.PenguinsPerTurn;
            if (enemy)
            {
                destinationAmount *= -1;
                penguinsPerTurn *= -1;
            }
            if (neutral)
            {
                penguinsPerTurn *= 0;
            }
            while (combinedGroups.Count() > 0)
            {
                int closest = combinedGroups.First().Item2;
                sumDisGroups += closest;
                for (int i = 0; i < combinedGroups.Count(); i++)
                {
                    combinedGroups[i] = (combinedGroups[i].Item1, combinedGroups[i].Item2 - closest);
                }
                var arrived = (from pg in combinedGroups where pg.Item2 == 0 select pg.Item1).ToArray();
                for (int i = arrived.Count(); i > 0; i--, combinedGroups.RemoveAt(0)) ;
                destinationAmount += penguinsPerTurn * closest;
                if (neutral)
                {
                    //fuck
                }
                else
                {
                    destinationAmount += penguinsPerTurn * closest + arrived.Sum();
                    if (destinationAmount < 0)
                    {
                        result.Add((System.Math.Abs(destinationAmount) + 1, sumDisGroups));
                        penguinsPerTurn = destination.PenguinsPerTurn * -1;
                    }
                    else if (destinationAmount > 0)
                    {
                        penguinsPerTurn = destination.PenguinsPerTurn;
                    }
                    else
                    {
                        System.Console.WriteLine("neutral");
                    }
                }

            }
            System.Console.WriteLine($"ice {destination} amount {destinationAmount} turn {turnsTillArrival}");
            return destinationAmount;
        }


        public static bool SafeToUpgrade(Game game, Iceberg icebergToUpgrade)
        {
            return false;
        }

    }
}