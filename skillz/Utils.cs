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
        public static List<(int, int)> HelpIcebergData(Game game, Iceberg iceberg, bool upgrade = false, int addition = 0)
        {
            var enemyPgToTarget = Utils.GetAttackingGroups(game, iceberg);
            var myPgToTarget = Utils.GetAttackingGroups(game, iceberg, false);
            List<int[]> combinedData = new List<int[]>();
            List<(int, int)> result = new List<(int, int)>();
            Player myPlayer = game.GetMyself();
            int penguinPerTurnRate = iceberg.PenguinsPerTurn;
            
            //int icebergLevel = iceberg.Level;

            if (upgrade) { penguinPerTurnRate += iceberg.UpgradeValue; }

            enemyPgToTarget.ForEach(pg => combinedData.Add(new int[] { -pg.PenguinAmount, pg.TurnsTillArrival }));
            myPgToTarget.ForEach(pg => combinedData.Add(new int[] { pg.PenguinAmount, pg.TurnsTillArrival }));
            combinedData.Sort((u1, u2) => u1[1].CompareTo(u2[1]));

            int sumCloseDistance = 0;
            int myIcebergCounter = iceberg.PenguinAmount;
            myIcebergCounter -= addition;

            while (combinedData.Count() > 0)
            {
                int closestDistance = combinedData[0][1];
                sumCloseDistance += closestDistance;
                foreach (var pg in combinedData)
                {
                    pg[1] -= closestDistance;
                }
                var arrived = (from pg in combinedData where pg[1] == 0 select pg[0]).ToList();
                for (int i = arrived.Count(); i > 0; i--, combinedData.RemoveAt(0)) ;
                int sumArrived = arrived.Sum();
                myIcebergCounter += penguinPerTurnRate * closestDistance + sumArrived;
                if (myIcebergCounter <= 0)
                {
                    result.Add((-1 * myIcebergCounter + 1, sumCloseDistance));
                    game.Debug($"need to save {iceberg} with {myIcebergCounter - 1}");
                }

            }

            return result;
        }
        public static double AverageDistanceFromEnemy(Game game, Iceberg iceberg)
        {
            double distance = 0;
            var enemIcbergs = game.GetEnemyIcebergs();
            foreach(var enemIce in enemIcbergs)
            {
                distance += enemIce.GetTurnsTillArrival(iceberg);
            }
            return distance / enemIcbergs.Length;
        }

        public static double AverageDistanceFromMyIcbergs(Game game, Iceberg iceberg)
        {
            double distance = 0;
            var myIcbergs = game.GetMyIcebergs();
            foreach(var myIceberg in myIcbergs)
            {
                if(!myIcbergs.Equals(iceberg)){
                    distance += myIceberg.GetTurnsTillArrival(iceberg);
                }
            }
            return distance / (myIcbergs.Length -1);
        } 

    


    }


}

