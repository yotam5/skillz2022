using PenguinGame;
using System.Collections.Generic;
using System.Linq;

namespace MyBot
{
    public class ReinforceIceberg : IMission
    {
        private SmartIceberg iceberg;
        private MissionState state;
        private List<TaskGroup> executionWays;
        public ReinforceIceberg(SmartIceberg iceberg)
        {
            this.iceberg = iceberg;
            this.executionWays = new List<TaskGroup>();
        }

        public void SetMissionState(MissionState state)
        {
            this.state = state;
        }

        public void InitializeExecutionWays()
        {
            this.executionWays = MissionManager.GetExecutionWays(this);
        }

        public int Benefit()
        {
            return this.iceberg.PenguinsPerTurn;
        }

        public SmartIceberg GetTarget()
        {
            return this.iceberg;
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
            return "ReinforceIceberg: " + this.iceberg;
        }

    }
}