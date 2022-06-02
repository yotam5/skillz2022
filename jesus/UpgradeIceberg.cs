using PenguinGame;
using System.Collections.Generic;
using System.Linq;

namespace MyBot
{
    public class UpgradeIceberg : IMission
    {
        private int timer;
        private SmartIceberg iceberg;
        private List<TaskGroup> executionWays;
        private MissionState state;

        public UpgradeIceberg(SmartIceberg iceberg)
        {
            this.iceberg = iceberg;
            this.executionWays = new List<TaskGroup>();
            this.state = MissionState.INITIALIZED;
            this.timer = 1; //!note!
        }

        public int GetTimer()
        {
            return this.timer;
        }

        public void TimerUp()
        {
            ++this.timer;
        }

        public void TimerDown()
        {
            --this.timer;
        }

        public void SetTimer(int value)
        {
            this.timer = value;
        }

        public void CalculateExecutionWays()
        {
            this.executionWays = MissionManager.GetExecutionWays(this);
            this.state = MissionState.INITIALIZED;
        }
        public MissionState GetMissionState(){return this.state;}

        public List<TaskGroup> GetExecutionWays()
        {
            return this.executionWays;
        }
        public bool CanBePerformed()
        {
            foreach(var task in this.GetExecutionWays()) //!need to initialize
            {
                if(task.CanBePerformed())
                {
                    return true;
                }
            }
            return false;
        }   
        public double Benefit()
        {
            return GameInfo.Game.turnsLeft * (this.iceberg.PenguinsPerTurn + 1) * 100; 
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
            return "upgradeIceberg: " + this.iceberg.UniqueId + " " + this.iceberg.Id;
        }

        public override string ToString()
        {
            return this.GetDescription();
        }

    }
}