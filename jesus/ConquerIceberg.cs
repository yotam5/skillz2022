using PenguinGame;
using System.Collections.Generic;
using System.Linq;

namespace MyBot
{
    public class ConquerIceberg : IMission
    {
        private SmartIceberg iceberg;
        private List<TaskGroup> executionWays;
        private MissionState state;
        public ConquerIceberg(SmartIceberg iceberg)
        {
            this.iceberg = iceberg;
            this.executionWays = new List<TaskGroup>();
        }

        public void InitializeExecutionWays()
        {
            this.executionWays = MissionManager.GetExecutionWays(this);
        }

        public int Benefit()
        {
            return this.iceberg.PenguinsPerTurn * GameInfo.Game.turn;
        }

        public SmartIceberg GetTarget()
        {
            return this.iceberg;
        }

        public void SetMissionState(MissionState state)
        {
            this.state = state;
        }

        public void CalculateExecutionWays()
        {
            this.executionWays = MissionManager.GetExecutionWays(this);
        }

        public List<TaskGroup> GetExecutionWays()
        {
            return this.executionWays;
        }

        public string GetDescription()
        {
            return "ConqurerIceberg: " + this.iceberg;
        }

        public override string ToString()
        {
            return this.GetDescription();
        }
    }
}