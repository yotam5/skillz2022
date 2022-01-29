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
        public static void execute(Game game,ResourceManager resourceManager)
        {
            System.Console.WriteLine($"Game turn is {game.Turn}");


            if (game.Turn < 12)
            {
                Expand.ConqureNeutrals(game);
            }
            else
            {
                foreach (var iceberg in resourceManager.GetMyIcebergsArray())
                {
                    if (Upgrade.SafeToUpgradeSimple(game, iceberg))
                    {
                        iceberg.Upgrade();
                    }
                }
                Expand.ConqureNeutrals(game);
            }
        }
    }

}