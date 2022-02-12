using PenguinGame;
using System.Collections.Generic;
using System.Linq;

namespace MyBot
{
    public enum MissionState
    {
        ACTIVE,
        FINISHED,
        PAUSED
    }

    public interface IMission
    {
        void SetMissionState(MissionState state);
        SmartIceberg GetTarget();

        int Benefit();

        void CalculateExecutionWays();

        List<TaskGroup> GetExecutionWays();

        string GetDescription();
    }
}