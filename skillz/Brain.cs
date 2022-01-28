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
        public static void execute(Game game)
        {
            Iceberg[] myIcebergs = game.GetMyIcebergs();
            Iceberg[] neutralsIcbergs = game.GetNeutralIcebergs();
            Iceberg[] enemyIcbergs = game.GetEnemyIcebergs();
            PenguinGroup[] enemyPenguinsGroups = game.GetEnemyPenguinGroups();
            PenguinGroup[] myPenguinsGroup = game.GetMyPenguinGroups();

            System.Console.WriteLine($"Game turn is {game.Turn}");


            if (game.Turn < 12)
            {
                Expand.ConqureNeutrals(game);
            }
            else
            {
                foreach (var iceberg in myIcebergs)
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