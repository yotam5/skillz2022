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


        /// <summary>
        /// update the missions based on the timer and remove when it comes down to zero 
        /// </summary>
        public static void UpdateActiveMissions()
        {
            var toRemove = new List<IMission>();

            foreach (var onGoing in ActiveMissions)
            {
                onGoing.TimerDown();
                if(onGoing.GetTimer() <= 0){
                    toRemove.Add(onGoing);
                    continue;
                }
                if (onGoing.GetType() == typeof(ReinforceIceberg))
                {
                    onGoing.CalculateExecutionWays();
                    if (onGoing.CanBePerformed())
                    {
                        var tmp = onGoing.GetExecutionWays();
                        foreach (var recoverMode in tmp)
                        {
                            if (!recoverMode.CanBePerformed()) { continue; }
                            recoverMode.GetTasks().ToList().ForEach(x => x.Performe());
                            break;
                        }
                    }
                }
            }
            toRemove.ForEach(x => ActiveMissions.Remove(x));
        }

        public static HashSet<IMission> DistributionTasksForIcebergs()
        {
            var amo = new HashSet<IMission>();
            var allMissions = GameInfo.Groups.allMissions;
            //System.Console.WriteLine("All missions:");
            //allMissions.ToList().ForEach(x => System.Console.WriteLine(x));
            var CanBePerformed = (from mission in allMissions
                                  where mission.CanBePerformed()
                                  && !isActive(mission)
                                  select mission).ToList();

            CanBePerformed = CanBePerformed.OrderBy(x => x.Benefit()).Reverse().ToList();
            //System.Console.WriteLine("can be performed");
            //CanBePerformed.ForEach(x => System.Console.WriteLine(x));
            foreach (var m in CanBePerformed)
            {
                if (m.CanBePerformed() && m.GetExecutionWays().ToList().Count() > 0)
                {
                    amo.Add(m);
                }
            }
            //System.Console.WriteLine("----------------------");
            //CanBePerformed.ToList().ForEach(x => System.Console.WriteLine(x));
            System.Console.WriteLine("before loop1");
            foreach (var m in amo)
            {
                //System.Console.WriteLine(m);
                //System.Console.WriteLine($"w is {w++}");
                var execs = m.GetExecutionWays().ToList();
                var canBePerformedWays = (from t in execs where t.CanBePerformed() select t).ToList();
                //System.Console.WriteLine(canBePerformedWays.Count());
                if (canBePerformedWays.Count() > 0)
                {
                    var choosenWay = canBePerformedWays.First();
                    var tmp = choosenWay.GetTasks().ToList();
                    if (tmp.Count() == 0)
                    {
                        //System.Console.WriteLine("bruh");
                        continue;
                    }
                    //System.Console.WriteLine("*************************");
                   // canBePerformedWays.ForEach(x => System.Console.WriteLine(x));
                    tmp = tmp.OrderBy(x => x.TaskTurns()).ToList();
                    m.SetTimer(tmp.Last().TaskTurns());
                    choosenWay.GetTasks().ToList().ForEach(x => x.Performe());
                    if(m.GetType() != typeof(ReinforceIceberg)){
                    ActiveMissions.Add(m);
                    }
                }
            }
            System.Console.WriteLine("after loop1");
            return amo;
        }

        public static List<IMission> AllMissions()
        {
            var missions = new List<IMission>();
            foreach (var iceberg in GameInfo.Icebergs.allIcebergs)
            {
                if (!iceberg.Owner.Id.Equals(GameInfo.Players.mySelf.Id))
                {
                    missions.Add(new ConquerIceberg(iceberg));
                    //System.Console.WriteLine("added conq: " + iceberg);
                    if (iceberg.Owner.Id == -1)
                    {
                        //missions.Add(new ReinforceIceberg(iceberg));
                    }
                }
                else
                {
                    if (GameInfo.Icebergs.threatenedIcebergs.Contains(iceberg))
                    {
                        //System.Console.WriteLine($"iceberg {iceberg} is threatend");
                        missions.Add(new ReinforceIceberg(iceberg));
                        continue;
                    }
                    if (iceberg.CanUpgrade() &&             //! need to add send for upgrade
                        iceberg.PreventConqure(upgrade: true).Count() == 0)
                    {
                        //System.Console.WriteLine($"UPGRADING: {iceberg}");
                        missions.Add(new UpgradeIceberg(iceberg));
                    }
                }
            }
            //missions.ForEach(x => System.Console.WriteLine(x));
            missions.RemoveAll(x => isActive(x));
            return missions;
        }


        public static bool isActive(IMission mission)
        {
            foreach (var activeMission in ActiveMissions)
            {
                if (activeMission.GetDescription().Equals(mission.GetDescription()))
                {
                    //System.Console.WriteLine($"active: {activeMission}");
                    //System.Console.WriteLine($"mission: {activeMission}");

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
                //TODO: all icebergs combinations?
                var tmp = ConqueringWays(GameInfo.Icebergs.myIcebergs, new ConquerIceberg(mission.GetTarget()));
                executionWays.Add(tmp);
            }
            else if (missionType == typeof(ReinforceIceberg))
            {
                var tmp = ReinforcingWays(GameInfo.Icebergs.myIcebergs, new ReinforceIceberg(mission.GetTarget()));
                executionWays.Add(tmp);
                //System.Console.WriteLine("added reinforce iceberg");
                //tmp.GetTasks().ToList().ForEach(x => System.Console.WriteLine(x));
                //System.Console.WriteLine("end");
            }
            else if (missionType == typeof(UpgradeIceberg))
            {
                if (mission.GetTarget().CanUpgrade())
                {
                    var tmp = new TaskGroup();
                    tmp.AddTask((new Upgrade(mission.GetTarget())));
                    executionWays.Add(tmp);
                    //System.Console.WriteLine("added upgrade iceberg: " + mission.GetTarget());
                }
            }

            return executionWays;
        }

    

        public static TaskGroup ReinforcingWays(List<SmartIceberg> reinforcers, ReinforceIceberg iceToReinforce)
        {
            var result = new TaskGroup();
            var target = iceToReinforce.GetTarget();
            var data = target.PreventConqure(b: false);
            //System.Console.WriteLine("reinforce " + target);
            //data.ForEach(x => System.Console.WriteLine(x.Item1 + " " + x.Item2));
            //System.Console.WriteLine($"REINFORCEWAYS {target}");
            // /System.Console.WriteLine("data");
            //data.ForEach(x => System.Console.WriteLine($"{x.Item1}-{x.Item2}"));
            var iceAndAmounts = Utils.SendAmountWithTurnLimit(target,
            GameInfo.Icebergs.myIcebergs.ToList(),
            data
            );
            //System.Console.WriteLine("reinforce ways counter is " + iceAndAmounts.Count());
            //iceAndAmounts.ForEach(x => System.Console.WriteLine(x));
            if (iceAndAmounts.Count() > 0)
            {
                foreach (var tuple in iceAndAmounts)
                {
                    result.AddTask(new Reinforce(tuple.Item1, target, tuple.Item2));
                }
            }
            else if (data.Count() > 0 && iceAndAmounts.Count() == 0)
            {
                var d = data.First();
                foreach (var iceberg in GameInfo.Icebergs.myIcebergs.ToList().OrderBy(x =>x.PenguinAmount).ToList())
                {   
                    bool CanCreateBridge = iceberg.CanCreateBridge(target);
                    bool reachable = iceberg.GetTurnsTillArrivalWithBridge(target) <= d.Item2;
                    bool probable  = iceberg.PenguinAmount - GameInfo.Bridge.bridgeCost + iceberg.PenguinsPerTurn
                        >= d.Item1;
                    if (CanCreateBridge && reachable && probable 
                        && iceberg.PreventConqure(addition: d.Item1).Count() == 0)
                    {
                        result.AddTask(new CreateBridge(iceberg, target));
                        //System.Console.WriteLine($"it worked source {iceberg}");
                        break;
                    }
                }
            }
            return result;
        }


        public static TaskGroup ConqueringWays(List<SmartIceberg> attackers, ConquerIceberg iceToTakeOver)
        {
            TaskGroup result = new TaskGroup();
            //int turnLimit = iceToTakeOver.GetTarget().Remotest(attackers).GetTurnsTillArrival(iceToTakeOver.GetTarget());
            //System.Console.WriteLine("worst case to take over in " + turnLimit + " turns " + iceToTakeOver.GetTarget().Iceberg + " is " + requiredPenguinAmount);
            for (int i = 0; i < 20; i++)
            {
                int requiredPenguinAmount = iceToTakeOver.GetTarget().PotentialBackup(i, GameInfo.Players.enemyPlayer.Id) + 1;
                var k = new List<(int, int)>();
                k.Add((requiredPenguinAmount, i));
                var tmp = Utils.SendAmountWithTurnLimit(
                    iceToTakeOver.GetTarget(),
                    GameInfo.Icebergs.myIcebergs.ToList(),
                    k
                );
                if (tmp.Count() > 0)
                {
                    foreach (var data in tmp)
                    {
                        var task = new Attack(data.Item1, iceToTakeOver.GetTarget(), data.Item2);
                        result.AddTask(task);
                    }
                    break;
                }
            }
            return result;
        }

        public static double AccumulatedBenefit(List<IMission> missionToAccumulated)
        {
            return missionToAccumulated.Sum(x => x.Benefit());
        }


    }
}