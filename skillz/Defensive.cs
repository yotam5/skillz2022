using System.Collections.Generic;
using System.Text;
using PenguinGame;
using System.Linq;

namespace MyBot
{
    /// <summary>
    /// iceberg defenese mechanisem
    /// </summary>
    public static class Defensive
    {

        public static void DefenseMehcanisem(ResourceManager resourceManager)
        {

            System.Console.WriteLine('A');
            var data = Defensive.GetMyAttackedIcebergs(resourceManager);
            data.Sort((u1, u2) => RiskEvaluation(resourceManager, u1.Item1).CompareTo(RiskEvaluation(resourceManager, u2.Item1)));
            foreach(var c in data){
                System.Console.WriteLine($"{c.Item1.UniqueId}");
            }
            System.Console.WriteLine();
            var tk = new List<(SmartIceberg, SmartIceberg, int)>();
            if (data.Count() > 0)
            {
                var selected = data[0];
                int minimumToTakeOver = RiskEvaluation(resourceManager, selected.Item1);
                if (minimumToTakeOver > 0)
                {
                    System.Console.WriteLine("exiting defnese");
                    return;
                }
                minimumToTakeOver *= -1;
                foreach (var p in resourceManager.GetMyIcebergs())
                {
                    int startingAmount = 0;
                    int safeToSend = Defensive.RiskEvaluation(resourceManager, p) ;
                    System.Console.WriteLine($"safetosend is {safeToSend}");
                    if(p.upgraded)
                    {
                        System.Console.WriteLine("ice already did upgrde can send");
                        continue;
                    }
                    if (safeToSend < 0) //FIX?
                    {
                        continue;
                    }
                    System.Console.WriteLine('B');
                    while (Defensive.RiskEvaluation(resourceManager, p, additionalAmount: -startingAmount) > 0&& startingAmount < minimumToTakeOver)
                    {
                        startingAmount++;
                    }
                    System.Console.WriteLine($"min achived is {startingAmount}");
                    tk.Add((p, selected.Item1, startingAmount));
                    minimumToTakeOver -= startingAmount;
                }
                if (minimumToTakeOver <= 0)
                {
                    foreach (var c in tk)
                    {
                        System.Console.WriteLine($"c is {c.Item2.Id}");
                        System.Console.WriteLine($"iceberg sending {c.Item1.Id}");
                        c.Item1.SendPenguins(c.Item2, c.Item3);
                    }
                }
                    System.Console.WriteLine('C');

            }
            else
            {
                System.Console.WriteLine("no iceberg to protect");
            }
        }

        /// <summary>
        /// check if iceberg will be conqured at a given turn and its near future i guess
        /// </summary>
        /// <param name="resourceManager"></param>
        /// <param name="target">targeted SmartIceberg</param>
        /// <param name="upgrade">check if iceberg is safe when upgraded</param>
        /// <param name="additionalAmount">change the amount on the iceberg</param>
        /// <returns>signed integer</returns>
        public static int RiskEvaluation(ResourceManager resourceManager,
            SmartIceberg target, bool upgrade = false, int additionalAmount = 1
            )
        {
            //TODO: consider if close to upgraded iceberg os if by itself
            var enemyPgToTarget = Defensive.GetAttackingGroups(resourceManager, target, true);
            var myPgToTarget = Defensive.GetAttackingGroups(resourceManager, target, false);

            int myIcebergPenguinAmount = target.PenguinAmount;
            int penguinPerTurnRate = target.PenguinsPerTurn;
            if (upgrade) { penguinPerTurnRate += target.UpgradeValue; }
            int level = target.Level;

            int myIcebergCounter = myIcebergPenguinAmount + additionalAmount;
            //TODO: sort all pg groups and do it by distance
            List<PenguinGroup> combinedGroups = new List<PenguinGroup>();
            enemyPgToTarget.ForEach(pg => combinedGroups.Add(pg));
            myPgToTarget.ForEach(pg => combinedGroups.Add(pg));
            combinedGroups.Sort((u1, u2) => u1.TurnsTillArrival.CompareTo(u2.TurnsTillArrival));

            for (;combinedGroups.Count() > 0;) //NOTE: need to add neutral situatio
            {
                int closeDistance = combinedGroups[0].TurnsTillArrival;
                combinedGroups.ForEach(pgk=>pgk.TurnsTillArrival-=closeDistance);
                var arrived = (from pgk in combinedGroups where pgk.TurnsTillArrival == 0 select pgk).ToList();
                for (int i = arrived.Count(); i > 0; i--, combinedGroups.RemoveAt(0)) ;
                foreach(var pkg in arrived)
                {
                    if(myIcebergCounter > 0)
                    {
                        myIcebergCounter += penguinPerTurnRate * closeDistance ;
                    }
                    else {
                            myIcebergCounter -= penguinPerTurnRate * closeDistance ;
                    }
                    myIcebergCounter += (pkg.Owner.Equals(resourceManager.GetMyself())) ? pkg.PenguinAmount : -pkg.PenguinAmount;
                }

            }
            System.Console.WriteLine($"ice {target.UniqueId} will be {myIcebergCounter}");
            System.Console.WriteLine('F');
            return myIcebergCounter;
        }


        /// <summary>
        /// return icebergs that are attacked by enemy with pg groups who are targeting it, Item2-my pg, Item3-enemy pg
        /// </summary>
        /// <param name="resourceManager"></param>
        public static List<(SmartIceberg, List<PenguinGroup>, List<PenguinGroup>)> GetMyAttackedIcebergs(ResourceManager resourceManager)
        {
            var myIcebergs = resourceManager.GetMyIcebergs();
            var data = new List<(SmartIceberg, List<PenguinGroup>, List<PenguinGroup>)>();
            foreach (var iceberg in myIcebergs)
            {
                var enemyAttackingGroups = Defensive.GetAttackingGroups(resourceManager, iceberg, true, true);
                var myAttackingGroups = Defensive.GetAttackingGroups(resourceManager, iceberg, false, true);
                data.Add((iceberg, myAttackingGroups, enemyAttackingGroups));
            }
            return data;

        }


        /// <summary>
        /// find groups that targeting specific iceberg
        /// </summary>
        /// <param name="resourceManager">Resourcehandler</param>
        /// <param name="dest">groups destination</param>
        /// <param name="enemy">my groups or enemy groups</param>
        /// <param name="sorted">boolean if sorting by distance</param>
        /// <returns>List<PenguinGroup></returns>
        public static List<PenguinGroup> GetAttackingGroups(ResourceManager resourceManager, SmartIceberg dest, bool enemy = true, bool sorted = true)
        {
            var attackingGroups = new List<PenguinGroup>();
            var Groups = enemy ? resourceManager.GetEnemyPenguinGroups() : resourceManager.GetMyPenguinGroups();

            //loop over each enemy group and check if they are going to dest
            foreach (var group in Groups)
            {
                if (dest.Equals(group.Destination))
                {
                    //System.Console.WriteLine($"free iceberg in {dest}");
                    attackingGroups.Add(group);
                }
            }
            //System.Console.WriteLine($"attack count {attackingGroups.Count}");

            if (sorted)
            {
                //sort the groups by asending order, probably
                attackingGroups.Sort((x, y) => x.TurnsTillArrival.CompareTo(y.TurnsTillArrival));
                return attackingGroups;
            }
            return attackingGroups;
        }

    }


}
