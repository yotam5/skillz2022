using PenguinGame;
using System.Collections.Generic;
using System.Linq;


namespace MyBot
{
    public static class Utils
    {
        public static List<SmartIceberg> ConvertToSmartIceberg(Iceberg[] icebergs)
        {
            var convertedIcebergs = new List<SmartIceberg>();
            foreach (var iceberg in icebergs)
            {
                convertedIcebergs.Add(new SmartIceberg(iceberg));
            }
            return convertedIcebergs;
        }

        public static void ConfigureIcebergs()
        {
            foreach (var myIceberg in GameInfo.Icebergs.myIcebergs)
            {
                if (!GameInfo.Icebergs.threatenedIcebergs.Contains(myIceberg))
                {
                    var inco = myIceberg.GetIncomingEnemyGroups();
                    if (inco.Count() != 0)
                    {
                        //myIceberg.SavePenguins(myIceberg.PenguinAmount - inco.Sum(x=>x.PenguinAmount)); //! need to change
                    }
                }
            }
        }

        public static void CalculateMissions()
        {
            foreach(var m in GameInfo.Groups.allMissions)
            {
                m.CalculateExecutionWays();
            }
        }

        public static List<SmartIceberg> NotMyIcebergs(Game game)
        {
            var notMine = new List<SmartIceberg>();
            foreach (var iceberg in game.GetAllIcebergs())
            {
                if (iceberg.Owner.Id != GameInfo.Players.mySelf.Id)
                {
                    notMine.Add(new SmartIceberg(iceberg));
                }
            }
            return notMine;
        }

        public static List<SmartIceberg> GetMyDangeredIcebergs()
        {

            var dangered = new List<SmartIceberg>();
            foreach (var myIce in GameInfo.Icebergs.myIcebergs.ToList())
            {
                var helpData = myIce.FutureState();
                if (helpData.Count() > 0)
                {
                    dangered.Add(myIce);
                }
            }
            return dangered;
        }

    }
}