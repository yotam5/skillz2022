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
                var AttackingGroups = Offensive.GetAttackingGroups(game, MyIceberg,true);
                int TotalAttackersCount = AttackingGroups.Sum(group=>group.PenguinAmount);
                int UpgradeCost = MyIceberg.UpgradeCost;
                int TotalInMyIsland = MyIceberg.PenguinAmount;
                int AmountAfterUpgrading = TotalInMyIsland - UpgradeCost;
                int IcebergGenerationAfterUpgrade = MyIceberg.PenguinsPerTurn + MyIceberg.UpgradeValue;
                bool CanBeUpgraded = MyIceberg.CanUpgrade();
                System.Console.WriteLine($"total in my island {TotalInMyIsland}");
                System.Console.WriteLine($"total in my island after upgrade {AmountAfterUpgrading}");
                System.Console.WriteLine($"total enemy to island {TotalAttackersCount}");
                System.Console.WriteLine($"gen1 {MyIceberg.PenguinsPerTurn} gen2 {MyIceberg.UpgradeValue}");
                int MyIcebergPenguinAmount = AmountAfterUpgrading;
                foreach(var attackinGroup in AttackingGroups){
                    int TurnsUntilArrival = attackinGroup.TurnsTillArrival;
                    MyIcebergPenguinAmount += TurnsUntilArrival * IcebergGenerationAfterUpgrade;
                    MyIcebergPenguinAmount -= attackinGroup.PenguinAmount;
                    if(MyIcebergPenguinAmount <= 0){
                        return false;
                    }
                }
                return true;

            }
            return false;
        }
    }
}