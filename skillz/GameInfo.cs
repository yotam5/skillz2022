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
        private static Dictionary<int,bool> attackedIcebergsByUs = new Dictionary<int, bool>();
        
        public static void InitializeAttckedEnemyIcebergs(Game game)
        {
            foreach(var ice in game.GetEnemyIcebergs())
            {
                attackedIcebergsByUs.Add(ice.UniqueId,false);
            }
            foreach(var ice in game.GetNeutralIcebergs())
            {
                attackedIcebergsByUs.Add(ice.UniqueId,false);
            }
        }
        
        public static void UpdateAttackedIcebergsByUs(Iceberg enemyIceberg,bool state)
        {
            attackedIcebergsByUs[enemyIceberg.UniqueId] = state;
        }

        public static bool IsAttackedByUs(Iceberg enemyIceberg)
        {
            return attackedIcebergsByUs[enemyIceberg.UniqueId];
        }

        public static void UpdateAttackedByus(Game game)
        {
            foreach(var myIce in game.GetMyIcebergs())
            {
                attackedIcebergsByUs[myIce.UniqueId] = false;
            }
        }

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