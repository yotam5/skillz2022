using PenguinGame;
using System.Collections.Generic;
using System.Linq;

namespace MyBot
{
    public class ReinforceIceberg : IMission
    {
        private int timer;
        private SmartIceberg iceberg;
        private MissionState state;
        private List<TaskGroup> executionWays;
        public ReinforceIceberg(SmartIceberg iceberg)
        {
            this.iceberg = iceberg;
            this.executionWays = new List<TaskGroup>();
            this.state = MissionState.INITIALIZED;

        }
        public MissionState GetMissionState(){return this.state;}

        public int GetTimer()
        {
            return this.timer;
        }
        public void TimerUp()
        {
            ++this.timer;
        }
        public bool CanBePerformed()
        {
            foreach(var task in this.GetExecutionWays()) //!need to initialize
            {
                if(task.CanBePerformed() && this.GetExecutionWays().Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public void TimerDown()
        {
            --this.timer;
        }

        public void SetTimer(int value)
        {
            this.timer = value;
        }

        public void SetMissionState(MissionState state)
        {
            this.state = state;
        }

        public void InitializeExecutionWays()
        {
            this.executionWays = MissionManager.GetExecutionWays(this);
        }

        public double Benefit()
        {
            return this.iceberg.PenguinsPerTurn * 980000;
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
            return "ReinforceIceberg: " + this.iceberg.UniqueId + " " + this.iceberg.Id;
        }

        public override string ToString()
        {
            return this.GetDescription();
        }

    }
}