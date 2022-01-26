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
        public static List<PenguinGroup> GetAttackingGroups(Game game,Iceberg dest,bool sorted=true){
            List<PenguinGroup> attackingGroups = new List<PenguinGroup>();
            var EnemyGroups = game.GetEnemyPenguinGroups();
            foreach(var group in EnemyGroups){
                if(group.Destination == dest){
                    System.Console.WriteLine($"kdkdkd");
                    attackingGroups.Add(group);
                }
            }
                    System.Console.WriteLine($"attack count {attackingGroups.Count}");

            if(sorted){
                    attackingGroups.Sort((x, y) => x.TurnsTillArrival.CompareTo(y.TurnsTillArrival));
                    return attackingGroups;


            }
            return attackingGroups;
        }

    }
    
}