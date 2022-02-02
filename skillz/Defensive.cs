using System.Collections.Generic;
using System.Text;
using PenguinGame;
using System.Linq;

namespace MyBot
{
    public static class Defensive
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        /// <param name="iceberg"></param>
        /// <param name="upgrade"></param>
        /// <param name="addition">int, negative value to send</param>
        /// <returns></returns>
        public static List<(int, int)> HelpIcebergData(Game game, Iceberg iceberg, bool upgrade = false, int addition = 0)
        {
            var enemyPgToTarget = Defensive.GetAttackingGroups(game, iceberg);
            var myPgToTarget = Defensive.GetAttackingGroups(game, iceberg, false);
            List<int[]> combinedData = new List<int[]>();
            List<(int, int)> result = new List<(int, int)>();
            Player myPlayer = game.GetMyself();
            int penguinPerTurnRate = iceberg.PenguinsPerTurn;
            int myIcebergCounter = iceberg.PenguinAmount;
            //int icebergLevel = iceberg.Level;

            if (upgrade) { penguinPerTurnRate += iceberg.UpgradeValue; myIcebergCounter -= iceberg.UpgradeCost;}

            enemyPgToTarget.ForEach(pg => combinedData.Add(new int[] { -pg.PenguinAmount, pg.TurnsTillArrival }));
            myPgToTarget.ForEach(pg => combinedData.Add(new int[] { pg.PenguinAmount, pg.TurnsTillArrival }));
            combinedData.Sort((u1, u2) => u1[1].CompareTo(u2[1]));

            int sumCloseDistance = 0;


            while (combinedData.Count() > 0)
            {
                int closestDistance = combinedData[0][1];
                sumCloseDistance += closestDistance;
                foreach (var pg in combinedData)
                {
                    pg[1] -= closestDistance;
                }
                var arrived = (from pg in combinedData where pg[1] == 0 select pg[0]).ToList();
                for (int i = arrived.Count(); i > 0; i--, combinedData.RemoveAt(0)) ;
                myIcebergCounter += penguinPerTurnRate * closestDistance + arrived.Sum();
                if (myIcebergCounter <= 0)
                {
                    result.Add((-1 * myIcebergCounter + 1, sumCloseDistance));
                    game.Debug($"need to save {iceberg} with {myIcebergCounter - 1}");
                }

            }

            return result;
        }


        public static List<(Iceberg, List<(int, int)>)> IcebergsInDangers(Game game)
        {
            var icebergsInDanger = new List<(Iceberg, List<(int, int)>)>();
            foreach (var iceberg in game.GetMyIcebergs())
            {
                var helpData = Defensive.HelpIcebergData(game, iceberg);
                if (helpData.Count() > 0)
                {
                    icebergsInDanger.Add((iceberg, helpData));
                }
            }
            return icebergsInDanger;
        }

        public static List<Iceberg> DefendeIcebergs(Game game)
        {

            var icebergsInDanger = Defensive.IcebergsInDangers(game);
            var succeded = new List<Iceberg>(); //probably?
            if (icebergsInDanger.Count() > 0)
            {
                icebergsInDanger.OrderByDescending(x=>Defensive.IcebergPriority(game, x.Item1)); 

                foreach (var icebergInDangerData in icebergsInDanger)
                {
                    var iceToDefend = icebergInDangerData.Item1;
                    var defendeData = icebergInDangerData.Item2;

                    foreach (var data in defendeData)
                    {
                        int neededAmount = data.Item1;
                        int timeToDeliver = data.Item2;
                        var possibleDefenders = new List<Iceberg>();
                        foreach (var myIceberg in game.GetMyIcebergs())
                        {
                            if (!myIceberg.Equals(iceToDefend) && iceToDefend.GetTurnsTillArrival(myIceberg) <= timeToDeliver)
                            {
                                //bruh
                                int sumEnemyGroups = game.GetEnemyPenguinGroups().Sum(pg => (pg.Destination.Equals(myIceberg) ? pg.PenguinAmount : 0));

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
                                succeded.Add(iceToDefend);
                                foreach (var ice in possibleDefenders)
                                {
                                    double ratio = (double)ice.PenguinAmount / sumDefenders;
                                    int amountToSend = (int)(ratio * neededAmount) + 1;
                                    if (ice.PenguinAmount < amountToSend)
                                    {
                                        amountToSend--;
                                    }
                                    ice.SendPenguins(iceToDefend, amountToSend);
                                }
                            }
                        }
                    }
                }
            }
        return succeded;
        }


        public static void EvacuateIceberg(Game game,Iceberg myIceberg) //! fixme not good bruh
        {
            var myIces = game.GetMyIcebergs().ToList();
            myIces.OrderBy(x=>x.GetTurnsTillArrival(myIceberg));
            myIceberg.SendPenguins(myIces.First(),myIceberg.PenguinAmount);
        }



        public static List<Iceberg> GetMyAttackedIcebergs(Game game) //the fuck
        {
            var myIcebergs = game.GetMyIcebergs();
            var data = new List<Iceberg>();
            foreach (var iceberg in myIcebergs)
            {
                var enemyAttackingGroups = Defensive.GetAttackingGroups(game, iceberg, true, true);
                var myAttackingGroups = Defensive.GetAttackingGroups(game, iceberg, false, true);
            }
            return data;
        }

        public static int AverageDistanceFromMyIcebergs(Game game, Iceberg exlude)
        {
            var myIcebergs = game.GetMyIcebergs();
            if(myIcebergs.Length == 1){
                return 0;
            }
            int totalDistance = 0;
            foreach (var iceberg in myIcebergs)
            {
                if (!iceberg.Equals(exlude))
                {
                    totalDistance += iceberg.GetTurnsTillArrival(exlude);
                }
            }
            //System.Console.WriteLine($"averag my {totalDistance/myIcebergs.Length} for {exlude}");
            return (totalDistance / (myIcebergs.Length - 1));
        }

        public static int AverageDistanceFromEnemyIcebergs(Game game, Iceberg iceberg)
        {
            int totalDistance = 0;
            var enemyIcebergs = game.GetEnemyIcebergs();
            if(enemyIcebergs.Length == 0){return 0;}
            foreach(var enemyIceberg in enemyIcebergs){
                totalDistance += enemyIceberg.GetTurnsTillArrival(iceberg);
            }
            //System.Console.WriteLine($" average enemy {totalDistance/enemyIcebergs.Length} for {iceberg}");
            return totalDistance/enemyIcebergs.Length;

        }

        public static List<PenguinGroup> GetAttackingGroups(Game game, Iceberg dest, bool enemy = true, bool sorted = true)
        {
            var attackingGroups = new List<PenguinGroup>();
            var Groups = enemy ? game.GetEnemyPenguinGroups() : game.GetMyPenguinGroups();

            foreach (var group in Groups)
            {
                if (dest.Equals(group.Destination))
                {
                    attackingGroups.Add(group);
                }
            }

            if (sorted)
            {
                attackingGroups.Sort((x, y) => x.TurnsTillArrival.CompareTo(y.TurnsTillArrival));
            }
            return attackingGroups;
        }

    
        public static double IcebergPriority(Game game, Iceberg iceberg, int fl = 20)
        {
            if(game.GetMyIcebergs().Length == 1){return 0;}
            return iceberg.PenguinAmount + iceberg.PenguinsPerTurn * fl - AverageDistanceFromMyIcebergs(game, iceberg);
        }
    }
}