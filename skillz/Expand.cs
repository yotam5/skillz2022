using System.Collections.Generic;
using System.Text;
using PenguinGame;
using System.Linq;

namespace MyBot
{   
    /*
    deals with expansion, neutrals island for now
    */
    public static class Expand
    {   
        //return list of the closest neutrals islands
        //check the distance in reverse but its the same (source -> dest = dest -> source)
        public static List<Iceberg>GetClosestNeutral(Game game, Iceberg source_iceberg, Iceberg[] neutrals_islands){
                return neutrals_islands.OrderBy(dest=>dest.GetTurnsTillArrival(source_iceberg)).ToList();
        }
    }
}