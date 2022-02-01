using System.Collections.Generic;
using System.Text;
using PenguinGame;
using System.Linq;


namespace MyBot
{
    public static class Offensive
    {
        public static int EnemyPenguinsAtArrival(Game game, Iceberg myIceberg, Iceberg enemyIceberg) //add my penguins that are on da way
        {
            int amount = enemyIceberg.PenguinAmount + enemyIceberg.PenguinsPerTurn * myIceberg.GetTurnsTillArrival(enemyIceberg);
            return amount;
        }

        public static Iceberg MiddleIceberg(Game game)
        {
            var distances = new List<(Iceberg, int)>();
            foreach (var myIceberg in game.GetMyIcebergs())
            {
                distances.Add((myIceberg, System.Math.Abs(Defensive.AverageDistanceFromEnemyIcebergs(game, myIceberg) - Defensive.AverageDistanceFromMyIcebergs(game, myIceberg))));
                System.Console.WriteLine($"ice {distances.Last().Item1} avg {distances.Last().Item2}");
            }
            return distances.OrderBy(x=>x.Item2).First().Item1;
        }

        public static (Iceberg, Iceberg, int) BestCombination(Game game)
        {
            var maxes = new List<(Iceberg, Iceberg, int)>();
            foreach (var myIceberg in game.GetMyIcebergs())
            {
                if (myIceberg.PenguinAmount > 30)
                {
                    var attackingGroups = Defensive.GetAttackingGroups(game, myIceberg, enemy: true, sorted: false);
                    if (attackingGroups.Count() == 0)
                    {
                        var enemiesList = (from enemyIceberg in game.GetEnemyIcebergs()
                                           where (myIceberg.PenguinAmount - Offensive.EnemyPenguinsAtArrival(game, myIceberg, enemyIceberg) > 0)
                                           select enemyIceberg).ToList();

                        if (enemiesList.Count() > 0)
                        {
                            var mx = enemiesList.OrderByDescending(x => (myIceberg.GetTurnsTillArrival(x) / (myIceberg.PenguinAmount - Offensive.EnemyPenguinsAtArrival(game, myIceberg, x))
                            )).First();
                            if (mx != null)
                            {
                                var c = (myIceberg, mx, myIceberg.GetTurnsTillArrival(mx) / (myIceberg.PenguinAmount - Offensive.EnemyPenguinsAtArrival(game, myIceberg, mx)));
                                maxes.Add(c);
                            }
                        }
                    }
                }
            }
            if (maxes.Count() == 0)
            {
                return (new Iceberg(), new Iceberg(), -999);
            }
            return maxes.OrderByDescending(x => x.Item3).First(); //TODO: error or something maybe?
        }
    }
}