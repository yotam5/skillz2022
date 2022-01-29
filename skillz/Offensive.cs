using System.Collections.Generic;
using System.Text;
using PenguinGame;
using System.Linq;

namespace MyBot
{
    /*
    deals with offensive yandres
    */
    public static class Offensive
    {   

        //find groups attacking specific island and return list that contains them

        public static List<PenguinGroup> GetAttackingGroups(ResourceManager resourceManager, SmartIceberg dest, bool enemy=true,  bool sorted = true)
        {
            var attackingGroups = new List<PenguinGroup>();
            var Groups = enemy ? resourceManager.GetEnemyPenguinGroups() : resourceManager.GetMyPenguinGroups();

            //loop over each enemy group and check if they are going to dest
            foreach (var group in Groups) 
            {
                try{
                    //System.Console.WriteLine($"getattacking {Groups}");

                    //System.Console.WriteLine($"group is {group}");
                    //System.Console.WriteLine($"dest is {dest.UniqueId} {dest.Id}");
                }
                catch {

                }
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