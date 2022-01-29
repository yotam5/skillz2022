using System.Collections.Generic;
using System.Text;
using PenguinGame;
using System.Linq;

namespace MyBot
{
    /*
    deals with moves decisons
    */
    public static class Brain
    {

        /// <summary>
        /// execute brain
        /// </summary>
        /// <param name="game"></param>
        public static void execute(ResourceManager resourceManager)
        {
            System.Console.WriteLine($"Game turn is {resourceManager.Turn}");


            if (resourceManager.Turn == 1)
            {
                resourceManager.GetMyIcebergs()[0].Upgrade();
            }
            else if (resourceManager.Turn < 20)
            {
                Expand.ConqureNeutrals(resourceManager);
            }
            else if (resourceManager.Turn < 32)
            {
                foreach (var c in resourceManager.GetMyIcebergs())
                {
                    if (Upgrade.SafeToUpgradeSimple(resourceManager, c))
                    {
                        c.Upgrade();
                    }
                }
            }
            else
            {
                Defensive.DefenseMehcanisem(resourceManager);
            }

        }
    }
}