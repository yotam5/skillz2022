using PenguinGame;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace MyBot
{
    public static class GameLogic
    {
        public static void execute(Game game)
        {

            if (game.Turn == 1)
                game.GetMyIcebergs()[0].Upgrade();
            else if (game.Turn == 7)
            {
                game.GetMyIcebergs()[0].SendPenguins(game.GetNeutralIcebergs()[0], 11);
            }
            else if (game.Turn == 12)
            {
                game.GetMyIcebergs()[0].SendPenguins(game.GetNeutralIcebergs()[1], 11);
            }
            else if (game.Turn == 19)
            {
                foreach (var nc in game.GetNeutralIcebergs())
                {
                    if (nc.Id == 7)
                    {
                        game.GetMyIcebergs()[0].SendPenguins(nc, 13);
                        break;
                    }
                }
            }
            else if (game.Turn == 22)
            {
                foreach (var nc in game.GetNeutralIcebergs())
                {
                    if (nc.Id == 7)
                    {
                        game.GetMyIcebergs()[1].SendPenguins(nc, 5);
                        break;
                    }
                }
            }
            else if (game.Turn > 22)
            {
                var wallIce = Defensive.GetWall(game);
                foreach (var myIce in game.GetMyIcebergs())
                {
                    if (!myIce.Equals(wallIce))
                    {
                        if (!myIce.AlreadyActed && myIce.CanSendPenguins(wallIce, wallIce.PenguinsPerTurn) && 
                           Utils.HelpIcebergData(game,myIce,wallIce.PenguinsPerTurn).Count() == 0)
                        {
                           myIce.SendPenguins(wallIce, wallIce.PenguinsPerTurn);
                        }
                    }
                }
            }
        }
    }
}
    
        