using System.Collections.Generic;
using System.Text;
using PenguinGame;
using System.Linq;

namespace MyBot
{   
    /*
    deals with expansion, neutrals island for now
    */
    public static class Upgrade
    {   
        //check if island can be upgraded in the condition that 
        //it wont be conquered in the given call,
        //need to check also if there are near enemy islands
        public static bool SafeToUpgradeSimple(Game game, Iceberg MyIceberg)
        {
            if(MyIceberg.CanUpgrade())
            {
                var AttackingGroups = Offensive.GetAttackingGroups(game, MyIceberg);
                int TotalAttackersCount = AttackingGroups.Sum(group=>group.PenguinAmount);
                int UpgradeCost = MyIceberg.UpgradeCost;
                int TotalInMyIsland = MyIceberg.PenguinAmount;
                int AmontAfterUpgrading = TotalInMyIsland = UpgradeCost;
                int IcebergGenerationAfterUpgrade = MyIceberg.PenguinsPerTurn + MyIceberg.UpgradeValue;
                bool CanBeUpgraded = MyIceberg.CanUpgrade();
                
                if(AmontAfterUpgrading > TotalAttackersCount){

                }
            }
            return false;
        }
    }
}