using PenguinGame;
using System.Collections.Generic;
using System.Linq;

namespace MyBot
{
    public class UpgradeIceberg : IMission
    {
        private SmartIceberg iceberg;
        private List<TaskGroup> executionWays;
        private MissionState state;

        public UpgradeIceberg(SmartIceberg iceberg)
        {
            this.iceberg = iceberg;
            this.executionWays = new List<TaskGroup>();
        }

        public void CalculateExecutionWays()
        {
            this.executionWays = MissionManager.GetExecutionWays(this);
        }

        public List<TaskGroup> GetExecutionWays()
        {
            return this.executionWays;
        }

        public int Benefit()
        {
            return GameInfo.Game.turnsLeft * (this.iceberg.PenguinsPerTurn + 1);
        }

        public SmartIceberg GetActor()
        {
            return this.iceberg;
        }

        public SmartIceberg GetTarget()
        {
            return this.iceberg;
        }

        public void SetMissionState(MissionState state)
        {
            this.state = state; 
        }

        public string GetDescription()
        {
            return "upgradeIceberg: " + this.iceberg;
        }

        public override string ToString()
        {
            return this.GetDescription();
        }

    }
}