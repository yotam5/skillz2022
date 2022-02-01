using System.Collections.Generic;
using System.Text;
using PenguinGame;
using System.Linq;

namespace MyBot
{
    /*
    deals with expansion, neutrals island for now
    */
    public static class Expand
    {
        /// <summary>
        /// return the closest netural taking to account distance and amount
        /// </summary>
        /// <param name="game">game handler</param>
        /// <param name="source_iceberg">source iceberg for comparision</param>
        /// <returns>List<Iceberg></returns>
        public static List<SmartIceberg> GetClosestNeutral(ResourceManager resourceManager, SmartIceberg source_iceberg, double df = 1, double af = 0)
        {
            return resourceManager.GetNeutralIcebergs().OrderByDescending(dest => dest.GetTurnsTillArrival(source_iceberg)).ToList();
        }


        //return the closest neutral iceberg relativly to couple of other,
        //take in account the amount in each iceberg with the factor value
        public static SmartIceberg GetClosestNeutral(ResourceManager resourceManager, SmartIceberg[] icebergs,
            bool freshIceberg = true, double df = 1, double af = 0)
        {
            var neutralIcebergs = Expand.GetFreshNeutralIcebergs(resourceManager);
            if (!freshIceberg)
            {
                neutralIcebergs = resourceManager.GetNeutralIcebergs();
            }
            foreach (var f in neutralIcebergs)
            {
                //System.Console.WriteLine($"fresh is {f}");
            }
            if (neutralIcebergs.Length == 0)
            {
                //System.Console.WriteLine("no neutral icebergs");
                return new SmartIceberg();
            }
            var closestIceberg = neutralIcebergs[0];
            double closestDistance = 9999.0;
            foreach (var neutralIceberg in neutralIcebergs)
            {
                //TODO: fix this
                if (Defensive.GetAttackingGroups(resourceManager, neutralIceberg, false).Count() > 0)
                {
                    //var ff = Defensive.GetAttackingGroups(resourceManager, neutralIceberg, false);

                }
                double distance = 0;
                foreach (var iceberg in icebergs) //NOTE: maybe the closest is dull with penguins
                {
                    int distanceInTurns = iceberg.GetTurnsTillArrival(neutralIceberg);
                    distance += distanceInTurns;
                    distance += (distance * df + iceberg.PenguinAmount * af);
                }
                if (closestDistance > distance)
                {
                    closestDistance = distance;
                    closestIceberg = neutralIceberg;
                }
            }
            return closestIceberg;
        }


        public static SmartIceberg[] GetFreshNeutralIcebergs(ResourceManager resourceManager)
        {
            var freshNeutrals = new List<SmartIceberg>();
            var neutralIcebergs = resourceManager.GetNeutralIcebergs();
            foreach (var iceberg in neutralIcebergs)
            {
                //System.Console.WriteLine($"count of my attacker neutral {Defensive.GetAttackingGroups(resourceManager,iceberg,false,false).Count()}");
                if (Defensive.GetAttackingGroups(resourceManager, iceberg, false, false).Count() == 0)
                {
                    freshNeutrals.Add(iceberg);
                }
            }
            return freshNeutrals.ToArray();
        }


        //TODO: funciton that check if can send to multiple neutrals icebergs bruh what
        public static void ConqureNeutrals(ResourceManager resourceManager)
        {
            var myIcebergs = resourceManager.GetMyIcebergs();
            //TODO: if iceberg about to die to dispatch penguins
            // bool safeA = Expand.SafeToSend(resourceManager,iceberg,0);

            var dest = Expand.GetClosestNeutral(resourceManager, myIcebergs, true, 1, 1.5);


            if (dest._empty)
            {
                System.Console.WriteLine("no neutral icebergs");
                return;
            }
            var data = new List<(SmartIceberg, SmartIceberg, int)>();

            int minimumToTakeOver = dest.PenguinAmount + 1;
            System.Console.WriteLine($"min to take over {minimumToTakeOver}");
            //System.Console.WriteLine($"dest req amount {minimumToTakeOver}, myc amont {myIcebergs.Length}");

            foreach (var p in myIcebergs)
            {
                if (p.upgraded)
                {
                    System.Console.WriteLine("iceberg upgradedcant sent"); continue;
                }
                if(p.CanUpgrade()){
                    p.Upgrade();
                }
                if (minimumToTakeOver > 0)
                {
                    int startingAmount = 0;
                    bool safeToSend = Defensive.RiskEvaluation(resourceManager, p) > 0;
                    if (!safeToSend) //FIX?
                    {
                        continue;
                    }
                                System.Console.WriteLine('D');

                    while (Defensive.RiskEvaluation(resourceManager, p, additionalAmount: -startingAmount) > 0 && startingAmount < minimumToTakeOver)
                    {
                        startingAmount++;
                    }
                                System.Console.WriteLine('E');

                    System.Console.WriteLine($"min achived is {startingAmount}");
                    data.Add((p, dest, startingAmount));
                    minimumToTakeOver -= startingAmount;
                }
            }
            if (minimumToTakeOver <= 0)
            {
                foreach (var c in data)
                {
                    System.Console.WriteLine($"c is {c.Item2.Id}");
                    System.Console.WriteLine($"iceberg sending {c.Item1.Id}");
                    c.Item1.SendPenguins(c.Item2, c.Item3);
                }
            }
        }

    }
}