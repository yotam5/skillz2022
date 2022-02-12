using PenguinGame;
using System.Collections.Generic;
using System.Linq;

namespace MyBot
{
    public class MissionManager
    {
        private static HashSet<IMission> ActiveMissions = new HashSet<IMission>();
        public MissionManager()
        {
        }

        /*public static HashSet<ITask> DistributionTasksForIcebergs()
        {
            var amo = new List<HashSet<IMission>>();    

            return amo.First; 
        }*/

        public static HashSet<IMission> AllMissions()
        {
            var missions = new HashSet<IMission>();
            foreach (var iceberg in GameInfo.Icebergs.allIcebergs)
            {
                if (!iceberg.Owner.Id.Equals(GameInfo.Players.mySelf.Id))
                {
                    missions.Add(new ConquerIceberg(iceberg));
                }
                else
                {
                    if (GameInfo.Icebergs.threatenedIcebergs.Contains(iceberg))
                    {
                        missions.Add(new ReinforceIceberg(iceberg));
                        continue;
                    }
                    if (iceberg.CanUpgrade() &&
                        iceberg.FutureState(upgrade: true).Count() == 0)
                    {
                        missions.Add(new UpgradeIceberg(iceberg));
                    }
                }
            }
            missions.RemoveWhere(isActive);
            return missions;
        }

        public static bool isActive(IMission mission)
        {
            foreach(var activeMission in ActiveMissions)
            {
                if(activeMission.GetDescription().Equals(mission.GetDescription()))
                {
                    return true;
                }
            }
            return false;
        }

        public static List<TaskGroup> GetExecutionWays(IMission mission)
        {
            var executionWays = new List<TaskGroup>();
            var missionType = mission.GetType();
            if (missionType == typeof(ConquerIceberg))
            {
                var tmp = ConqueringWays(GameInfo.Icebergs.myIcebergs, new ConquerIceberg(mission.GetTarget()));
                executionWays.Add(tmp);
            }
            else if (missionType == typeof(ReinforceIceberg))
            {
                //executionWays.Add()
            }
            else if (missionType == typeof(UpgradeIceberg))
            {
                if (mission.GetTarget().CanUpgrade())
                {
                    var tmp = new TaskGroup();
                    tmp.AddTask((new Upgrade(mission.GetTarget())));
                    executionWays.Add(tmp);
                }
            }

            return executionWays;
        }



        /*public static HashSet<ITask> CreateTasksForIcebergs()
        {
            var allMission = GameInfo.Groups.allMissions;
            if()
        }*/

        public static TaskGroup ReinforcingWays(List<SmartIceberg> reinforcers, ReinforceIceberg iceToReinforce)
        {
            var result = new TaskGroup();
            int requiredPenguinAmount = iceToReinforce.GetTarget().minPenguinsToTakeOver(
                    iceToReinforce.GetTarget().Remotest(reinforcers).GetTurnsTillArrival(iceToReinforce.GetTarget()));

            double available = 0;
            foreach (var myIceberg in reinforcers)
            {
                int delta = myIceberg.GetUnusedPenguins() - myIceberg.GetIncomingPenguinsFromIceberg(iceToReinforce.GetTarget());
                if (delta <= 0)
                {
                    return result;
                }
                available += delta;
            }
            if (available > requiredPenguinAmount)
            {
                foreach (var myIceberg in reinforcers)
                {
                    int realFreePenguins = myIceberg.GetUnusedPenguins() - myIceberg.GetIncomingPenguinsFromIceberg(iceToReinforce.GetTarget());
                    int toSend = (int)System.Math.Round((realFreePenguins / available) * requiredPenguinAmount);
                    var tmp = new Reinforce(myIceberg, iceToReinforce.GetTarget(), toSend);
                    result.AddTask(tmp);
                }
            }
            return result;
        }

        public static TaskGroup ConqueringWays(List<SmartIceberg> attackers, ConquerIceberg iceToTakeOver)
        {
            TaskGroup result = new TaskGroup();
            int turnLimit = iceToTakeOver.GetTarget().Remotest(attackers).GetTurnsTillArrival(iceToTakeOver.GetTarget());
            int requiredPenguinAmount = iceToTakeOver.GetTarget().minPenguinsToTakeOver(turnLimit);
            System.Console.WriteLine("min to take over " + iceToTakeOver.GetTarget().Iceberg + " is " + requiredPenguinAmount);
            double availablePenguins = 0;
            int actuallySent = 0;
            int max = 0;
            foreach (var attacker in attackers)
            {
                int realFreePenguins = attacker.GetUnusedPenguins() - attacker.GetIncomingPenguinsFromIceberg(iceToTakeOver.GetTarget());
                if (realFreePenguins <= 0)
                {
                    return result;
                }
                availablePenguins += realFreePenguins;
            }
            foreach (var attacker in attackers)
            {
                int realFreePenguins = attacker.GetUnusedPenguins() - attacker.GetIncomingPenguinsFromIceberg(iceToTakeOver.GetTarget());
                actuallySent += (int)(System.Math.Round((realFreePenguins / availablePenguins) * requiredPenguinAmount));
                if (realFreePenguins > max)
                {
                    max = realFreePenguins;
                }
            }
            System.Console.WriteLine("actuallysent " + actuallySent);
            if (availablePenguins > requiredPenguinAmount)
            {
                foreach (var attacker in attackers)
                {
                    int realFreePenguins = attacker.GetUnusedPenguins() - attacker.GetIncomingPenguinsFromIceberg(iceToTakeOver.GetTarget());
                    System.Console.WriteLine("free " + realFreePenguins);
                    int round = (int)(System.Math.Round((realFreePenguins / availablePenguins) * requiredPenguinAmount));
                    System.Console.WriteLine("round " + round);
                    if (max == realFreePenguins && actuallySent + 1 == requiredPenguinAmount && round + 1 <= realFreePenguins)
                    {
                        ++round;
                        ++actuallySent;
                    }
                    result.AddTask(new Attack(attacker, iceToTakeOver.GetTarget(), round));
                }
            }
            return result;
        }

        public static int AccumulatedBenefit(List<IMission> missionToAccumulated)
        {
            return missionToAccumulated.Sum(x => x.Benefit());
        }


    }
}