using System.Collections.Generic;
using System.Text;
using PenguinGame;
using System.Linq;


namespace MyBot
{
    public static class Offensive
    {


        //TODO: take into account penguins that are on the way and if its likely that the iceberg will upgrade
        public static int EnemyPenguinsAtArrival(Game game, Iceberg myIceberg, Iceberg enemyIceberg) //add my penguins that are on da way
        {
            int turnsTillArrival = myIceberg.GetTurnsTillArrival(enemyIceberg);
            int amount = enemyIceberg.PenguinAmount + enemyIceberg.PenguinsPerTurn * turnsTillArrival;
            var enemyIcebergs = game.GetEnemyIcebergs();
            foreach(var closeIceberg in enemyIcebergs)
            {
                if(closeIceberg.GetTurnsTillArrival(myIceberg) <= enemyIceberg.GetTurnsTillArrival(myIceberg))
                {
                    amount += closeIceberg.PenguinAmount + closeIceberg.PenguinsPerTurn*closeIceberg.GetTurnsTillArrival(enemyIceberg);
                }
            }

            /*foreach(var enemyPg in Defensive.GetAttackingGroups(game,enemyIceberg,enemy:true,sorted: false)) //! wont work on rufulf need to check why
            {
                if(enemyPg.TurnsTillArrival <= turnsTillArrival)
                {
                    amount += enemyPg.PenguinAmount;
                }
            }
            foreach(var myPg in Defensive.GetAttackingGroups(game,enemyIceberg,enemy: false,sorted: false))
            {
                if(myPg.TurnsTillArrival <= turnsTillArrival){
                    amount -= myPg.PenguinAmount;
                }
            }*/
            System.Console.WriteLine($"at iceberg {enemyIceberg} at turn {turnsTillArrival} will be {amount}");
            return amount;
        }

        public static Iceberg MiddleIceberg(Game game) //TODO: make it smarter
        {
            var distances = new List<(Iceberg, double)>();
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
                if (myIceberg.PenguinAmount > 30) //NOTE: why 30? magic number or what?
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