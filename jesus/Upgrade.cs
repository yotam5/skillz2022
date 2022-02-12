using PenguinGame;
using System.Collections.Generic;
using System.Linq;

namespace MyBot
{
    public class Upgrade : ITask
    {
        private SmartIceberg iceberg;

        public Upgrade(SmartIceberg iceberg)
        {
            this.iceberg = iceberg;
        }

        public SmartIceberg GetTarget()
        {
            return this.iceberg;
        }

        public SmartIceberg GetActor()
        {
            return this.iceberg;
        }

        public void Performe()
        {
            this.iceberg.Upgrade();
        }

        public int Loss()
        {
            return this.iceberg.UpgradeCost/this.iceberg.PenguinAmount;
        }

        public int PenguinsRequired()
        {  
            return this.iceberg.UpgradeCost;
        }
    }
}