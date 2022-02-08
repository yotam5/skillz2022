using PenguinGame;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MyBot
{
    public static class Defensive
    {
        /// <summary>
        /// return array that contain the wall
        /// </summary>
        /// <param name="game"></param>
        /// <returns>Iceberg[] containing the wall</returns>
        public static Iceberg[] GetWall(Game game)
        {
            var distances = new List<(Iceberg, double)>();
            foreach (var myIceberg in game.GetMyIcebergs())
            {
                distances.Add((myIceberg, Utils.AverageDistanceFromEnemy(game, myIceberg)));
            }

            distances.Sort((x, y) => x.Item2.CompareTo(y.Item2));
            foreach (var k in distances)
            {
                //System.Console.WriteLine($"ice {k.Item1} dis {k.Item2}");
            }
            Iceberg[] theWalls = { distances[0].Item1 };
            if (distances.Count() > 1)
            {
                theWalls = theWalls.Append(distances[1].Item1).ToArray();
            }

            return theWalls;
        }

        /// <summary>
        /// defend our icebergs
        /// </summary>
        /// <param name="game"></param>
        public static void DefendIcebergs(Game game)
        {
            var icebergsInDanger = Utils.GetIcebergsInDanger(game);
            //TODO: improve priority
            icebergsInDanger = icebergsInDanger.OrderByDescending(x => Utils.GetIcebergPriority(game, x.Item1)).ToList();
            foreach(var ice in icebergsInDanger)
            {
                //System.Console.WriteLine($"danger ice {ice}");
                if(Utils.HelpIcebergData(game,ice.Item1,0,true).Count() == 0)
                {
                    //! need to upgrade if possible
                }
            }
            foreach(var icebergInDangerData in icebergsInDanger)
            {
                if(!Utils.SendAmountWithTurnsLimit(game,icebergInDangerData.Item1,icebergInDangerData.Item2))
                {
                    
                }
            }
        }
    }
}

