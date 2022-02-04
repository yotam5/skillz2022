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

        public static Iceberg[] NClosestIcebergsToWall(Game game, int n){
            List<(Iceberg, int)> icebergsdistance = new List<(Iceberg, int)>();
            foreach (var iceberg in game.GetAllIcebergs()){
                icebergsdistance.Add((iceberg, Defensive.GetWall(game)[0].GetTurnsTillArrival(iceberg)));
            }
            icebergsdistance.OrderBy(ib => ib.Item2);
            List<Iceberg> result = new List<Iceberg>();
            for(int i = 0; i < n; i++){
                try{
                    result.Add(icebergsdistance[i].Item1);
                }
                catch{
                }
            }

            return result.ToArray();
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
            //if (upgrade) {myIcebergCounter -= iceberg.UpgradeCost; }
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
                int sumArrived = arrived.Sum();
                myIcebergCounter += closest * penguinPerTurnRate + sumArrived;
                if (myIcebergCounter <= 0)
                {
                    result.Add((-1 * myIcebergCounter + 1, sumCloseDistance));
                    game.Debug($"need to save {iceberg} with {myIcebergCounter - 1}");
                }
            }
            return result;
        }

        public static int BackupAtArrival(Game game, Iceberg destination , int turnsTillArrival)
        {
            var enemyPgToTarget = Utils.GetAttackingGroups(game, destination); //enemy groups
            var myPgToTarget = Utils.GetAttackingGroups(game, destination, false); //my groups
            var combinedGroups = new List<(int,int)>(); //item1 amount, item2 distance
            foreach (var pg in enemyPgToTarget)
            {
                if (pg.TurnsTillArrival <= turnsTillArrival)
                {
                    combinedGroups.Add((-pg.PenguinAmount,pg.TurnsTillArrival));
                }
            }
            foreach (var pg in myPgToTarget)
            {
                if (pg.TurnsTillArrival <= turnsTillArrival)
                {
                    combinedGroups.Add((pg.PenguinAmount,pg.TurnsTillArrival));
                }
            }
            combinedGroups.OrderBy(pg=>pg.Item2); //order by distance

            bool enemy = destination.Owner.Id == game.GetEnemy().Id;
            bool neutral = destination.Owner.Id == -1;
            int destinationAmount = destination.PenguinAmount;
            int penguinsPerTurn = destination.PenguinsPerTurn;
            if(enemy)
            {
                destinationAmount*=-1;
            }
            int currTurn = 0;
            int index = 0;
            while(combinedGroups.Count() > index)
            {   
                var currGrp = combinedGroups[index];
                if(enemy)
                {
                    destinationAmount -= penguinsPerTurn* (currGrp.Item2 - currTurn);
                }
                else if (!neutral)
                {
                    destinationAmount += penguinsPerTurn* (currGrp.Item2 - currTurn);
                }
                if(neutral)
                {
                    destinationAmount += -System.Math.Abs(currGrp.Item1);

                    if(destinationAmount <= 0)
                    {
                        neutral = false;

                        if(currGrp.Item1 < 0)
                        {
                            enemy = true;
                        }
                    }
                }
                else
                {
                    destinationAmount += currGrp.Item1;

                    if(enemy && destinationAmount > 0)
                    {
                        enemy = false;
                    }
                    else if(!enemy && destinationAmount < 0)
                    {
                        enemy = true;
                    }
                }
                currTurn = currGrp.Item2;
                index++;
            }
            
            return destinationAmount;
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
            foreach (var enemIce in enemIcbergs)
            {
                distance += enemIce.GetTurnsTillArrival(iceberg);
            }
            return distance / enemIcbergs.Length;
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

        public static double GetIcebergPriority(Game game, Iceberg iceberg,double lf=20)
        {
            return iceberg.Level * lf + Utils.AverageDistanceFromEnemy(game,iceberg);
        }
    }


}

