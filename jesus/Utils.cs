using PenguinGame;
using System.Collections.Generic;
using System.Linq;


namespace MyBot
{
    public static class Utils
    {
        /// <summary>
        /// convert game iceberg object to smarticeberg
        /// </summary>
        /// <param name="icebergs">list of game icebergs</param>
        /// <returns>converted game icebergs</returns>
        public static List<SmartIceberg> ConvertToSmartIceberg(Iceberg[] icebergs)
        {
            var convertedIcebergs = new List<SmartIceberg>();
            foreach (var iceberg in icebergs)
            {
                convertedIcebergs.Add(new SmartIceberg(iceberg));
            }
            return convertedIcebergs;
        }

        public static List<SmartIceberg> GetWalls()
        {

            //!NOTE: instead of distance to use how close to enemy amount, one is closer to enemy but to less of it
            var walls = new List<SmartIceberg>();
            var myIcebergs = GameInfo.Icebergs.myIcebergs.ToList();
            var myIcebergsHelper = GameInfo.Icebergs.myIcebergs.ToList();
            var enemyIcebergs = GameInfo.Icebergs.enemyIcebergs.ToList();
            if (myIcebergs.Count() < 2)
            {
                return myIcebergs;
            }
            foreach (var myIce in GameInfo.Icebergs.myIcebergs.ToList())
            {
                var closestEnemy = enemyIcebergs.OrderBy(x => x.GetTurnsTillArrival(myIce)).First();
                int enemyDistance = closestEnemy.GetTurnsTillArrival(myIce);
                var closestAlly = myIcebergsHelper.OrderBy(x => x.GetTurnsTillArrival(myIce)).ToList()[1]; //0 is the iceberg itself
                int allyDistance = closestAlly.GetTurnsTillArrival(myIce);
                //System.Console.WriteLine($"{myIce} enem:{enemyDistance} ally {allyDistance}");
                if (enemyDistance <= allyDistance)
                {
                    walls.Add(myIce);
                }
            }

            //System.Console.WriteLine($"walls count {walls.Count()}");
            if (walls.Count() >= 2)
            {
                return GetWallsHelper(walls);
            }
            var result = new List<SmartIceberg>();
            var ordered = myIcebergs.OrderBy(x => x.AverageDistanceFromPlayer(GameInfo.Players.enemyPlayer.Id)).ToList();
            double min = 999;
            foreach (var ice in ordered)
            {
                double newMin = ice.AverageDistanceFromPlayer(GameInfo.Players.enemyPlayer.Id);
                if (newMin < min || System.Math.Abs(newMin - min) <= 2)
                {
                    result.Add(ice);
                    min = newMin;
                }
            }

            //result.ForEach(x => System.Console.WriteLine(x));
            return result;
        }
        public static void allornothing()
        {
            foreach(var ice in GameInfo.Icebergs.myIcebergs.ToList())
            {
                foreach(var enem in GameInfo.Icebergs.enemyIcebergs.ToList())
                {
                    int turnsWithBridge = (int)ice.GetTurnsTillArrivalWithBridge(enem);
                    if(ice.CanCreateBridge(enem) && ice.PenguinAmount - GameInfo.Bridge.bridgeCost -1 > enem.PotentialBackup(turnsWithBridge,
                        enem.Owner.Id) && ice.PreventConqure(addition: ice.PenguinAmount - GameInfo.Bridge.bridgeCost -1).Count() == 0)
                    {
                        ice.CreateBridge(enem);
                        break;
                    }
                }
            }
        }
        public static List<(int, int)> PreventNeutralTaken(SmartIceberg dest)
        {
            //! need to take into account icebergs that still didnt send
            int myId = GameInfo.Players.mySelf.Id;
            var incomingGroups = new List<(int, int)>();

            var helpData = new List<(int, int)>();

            int penguinPerTurn = dest.PenguinsPerTurn;

            int icebergCounter = dest.PenguinAmount;
            icebergCounter *= -1;
            int currentOwnerId = dest.Owner.Id;

            foreach (var group in dest.AllIncomingPenguinGroups())
            {
                int turnsTillArrival = Utils.TurnsTillArrivalWithBridge(group);
                if (group.Owner.Id == myId)
                {
                    incomingGroups.Add((group.PenguinAmount, turnsTillArrival));
                }
                else
                {
                    incomingGroups.Add((-group.PenguinAmount, turnsTillArrival));
                }
            }
            bool n = true;
            incomingGroups = incomingGroups.OrderBy(x => x.Item2).ToList();
            int totalDistance = 0;
            while (incomingGroups.Count() > 0)
            {
                int closestGroup = incomingGroups.First().Item2;
                totalDistance += closestGroup;
                for (int i = 0; i < incomingGroups.Count(); i++)
                {
                    incomingGroups[i] = (incomingGroups[i].Item1, incomingGroups[i].Item2 - closestGroup);
                }
                var arrived = (from pg in incomingGroups where pg.Item2 == 0 select pg.Item1).ToList();
                for (int i = arrived.Count(); i > 0; i--, incomingGroups.RemoveAt(0)) ;
                icebergCounter += arrived.Sum();
                if (!n)
                {
                    icebergCounter += closestGroup * penguinPerTurn;
                }
                if (n && icebergCounter > 0)
                {
                    currentOwnerId = myId;
                    n = false;
                }

                if (icebergCounter <= 0)
                {
                    helpData.Add((-1 * icebergCounter + 1, totalDistance));
                    //System.Console.WriteLine("detected!");
                    break;
                }
            }
            return helpData;
        }

        private static List<SmartIceberg> GetWallsHelper(List<SmartIceberg> walls)
        {
            //System.Console.WriteLine("helper");
            if (walls.Count() < 2)
            {
                //walls.ForEach(x => System.Console.WriteLine(walls));
                return walls;
            }
            var wallsCopy = walls.ToList();
            var recalculatedWalls = new List<SmartIceberg>();
            var enemyIcebergs = GameInfo.Icebergs.enemyIcebergs.ToList();
            foreach (var wall in walls)
            {
                wallsCopy = wallsCopy.OrderBy(x => x.GetTurnsTillArrival(wall)).ToList();
                var closestAlly = wallsCopy[1];
                int allyDistance = closestAlly.GetTurnsTillArrival(wall);
                var closestEnemy = enemyIcebergs.OrderBy(x => x.GetTurnsTillArrival(wall)).First();
                int enemyDistance = closestEnemy.GetTurnsTillArrival(wall);
                if (allyDistance >= enemyDistance)
                {
                    recalculatedWalls.Add(wall);
                }
            }
            //recalculatedWalls.ForEach(x => System.Console.WriteLine(x));
            return recalculatedWalls;
        }

        public static int GetBonusUntillArrival(SmartIceberg iceberg, int turns)
        {
            int totalBonus = 0;
            var ignore = new List<int> { GameInfo.Players.mySelf.Id, GameInfo.Players.neutral.Id };
            if (ignore.Contains(GameInfo.Bonus.bonusIceberg.Owner.Id))
            {
                return 0;
            }
            int turnsLeftToBonus = GameInfo.Bonus.turnsLeftToBonus;
            int bonusAmount = GameInfo.Bonus.bonusAmount;
            int bonusCycle = GameInfo.Bonus.bonusCycle;
            if (turnsLeftToBonus <= turns)
            {
                return bonusAmount;
            }
            totalBonus += bonusAmount;
            int start = turnsLeftToBonus + bonusCycle;
            while (start <= turns)
            {
                totalBonus += bonusAmount;
                start += bonusCycle;
            }
            return totalBonus;

        }

        public static int TurnsTillArrivalWithBridge(PenguinGroup pgGroup, bool worst = true)
        {
            int withoutBridge = pgGroup.TurnsTillArrival;
            if (worst)
            {
                return (int)(System.Math.Floor(withoutBridge / GameInfo.Bridge.bridgeSpeed));
            }
            if (pgGroup.Source.GetType() == typeof(Iceberg))
            {
                var source = pgGroup.Source as Iceberg; //iceberg can do only one bridge to another, multiple to others
                foreach (var bridge in source.Bridges)
                {
                    var edges = bridge.GetEdges();
                    if (edges[1].UniqueId == pgGroup.Destination.UniqueId)
                    {
                        if (withoutBridge <= bridge.Duration * bridge.SpeedMultiplier
                            || withoutBridge <= bridge.Duration)
                        {
                            return (int)(System.Math.Floor(withoutBridge / bridge.SpeedMultiplier));
                        }
                        else
                        {
                            int calcualtedTime = bridge.Duration + (withoutBridge - (int)bridge.SpeedMultiplier * bridge.Duration);
                            return calcualtedTime;
                        }
                    }
                }
            }
            return withoutBridge;
        }


        public static List<(SmartIceberg, int)> SendAmountWithTurnLimit(SmartIceberg dest, List<SmartIceberg> icebergsForUse,
            List<(int, int)> dataToSend)
        {

            var n = new List<(SmartIceberg, int)>();
            foreach (var data in dataToSend)
            {
                int neededAmount = data.Item1;
                int timeToDeliver = data.Item2;
                var actuallyCanSend = new List<(SmartIceberg, int)>();
                var possibleSenders = new List<SmartIceberg>();
                foreach (var iceberg in icebergsForUse)
                {
                    //! to check if upgraded this turn
                    int turnsTillArrival = dest.GetTurnsTillArrival(iceberg);
                    var bridges = iceberg.Bridges.ToList();
                    if (bridges.Count() > 0)
                    {
                        foreach (var b in bridges)
                        {
                            if (dest.pIceberg.Equals(b.GetEdges()[1]))
                            {
                                int durationLeft = b.Duration;
                                if (b.Duration * b.SpeedMultiplier >= turnsTillArrival)
                                {
                                    turnsTillArrival /= (int)b.SpeedMultiplier;
                                }
                                else
                                {
                                    turnsTillArrival = b.Duration + (turnsTillArrival - b.Duration * (int)b.SpeedMultiplier);
                                }
                                break;
                            }
                        }
                    }
                    if (!iceberg.Equals(dest) && turnsTillArrival <= timeToDeliver && iceberg.PreventConqure().Count() == 0)
                    {
                        possibleSenders.Add(iceberg);
                    }
                }
                if (possibleSenders.Count() > 0)
                {
                    int sumSenders = possibleSenders.Sum(x => x.GetUnusedPenguins());
                    if (sumSenders >= neededAmount)
                    {
                        foreach (var ice in possibleSenders)
                        {
                            double ratio = (double)ice.GetUnusedPenguins() / sumSenders;
                            int amountToSend = (int)(ratio * neededAmount) + 1;
                            if (ice.GetUnusedPenguins() < amountToSend)
                            {
                                --amountToSend;
                            }
                            bool safeToSend = ice.PreventConqure(upgrade: false, addition: amountToSend).Count() == 0;
                            while (!safeToSend && amountToSend > 0)
                            {
                                --amountToSend;
                                safeToSend = ice.PreventConqure(upgrade: false, addition: amountToSend).Count() == 0;
                            }
                            if (amountToSend > 0 && ice.CanSendPenguins(dest, amountToSend)
                                && !ice.Upgraded)
                            {
                                //System.Console.WriteLine($"{ice} can send {amountToSend}");
                                actuallyCanSend.Add((ice, amountToSend));
                            }
                        }
                        if (actuallyCanSend.Sum(x => x.Item2) >= neededAmount)
                        {
                            actuallyCanSend.ForEach(x => n.Add(x));
                        }
                    }
                }
            }
            return n;
        }

        public static void ConfigureIcebergs()
        {
            foreach (var myIceberg in GameInfo.Icebergs.myIcebergs)
            {
                if (!GameInfo.Icebergs.threatenedIcebergs.Contains(myIceberg))
                {
                    //TODO: use helpdata list to chech amount availabe for usage
                }
            }
        }

        public static void CalculateMissions()
        {
            foreach (var m in GameInfo.Groups.allMissions)
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
            //System.Console.WriteLine("called");
            var dangered = new List<SmartIceberg>();
            foreach (var myIce in GameInfo.Icebergs.myIcebergs.ToList())
            {
                var helpData = myIce.PreventConqure();
                if (helpData.Count() > 0)
                {
                    //System.Console.WriteLine($"iceberg {myIce} is in danger");
                    dangered.Add(myIce);
                }
                else
                {
                    //System.Console.WriteLine($"no danger for {myIce}");
                }
            }
            return dangered;
        }

    }
}