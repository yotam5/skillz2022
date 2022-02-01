using PenguinGame;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MyBot {

    public static class Expand
    {
        /// <summary>
        /// return the closest netural taking to account distance and amount
        /// </summary>
        /// <param name="game">game handler</param>
        /// <param name="sourceIceberg">source iceberg for comparision</param>
        /// <returns>List<Iceberg></returns>
        public static List<Iceberg> GetClosestNeutral(Game game, Iceberg sourceIceberg)
        {
            var neutralIcebergs =  game.GetNeutralIcebergs().ToList();
            neutralIcebergs.Sort((x,y)=>x.GetTurnsTillArrival(sourceIceberg).CompareTo(y.GetTurnsTillArrival(sourceIceberg)));
            return neutralIcebergs;
        }
    }
}
    
        