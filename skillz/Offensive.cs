using System.Collections.Generic;
using System.Text;
using PenguinGame;
using System.Linq;


namespace MyBot
{
    public static class Offensive
    {


        //TODO: take into account penguins that are on the way and if its likely that the iceberg will upgrade
        public static int EnemyPenguinsAtArrival(Game game, Iceberg myIceberg, Iceberg destIceberg) //add my penguins that are on da way1
        {
            int turnsTillArrival = myIceberg.GetTurnsTillArrival(destIceberg);
            int amount = destIceberg.PenguinAmount;
            if(destIceberg.Owner.Id != game.GetMyself().Id)
            {
                amount *= -1;
            }
            
            var destIcebergs = game.GetEnemyIcebergs();
            foreach (var closeIceberg in destIcebergs)
            {
                if (closeIceberg.GetTurnsTillArrival(destIceberg) <= turnsTillArrival)
                {
                    amount -= closeIceberg.PenguinsPerTurn * closeIceberg.GetTurnsTillArrival(destIceberg);
                    amount -= closeIceberg.PenguinAmount;
                }
            }

            /*foreach(var enemyPg in Defensive.GetAttackingGroups(game,destIceberg,enemy:true,sorted: false)) //! wont work on rufulf need to check why
            {
                if(enemyPg.TurnsTillArrival <= turnsTillArrival)
                {

                    amount -= enemyPg.PenguinAmount;
                }
            }*/
            System.Console.WriteLine($"at iceberg {destIceberg} at turn {turnsTillArrival} will be E {amount}");

            return amount;
        }



            public static int EnemyPenguinsAtArrivalNeutral(Game game, Iceberg myIceberg, Iceberg destIceberg)
            {
                if(destIceberg.Owner.Id != -1)
                {
                    System.Console.WriteLine("ICEBERG IS NOT NEUTRAL");
                    return -999;
                }
                else{
                    int amount = destIceberg.PenguinAmount;
                    int turnsTillArrival = myIceberg.GetTurnsTillArrival(destIceberg);
                    var destIcebergs = game.GetEnemyIcebergs();
                    foreach(var closeIceberg in destIcebergs)
                    {
                        if(closeIceberg.GetTurnsTillArrival(destIceberg) <= turnsTillArrival)
                        {
                            amount -= closeIceberg.PenguinsPerTurn * closeIceberg.GetTurnsTillArrival(destIceberg);
                            amount -= closeIceberg.PenguinAmount;
                        }
                    }
                    foreach(var enemyPg in Defensive.GetAttackingGroups(game,destIceberg,enemy:true,sorted: false)) //! wont work on rufulf need to check why
                    {
                        if(enemyPg.TurnsTillArrival <= turnsTillArrival)
                        {

                            amount -= enemyPg.PenguinAmount;
                        } 
                    }
                    System.Console.WriteLine($"at iceberg {destIceberg} at turn {turnsTillArrival} will N be {amount}");
                    return amount;
                }
            }


        public static int BackupAtArrival(Game game,Iceberg destination, int turnsTillArrival)
        {
            var enemyPgToTarget = Defensive.GetAttackingGroups(game, destination); //enemy groups
            var myPgToTarget = Defensive.GetAttackingGroups(game, destination, false); //my groups
            var combinedGroups = new List<(int,int)>(); //item1 amount, item2 distance
            var result = new List<(int,int)>();
            foreach (var pg in enemyPgToTarget)
            {
                if (pg.TurnsTillArrival <= turnsTillArrival)
                {
                    combinedGroups.Add((-pg.PenguinAmount,pg.TurnsTillArrival));
                }
            }
            foreach (var pg in myPgToTarget)
            {
                if (pg.TurnsTillArrival <= turnsTillArrival)
                {
                    combinedGroups.Add((pg.PenguinAmount,pg.TurnsTillArrival));
                }
            }
            combinedGroups.OrderBy(pg=>pg.Item2); //order by distance
            
            int sumDisGroups = 0;
            bool enemy = destination.Owner.Id == game.GetEnemy().Id;
            bool neutral = destination.Owner.Id == -1;
            int destinationAmount = destination.PenguinAmount;
            int penguinsPerTurn = destination.PenguinsPerTurn;
            if(enemy)
            {
                destinationAmount*=-1;
                penguinsPerTurn*=-1;
            }
            if(neutral)
            {
                penguinsPerTurn *= 0;
            }
            while(combinedGroups.Count() > 0)
            {   
                int closest = combinedGroups.First().Item2;
                sumDisGroups += closest;
                for(int i =0;i < combinedGroups.Count();i++)
                {
                    combinedGroups[i] = (combinedGroups[i].Item1,combinedGroups[i].Item2 - closest);
                }
                var arrived = (from pg in combinedGroups where pg.Item2 == 0 select pg.Item1).ToArray();
                for (int i = arrived.Count(); i > 0; i--, combinedGroups.RemoveAt(0));
                destinationAmount += penguinsPerTurn*closest;
                if(neutral)
                {
                    //fuck
                }
                else
                {
                    destinationAmount += penguinsPerTurn * closest + arrived.Sum();
                    if(destinationAmount < 0)
                    {
                        result.Add((System.Math.Abs(destinationAmount) + 1,sumDisGroups));
                        penguinsPerTurn = destination.PenguinsPerTurn * -1;
                    }
                    else if(destinationAmount > 0){
                        penguinsPerTurn = destination.PenguinsPerTurn;
                    }
                    else{
                        System.Console.WriteLine("neutral");
                    }
                }

            }
            System.Console.WriteLine($"ice {destination} c {destinationAmount}");
            return destinationAmount;
        }

        public static Iceberg MiddleIceberg(Game game) //TODO: make it smarter
        {
            var distances = new List<(Iceberg, double)>();
            foreach (var myIceberg in game.GetMyIcebergs())
            {
                distances.Add((myIceberg, System.Math.Abs(Defensive.AverageDistanceFromEnemyIcebergs(game, myIceberg) - Defensive.AverageDistanceFromMyIcebergs(game, myIceberg))));
                System.Console.WriteLine($"ice {distances.Last().Item1} avg {distances.Last().Item2}");
            }
            return distances.OrderBy(x => x.Item2).First().Item1;
        }

        public static (Iceberg, Iceberg, int) BestCombination(Game game)
        {
            var maxes = new List<(Iceberg, Iceberg, int)>();
            foreach (var myIceberg in game.GetMyIcebergs())
            {
                if (myIceberg.PenguinAmount > 1 && !myIceberg.AlreadyActed) //NOTE: why 30? magic number or what?
                {
                    if (Defensive.HelpIcebergData(game, myIceberg, addition: myIceberg.PenguinAmount - 1).Count() == 0)
                    {
                        var enemiesList = (from enemyIceberg in game.GetEnemyIcebergs()
                                           where (myIceberg.PenguinAmount - System.Math.Abs(Offensive.EnemyPenguinsAtArrival(game, myIceberg, enemyIceberg)) > 0 &&
                                           Defensive.GetAttackingGroups(game, enemyIceberg, enemy: false).Count() == 0)
                                           select enemyIceberg).ToList();

                        foreach (var n in game.GetNeutralIcebergs())
                        {
                            if (myIceberg.PenguinAmount - System.Math.Abs(Offensive.EnemyPenguinsAtArrivalNeutral(game, myIceberg, n)) > 0)
                            {
                                System.Console.WriteLine($"possible neutal at {n}");
                                enemiesList.Add(n);
                            }
                        }
                        if (enemiesList.Count() > 0)
                        {
                            var mx = enemiesList.OrderByDescending(x => Offensive.EnemyPenguinsAtArrival(game, myIceberg, x)
                            ).ThenBy(x => x.GetTurnsTillArrival(myIceberg)).First();
                            if (mx != null)
                            {
                                var c = (myIceberg, mx, System.Math.Abs(Offensive.EnemyPenguinsAtArrival(game, myIceberg, mx)) + 1);
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