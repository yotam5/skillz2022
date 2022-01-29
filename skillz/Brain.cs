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


            if (resourceManager.Turn < 12)
            {
                Expand.ConqureNeutrals(resourceManager);
            }
            else
            {
                foreach (var iceberg in resourceManager.GetMyIcebergs())
                {
                    if (Upgrade.SafeToUpgradeSimple(resourceManager, iceberg))
                    {
                        iceberg.Upgrade();
                    }
                }
                Expand.ConqureNeutrals(resourceManager);
            }
        }
    }

}