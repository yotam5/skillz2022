using PenguinGame;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace MyBot
{
    public static class GameInfo
    {
        //!need to add attacked icebergs
        private static Dictionary<int,bool> upgradedThisTurn = new Dictionary<int, bool>();

        public static void InitializeUpgradeDict(Game game)
        {
            foreach(var myIce in game.GetAllIcebergs())
            {
                upgradedThisTurn.Add(myIce.UniqueId,false);
            }
        }

        public static void UpdateUpgradeDict(int uniqueId)
        {
            upgradedThisTurn[uniqueId] = true;
        }

        public static bool UpgradedThisTurn(int uniqueId)
        {
            return upgradedThisTurn[uniqueId];
        }

        public static void EndTurn(Game game)
        {
            foreach(var iceberg in game.GetAllIcebergs())
            {
                upgradedThisTurn[iceberg.UniqueId] = false;
            }
        }
    }
}