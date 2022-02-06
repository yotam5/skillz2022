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
            foreach (var icebergInDangerData in icebergsInDanger)
            {
                var iceToDefend = icebergInDangerData.Item1;
                var defendeData = icebergInDangerData.Item2;

                foreach (var data in defendeData)
                {
                    int neededAmount = data.Item1;
                    int timeToDeliver = data.Item2;
                    var possibleDefenders = new List<Iceberg>();
                    foreach (var myIceberg in GetWall(game))
                    {
                        if (!myIceberg.Equals(iceToDefend) && iceToDefend.GetTurnsTillArrival(myIceberg) <= timeToDeliver && !GameInfo.UpgradedThisTurn(
                            myIceberg.UniqueId) && Utils.HelpIcebergData(game,myIceberg,0).Count() == 0)                        {
                            possibleDefenders.Add(myIceberg);
                        }
                    }
                    if (possibleDefenders.Count() > 0)
                    {
                        var protectors = new List<(Iceberg,int)>();
                        int sumDefenders = possibleDefenders.Sum(defender => defender.PenguinAmount);
                        if (sumDefenders >= neededAmount)
                        {
                            foreach (var ice in possibleDefenders)
                            {
                                double ratio = (double)ice.PenguinAmount / sumDefenders;
                                int amountToSend = (int)(ratio * neededAmount) + 1;
                                if(ice.PenguinAmount < amountToSend){--amountToSend;}
                                bool safeToSend = Utils.HelpIcebergData(game, ice, amountToSend).Count() == 0;
                                while (!safeToSend && amountToSend > 0)
                                {
                                    --amountToSend;
                                    safeToSend = Utils.HelpIcebergData(game, ice, amountToSend).Count() == 0;
                                }
                                if(amountToSend > 0 && ice.CanSendPenguins(iceToDefend,amountToSend))
                                {
                                    protectors.Add((ice,amountToSend));
                                }
                                if(protectors.Sum(x=>x.Item2) >= neededAmount)
                                {
                                    foreach(var protector in protectors)
                                    {
                                        protector.Item1.SendPenguins(iceToDefend,protector.Item2);
                                    }
                                }
                        }
                    }
                }
            }

        }
    }
}
}

