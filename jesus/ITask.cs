using PenguinGame;
using System.Collections.Generic;
using System.Linq;

namespace MyBot
{
    public interface ITask
    {
         SmartIceberg GetActor();
         SmartIceberg GetTarget();

         void Performe();

         int Loss();

         int PenguinsRequired();

         int TaskTurns();

    }
}