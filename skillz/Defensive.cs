using PenguinGame;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MyBot
{
    public static class Defensive
    {
        public static Iceberg GetWall(Game game)
        {
            var distances = new List<(Iceberg, double)>();
            foreach (var myIceberg in game.GetMyIcebergs())
            {
                distances.Add((myIceberg, Utils.AverageDistanceFromEnemy(game, myIceberg)));
            }

            distances.Sort((x, y) => x.Item2.CompareTo(y.Item2));
            foreach (var k in distances)
            {
                System.Console.WriteLine($"ice {k.Item1} dis {k.Item2}");
            }
            return distances[0].Item1;
        }

        public static void DefendIcebergs(Game game)
        {
            var icebergsInDanger = Utils.GetIcebergsInDanger(game);
            //TODO: improve priority
            icebergsInDanger = icebergsInDanger.OrderByDescending(x => Utils.GetIcebergPriority(game, x.Item1)).ToList();
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
    }

}

