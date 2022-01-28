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

            //TODO: send to multiple neutrals icebergs
            if (game.Turn == 1)  
            {
                var nearestNeutral = Expand.GetClosestNeutral(game, myIcebergs[0])[0];
                int amountToSend = nearestNeutral.PenguinAmount;
                if(Expand.SafeToSend(game,myIcebergs[0],amountToSend+1))
                {
                    myIcebergs[0].SendPenguins(nearestNeutral, amountToSend + 1);

                }
                else if(Upgrade.SafeToUpgradeSimple(game,myIcebergs[0])){
                    myIcebergs[0].Upgrade();
                }
            }

            if (myIcebergs.Count() >= 2  )
            {
                foreach (var iceberg in myIcebergs)
                {
                    if(Upgrade.SafeToUpgradeSimple(game,iceberg)){
                        iceberg.Upgrade();
                    }
                }
            }


        }
    }

}