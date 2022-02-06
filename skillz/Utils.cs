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
            if (upgrade) { penguinPerTurnRate += iceberg.UpgradeValue; myIcebergCounter -= iceberg.UpgradeCost + 1; }
            enemyPgToTarget.ForEach(pg => combinedData.Add((-pg.PenguinAmount, pg.TurnsTillArrival)));
            myPgToTarget.ForEach(pg => combinedData.Add((pg.PenguinAmount, pg.TurnsTillArrival)));
            combinedData.Sort((u1, u2) => u1.Item2.CompareTo(u2.Item2));

            int sumCloseDistance = 0;
            myIcebergCounter -= additon;
            if(myIcebergCounter <= 0){
                //! do something
            }
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
                    game.Debug($"need to save {iceberg} with {myIcebergCounter*-1 - 1}");
                }
            }
            return result;
        }

        public static void SendAmountWithTurnsLimit(Game game, Iceberg dest, List<(int, int)> dataToSend)
        {
            foreach (var data in dataToSend)
            {
                int neededAmount = data.Item1;
                int timeToDeliver = data.Item2;
                var possibleDefenders = new List<Iceberg>();
                foreach (var myIceberg in Defensive.GetWall(game)) //! myicebergs?
                {
                    if (!myIceberg.Equals(dest) && dest.GetTurnsTillArrival(myIceberg) <= timeToDeliver && !GameInfo.UpgradedThisTurn(
                        myIceberg.UniqueId) && Utils.HelpIcebergData(game, myIceberg, 0).Count() == 0)
                    {
                        possibleDefenders.Add(myIceberg);
                    }
                }
                if (possibleDefenders.Count() > 0)
                {
                    var actuallyCanSend = new List<(Iceberg, int)>();
                    int sumDefenders = possibleDefenders.Sum(defender => defender.PenguinAmount);
                    if (sumDefenders >= neededAmount)
                    {
                        foreach (var ice in possibleDefenders)
                        {
                            double ratio = (double)ice.PenguinAmount / sumDefenders;
                            int amountToSend = (int)(ratio * neededAmount) + 1;
                            if (ice.PenguinAmount < amountToSend) { --amountToSend; }
                            bool safeToSend = Utils.HelpIcebergData(game, ice, amountToSend).Count() == 0;
                            while (!safeToSend && amountToSend > 0)
                            {
                                --amountToSend;
                                safeToSend = Utils.HelpIcebergData(game, ice, amountToSend).Count() == 0;
                            }
                            if (amountToSend > 0 && ice.CanSendPenguins(dest, amountToSend))
                            {
                                actuallyCanSend.Add((ice, amountToSend));
                            }
                            if (actuallyCanSend.Sum(x => x.Item2) >= neededAmount)
                            {
                                foreach (var protector in actuallyCanSend)
                                {
                                    protector.Item1.SendPenguins(dest, protector.Item2);
                                }
                            }

                        }
                    }
                }
            }
        }

        public static int WorstCaseEnemyReinforcment(Game game, Iceberg destIce, int turnsTillArrival)
        {
            var enemyIcebergs = game.GetEnemyIcebergs();
            bool neutral = destIce.Owner.Id == -1;
            int totalEnemies = destIce.PenguinAmount;
            if (!neutral)
            {
                totalEnemies += destIce.PenguinsPerTurn * turnsTillArrival;
            }
            foreach (var reinforcmentIce in enemyIcebergs)
            {
                if (!reinforcmentIce.Equals(destIce) &&
                    reinforcmentIce.GetTurnsTillArrival(destIce) <= turnsTillArrival)
                {
                    totalEnemies += reinforcmentIce.PenguinAmount + (reinforcmentIce.PenguinsPerTurn * turnsTillArrival) - 1;
                }
            }
            return totalEnemies;
        }

        public static int EnemyPenguinAmountAtArrival(Game game, Iceberg destination, int turnsTillArrival)
        {
            int destAmount = destination.PenguinAmount;
            if (destination.Owner.Id == -1)
            {
                return destAmount;
            }
            else
            {
                destAmount += destination.PenguinsPerTurn * turnsTillArrival;
                foreach (var k in game.GetEnemyPenguinGroups())
                {
                    if (k.Destination.Equals(destination) && k.TurnsTillArrival <= turnsTillArrival)
                    {
                        destAmount += k.PenguinAmount;
                    }
                }
                return destAmount;
            }
        }

        public static double AverageDistanceFromWall(Game game, Iceberg iceberg)
        {
            double totalDistance = 0;
            var walls = Defensive.GetWall(game);
            foreach (var wall in walls)
            {
                totalDistance += wall.GetTurnsTillArrival(iceberg);
            }
            return totalDistance / (walls.Count());
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
            int count = myIcbergs.Length;
            if (iceberg.Owner.Id == game.GetMyself().Id) { --count; }
            foreach (var myIceberg in myIcbergs)
            {
                if (!myIcbergs.Equals(iceberg))
                {
                    distance += myIceberg.GetTurnsTillArrival(iceberg);
                }
            }
            return distance / myIcbergs.Length;
        }

        public static double GetIcebergPriority(Game game, Iceberg iceberg, double lf = 20)
        {
            return (iceberg.Level * lf) + Utils.AverageDistanceFromWall(game, iceberg);
        }
    }


}

