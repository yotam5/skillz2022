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
        public static List<Iceberg> GetClosestNeutral(Game game, Iceberg source_iceberg, double df=1, double af=0)
        {
            return game.GetNeutralIcebergs().OrderBy(dest => dest.GetTurnsTillArrival(source_iceberg)).ToList();
        }


        //return the closest neutral iceberg relativly to couple of other,
        //take in account the amount in each iceberg with the factor value
        public static Iceberg GetClosestNeutral(Game game, List<Iceberg> icebergs,
            bool newIceberg = true, double df = 1, double af = 0)
        {
            var neutralIcebergs = game.GetNeutralIcebergs();
            Iceberg closestIceberg = icebergs[0];
            double closestDistance = 9999.0;
            foreach (var neutralIceberg in neutralIcebergs)
            {
                //TODO: fix this
                if (Offensive.GetAttackingGroups(game, neutralIceberg, false).Count() > 0)
                {
                    continue;
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



        //TODO: need to make it more complex, what if enemy iceberg are nearby
        /// <summary>
        /// check if its safe to send a specific amount of penguins at a given turn
        /// </summary>
        /// <param name="game">game handler</param>
        /// <param name="source">source to send from</param>
        /// <param name="amountToSend">amount of penguins to send</param>
        /// <returns>boolean value</returns>
        public static bool SafeToSend(Game game, Iceberg source, int amountToSend)
        {
            if (amountToSend >= source.PenguinAmount)
            {
                return false;
            }
            var AttackingGroups = Offensive.GetAttackingGroups(game, source, true);
            int GenerationRate = source.PenguinsPerTurn;

            int MyIcebergCounter = source.PenguinAmount - amountToSend;

            foreach (var attackingGroup in AttackingGroups)
            {
                MyIcebergCounter += source.PenguinsPerTurn * attackingGroup.TurnsTillArrival;
                MyIcebergCounter -= attackingGroup.PenguinAmount;
                if (MyIcebergCounter <= 0)
                { //NOTE: neutral maybe good dependse
                    return false;
                }

            }
            return true;
        }

        //TODO: funciton that check if can send to multiple neutrals icebergs
        public static void ConqureNeutrals(Game game)
        {
            var myIcebergs = ();
            var neutrals = game.GetNeutralIcebergs();

            //TODO: if iceberg about to die to dispatch penguins
            // bool safeA = Expand.SafeToSend(game,iceberg,0);

            var dest = Expand.GetClosestNeutral(game, myIcebergs.ToList(), true, 1, 1.5);

            var data = new List<(Iceberg, Iceberg, int)>();

            int rquired_amount = dest.PenguinAmount + 1;
            System.Console.WriteLine($"dest req amount {rquired_amount}, myc amont {myIcebergs.Length}");
            int n = 0;

            foreach (var p in myIcebergs)
            {
                if (rquired_amount > 0)
                {
                    int startingAmount = 0;
                    bool safeToSend = Expand.SafeToSend(game, p, 0);
                    if (!safeToSend)
                    {
                        continue;
                    }
                    while (startingAmount < p.PenguinAmount && Expand.SafeToSend(game, p, startingAmount))
                    {
                        startingAmount += 3;
                        System.Console.WriteLine(n++);
                    }
                    startingAmount -= 3;
                    data.Add((p, dest, startingAmount));
                    rquired_amount -= startingAmount;
                }
            }
            if (rquired_amount < 0)
            {
                foreach (var c in data)
                {
                    c.Item1.SendPenguins(c.Item2, c.Item3);
                }
            }
        }

    }
}