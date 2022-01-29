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
            var data = Defensive.GetMyAttackedIcebergs(resourceManager);
            data.Sort((u1,u2)=>RiskEvaluation(resourceManager,u1).CompareTo(RiskEvaluation(resourceManager,u2)));
            var tk = new List<(SmartIceberg,SmartIceberg,int)>();
            if(data.Count() > 0)
            {
                var selected = data[0];
                int minimumToTakeOver = RiskEvaluation(resourceManager,selected);
                if(minimumToTakeOver > 0){
                    return;
                }
                minimumToTakeOver*=-1;
                foreach(var p in resourceManager.GetMyIcebergs()){
                    if(p.UniqueId == selected.Item1.UniqueId)
                    {
                        continue;
                    }
                    int startingAmount = 0;
                    bool safeToSend = Expand.SafeToSend(resourceManager, p, 0);
                    if (!safeToSend) //FIX?
                    {
                        continue;
                    }
                    while (Expand.SafeToSend(resourceManager, p, startingAmount) && startingAmount < minimumToTakeOver)
                    {
                        startingAmount++;
                    }
                    System.Console.WriteLine($"min achived is {startingAmount}");
                    tk.Add((p,selected.Item1, startingAmount));
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
            }
        }

        public static int RiskEvaluation(ResourceManager resourceManager,
            (SmartIceberg, List<PenguinGroup>, List<PenguinGroup>) data,
            double lf = 1.0, double ldf = 1.0)
        {
            double risk = 0;

            int myIcebergPenguinAmount = data.Item1.PenguinAmount;
            int penguinPerTurnRate = data.Item1.PenguinsPerTurn;
            int level = data.Item1.Level;

            int myIcebergCounter = myIcebergPenguinAmount;
            //TODO: sort all pg groups and do it by distance
            List<PenguinGroup> combinedGroups = new List<PenguinGroup>();
            data.Item2.ForEach(pg => combinedGroups.Add(pg));
            data.Item3.ForEach(pg => combinedGroups.Add(pg));
            combinedGroups.Sort((u1, u2) => u1.TurnsTillArrival.CompareTo(u2.TurnsTillArrival));
            foreach (var pg in combinedGroups) //NOTE: need to add neutral situatio
            {
                bool myGp = pg.Owner.Equals(resourceManager.GetMyself());
                int additionVector1 = myGp ? 1 : -1;
                int additionVector2 = (myIcebergCounter > 0) ? 1 : -1;
                myIcebergCounter += pg.PenguinAmount*additionVector1;
                myIcebergCounter += penguinPerTurnRate * pg.TurnsTillArrival*additionVector2;
            }
            return myIcebergCounter;
            //risk += myIcebergCounter;
            //System.Console.WriteLine($"ice {data.Item1.UniqueId} will be {myIcebergCounter}");
            //return risk;
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
