using PenguinGame;
using System.Collections.Generic;
using System.Linq;

namespace MyBot
{
    public class Attack : ITask
    {
        private SmartIceberg source;
        private SmartIceberg destination;
        private List<TaskGroup> executionWays;
        private int penguinsRequired;


        public Attack(SmartIceberg source, SmartIceberg dest,int penguinAmount)
        {
            this.source = source;
            this.destination = dest;
            this.penguinsRequired = penguinAmount;
            this.executionWays = new List<TaskGroup>();
        }

        public SmartIceberg GetActor()
        {
            return this.source;
        }

        public SmartIceberg GetTarget()
        {
            return this.destination;
        }

        public void Performe()
        {
            this.source.SendPenguins(this.destination, this.penguinsRequired);
        }

        public int PenguinsRequired()
        {
            return this.penguinsRequired;
        }

        public int Loss()
        {
           return (this.penguinsRequired / this.source.PenguinAmount) + this.source.GetTurnsTillArrival(this.destination);
        }

        public override string ToString()
        {
            return "attacker: " + this.source + " target: " + this.destination;
        }

    }
}