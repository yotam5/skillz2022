using PenguinGame;
using System.Collections.Generic;
using System.Linq;

namespace MyBot
{
    public class ConquerIceberg : IMission
    {
        private int timer;
        private SmartIceberg iceberg;
        private List<TaskGroup> executionWays;
        private MissionState state;
        public ConquerIceberg(SmartIceberg iceberg)
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

        public void TimerDown()
        {
            --this.timer;
        }

        public void SetTimer(int value)
        {
            this.timer = value;
        }

        public bool CanBePerformed()
        {
            System.Console.WriteLine($"can conqure in {this.GetExecutionWays().Count()}");
            foreach(var task in this.GetExecutionWays()) //!need to initialize
            {
                if(task.CanBePerformed())
                {
                    return true;
                }
            }
            return false;
        }

        public void InitializeExecutionWays()
        {
            this.executionWays = MissionManager.GetExecutionWays(this);
        }

        public double Benefit()
        {
            return GameInfo.Game.turnsLeft/this.iceberg.AverageDistanceFromPlayer(GameInfo.Players.mySelf.Id) + 
                this.iceberg.PenguinsPerTurn*5 - this.iceberg.PenguinAmount*2;
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
            return "ConqurerIceberg: " + this.GetTarget().UniqueId + " " + this.GetTarget().Id;
        }

        public override string ToString()
        {
            return this.GetDescription();
        }

            
    }
}