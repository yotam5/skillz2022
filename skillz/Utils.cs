using PenguinGame;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace MyBot
{
    public static class Utils
    {

        /// <summary>
        /// return penguins groups that are targeting specific iceberg
        /// </summary>
        /// <param name="game"></param>
        /// <param name="destination">destination of the pg groups</param>
        /// <param name="enemy">enemy groups or your groups</param>
        /// <returns>list of type PenguinGroup</returns>
        public static List<PenguinGroup> GetAttackingGroups(Game game, Iceberg destination, bool enemy = true)
        {
            var pgGroups = enemy ? game.GetEnemyPenguinGroups() : game.GetMyPenguinGroups();
            var pgList = new List<PenguinGroup>();
            foreach (var pg in pgGroups)
            {
                if (pg.Destination.Equals(destination))
                {
                    pgList.Add(pg);
                }
            }
            return pgList;
        }

        public static List<(int, int)> HelpIcebergData(Game game, Iceberg iceberg, int additon, bool upgrade = false)
        {
            var enemyPgToTarget = Utils.GetAttackingGroups(game, iceberg);
            var myPgToTarget = Utils.GetAttackingGroups(game, iceberg, false);
            var combinedData = new List<(int, int)>();
            var result = new List<(int, int)>();
            int myId = game.GetMyself().Id;
            int penguinPerTurnRate = iceberg.PenguinsPerTurn;
            int myIcebergCounter = iceberg.PenguinAmount;
            if (upgrade) { penguinPerTurnRate += iceberg.UpgradeValue; myIcebergCounter -= iceberg.UpgradeCost; }
            enemyPgToTarget.ForEach(pg => combinedData.Add((-pg.PenguinAmount, pg.TurnsTillArrival)));
            myPgToTarget.ForEach(pg => combinedData.Add((pg.PenguinAmount, pg.TurnsTillArrival)));
            combinedData.Sort((u1, u2) => u1.Item2.CompareTo(u2.Item2));

            int sumCloseDistance = 0;
            myIcebergCounter -= additon;

            while (combinedData.Count() > 0)
            {
                int closest = combinedData.First().Item2;
                sumCloseDistance += closest;
                for (int i = 0; i < combinedData.Count(); i++)
                {
                    combinedData[i] = (combinedData[i].Item1, combinedData[i].Item2 - closest);
                }
                var arrived = (from pg in combinedData where pg.Item2 == 0 select pg.Item1).ToList();
                for (int i = arrived.Count(); i > 0; i--, combinedData.RemoveAt(0)) ;
                myIcebergCounter += closest * penguinPerTurnRate + arrived.Sum();
                if (myIcebergCounter <= 0)
                {
                    result.Add((-1 * myIcebergCounter + 1, sumCloseDistance));
                    game.Debug($"need to save {iceberg} with {myIcebergCounter - 1}");
                }
            }
            return result;
        }

        public static int EnemyPenguinAmountAtArrival(Game game, Iceberg destination, int turnsTillArrival)
        {
            int destAmount = destination.PenguinAmount;
            if(destination.Owner.Id == -1)
            {
                return destAmount;
            }
            else
            {
                destAmount += destination.PenguinsPerTurn * turnsTillArrival;
                foreach(var k in game.GetEnemyPenguinGroups())
                {
                    if(k.Destination.Equals(destination) && k.TurnsTillArrival <= turnsTillArrival)
                    {
                        destAmount += k.PenguinAmount;
                    }
                }
                return destAmount;
            }
        }

        public static List<(Iceberg, List<(int, int)>)> GetIcebergsInDanger(Game game)
        {
            var icebergsInDanger = new List<(Iceberg, List<(int, int)>)>();
            foreach (var iceberg in game.GetMyIcebergs())
            {
                var helpData = Utils.HelpIcebergData(game, iceberg, 0);
                if (helpData.Count() > 0)
                {
                    icebergsInDanger.Add((iceberg, helpData));
                }
            }
            return icebergsInDanger;
        }

        public static double AverageDistanceFromEnemy(Game game, Iceberg iceberg)
        {
            double distance = 0;
            var enemIcbergs = game.GetEnemyIcebergs();
            int sub = (iceberg.Owner.Id == game.GetEnemy().Id) ? -1 : 0;
            foreach (var enemIce in enemIcbergs)
            {
                if (!enemIce.Equals(iceberg))
                {
                    distance += enemIce.GetTurnsTillArrival(iceberg);
                }
            }
            return distance / (enemIcbergs.Length - sub);
        }

        public static double AverageDistanceFromMyIcbergs(Game game, Iceberg iceberg)
        {
            double distance = 0;
            var myIcbergs = game.GetMyIcebergs();
            foreach (var myIceberg in myIcbergs)
            {
                if (!myIcbergs.Equals(iceberg))
                {
                    distance += myIceberg.GetTurnsTillArrival(iceberg);
                }
            }
            return distance / (myIcbergs.Length - 1);
        }

        public static double GetIcebergPriority(Game game, Iceberg iceberg, double lf = 20)
        {
            return iceberg.Level * lf + Utils.AverageDistanceFromEnemy(game, iceberg);
        }
    }


}

