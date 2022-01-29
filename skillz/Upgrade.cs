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
        
        /// <summary>
        /// check if its safe to upgrade iceberg at a given turn
        /// </summary>
        /// <param name="game">game handler</param>
        /// <param name="MyIceberg">iceberg to upgrade</param>
        /// <returns>boolean value</returns>
        public static bool SafeToUpgradeSimple(ResourceManager resourceManager, SmartIceberg MyIceberg) //NOTE: need to check if there are enemies nearby
        {
            if(MyIceberg.CanUpgrade())
            {
                var AttackingGroups = Offensive.GetAttackingGroups(resourceManager, MyIceberg);
                int TotalAttackersCount = AttackingGroups.Sum(group=>group.PenguinAmount);
                int UpgradeCost = MyIceberg.UpgradeCost;
                int TotalInMyIceberg = MyIceberg.PenguinAmount;
                int AmountAfterUpgrading = TotalInMyIceberg - UpgradeCost;
                int IcebergGenerationAfterUpgrade = MyIceberg.PenguinsPerTurn + MyIceberg.UpgradeValue;
                bool CanBeUpgraded = MyIceberg.CanUpgrade();

                //System.Console.WriteLine($"total in my island {TotalInMyIceberg}");
                //System.Console.WriteLine($"total in my island after upgrade {AmountAfterUpgrading}");
                //System.Console.WriteLine($"total enemy to island {TotalAttackersCount}");
                //System.Console.WriteLine($"gen1 {MyIceberg.PenguinsPerTurn} gen2 {MyIceberg.UpgradeValue}");
                
                int MyIcebergPenguinAmount = AmountAfterUpgrading;

                //loop on each attacking group and check if it will
                //conquere the iceberg when arriving

                foreach(var attackinGroup in AttackingGroups){
                    int TurnsUntilArrival = attackinGroup.TurnsTillArrival;
                    MyIcebergPenguinAmount += TurnsUntilArrival * IcebergGenerationAfterUpgrade;
                    MyIcebergPenguinAmount -= attackinGroup.PenguinAmount;
                    if(MyIcebergPenguinAmount <= 0){ //if the iceberg will be conqured
                        return false;
                    }
                }
                return true;

            }
            return false;
        }
    }
}