using PenguinGame;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MyBot
{
    public static class Defensive
    {
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

        public static void DefendIcebergs(Game game)
        {
            var icebergsInDanger = Utils.GetIcebergsInDanger(game);
            //TODO: improve priority
            icebergsInDanger = icebergsInDanger.OrderByDescending(x => Utils.GetIcebergPriority(game, x.Item1)).ToList();
            foreach(var icebergInDangerData in icebergsInDanger)
            {
                Utils.SendAmountWithTurnsLimit(game,icebergInDangerData.Item1,icebergInDangerData.Item2);
            }
        }
    }
}

