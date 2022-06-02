using PenguinGame;
using System.Collections.Generic;
using System.Linq;

namespace MyBot
{
    public enum MissionState
    {
        ACTIVE,
        FINISHED,
        PAUSED,
        INITIALIZED
    }

    public interface IMission
    {
        void SetMissionState(MissionState state);

        MissionState GetMissionState();
        SmartIceberg GetTarget();

        double Benefit();

        void CalculateExecutionWays();

        bool CanBePerformed();

        List<TaskGroup> GetExecutionWays();

        string GetDescription();

        int GetTimer();

        void TimerUp();
        void TimerDown();

        void SetTimer(int value);
    }
}