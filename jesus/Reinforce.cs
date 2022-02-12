using PenguinGame;
using System.Collections.Generic;
using System.Linq;

namespace MyBot
{
    public class Reinforce : ITask
    {
        private SmartIceberg source;
        private SmartIceberg destination;

        private int requiredPenguinAmount;

        public Reinforce(SmartIceberg source, SmartIceberg dest, int penguinAmount)
        {
            this.source = source;
            this.destination = dest;
            this.requiredPenguinAmount = penguinAmount;
        }
        public SmartIceberg GetActor(){
            return this.source;
        }

        public void Performe(){
            this.source.SendPenguins(this.destination,this.requiredPenguinAmount);
        }

        public int Loss(){
            return this.requiredPenguinAmount/this.source.PenguinAmount + this.source.GetTurnsTillArrival(this.destination);
        }

        public SmartIceberg GetTarget()
        {
            return this.destination;
        }

        public override string ToString()
        {
            return "reinforcer: " + this.source + " reinforced: " + this.destination;
        }

        public int PenguinsRequired(){
            return this.requiredPenguinAmount;
        }

    }
}