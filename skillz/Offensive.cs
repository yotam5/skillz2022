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
        public static List<PenguinGroup> findAttackingGroup(Game game,Iceberg dest){
            List<PenguinGroup> attackingGroups = new List<PenguinGroup>();
            var EnemyGroups = game.GetEnemyPenguinGroups();
            foreach(var group in EnemyGroups){
                if(group.Destination == dest){
                    attackingGroups.Append(group);
                }
            }
            return attackingGroups;
        }
    }
    
}