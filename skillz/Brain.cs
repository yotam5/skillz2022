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
        public static void execute(Game game)
        {
            Iceberg[] myIcebergs = game.GetMyIcebergs();
            Iceberg[] neutralsIcbergs = game.GetNeutralIcebergs();
            Iceberg[] enemyIcbergs = game.GetEnemyIcebergs();
            PenguinGroup[] enemyPenguinsGroups = game.GetEnemyPenguinGroups();
            PenguinGroup[] myPenguinsGroup = game.GetMyPenguinGroups();

            if (game.Turn == 1)
            {
                var nearestNeutral = Expand.GetClosestNeutral(game, myIcebergs[0])[0];
                int amountToSend = nearestNeutral.PenguinAmount;
                if (myIcebergs[0].PenguinAmount > amountToSend + 1)
                {
                    myIcebergs[0].SendPenguins(nearestNeutral, amountToSend + 1);
                }
                return;
            }

            if (myIcebergs.Count() >= 2  )
            {
                foreach (var k in myIcebergs)
                {
                    if(Upgrade.SafeToUpgradeSimple(game,k)){
                        k.Upgrade();
                    }
                }
            }


        }
    }

}