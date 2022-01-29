using PenguinGame;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace MyBot
{
    
    //TODO: verify actions that can be initiated only on my icebergs indicate when action taken on wrong iceberg
    //TODO: to contain information about what groups are coming to the iceberg
    //TODO: take action if in danger?
    public class SmartIceberg 
    {
        private Iceberg _iceberg;
        private bool _sent;

        private bool _upgraded;
        public bool upgraded{get{return this._upgraded;}}
        public bool _empty{get;}
        public SmartIceberg(Iceberg iceberg)
        {
            this._iceberg = iceberg;
            this._sent = false;
            this._upgraded = false;
            this._empty = false;
        }
        public SmartIceberg()
        {
            this._iceberg = new Iceberg();
            this._empty = true;
        }

        public int PenguinAmount
        {
            get { return this._iceberg.PenguinAmount; }
        }

        public Player Owner
        {
            get{return this._iceberg.Owner;}
        }

        public int Level
        {
            get{return this._iceberg.Level;}
        }

        public int PenguinsPerTurn
        {
            get{return this._iceberg.PenguinsPerTurn;}
        }

        public int CostFactor
        {
            get{return this._iceberg.CostFactor;}
        }

        public int UpgradeCost
        {
            get{return this._iceberg.UpgradeCost;}
        }

        public int UpgradeLevelLimit
        {
            get{return this._iceberg.UpgradeLevelLimit;}
        }

        public int UpgradeValue
        {
            get{return this._iceberg.UpgradeValue;}
        }

        public bool CanUpgrade()
        {
            return this._iceberg.CanUpgrade();
        }

        public override int GetHashCode()
        {
            return this._iceberg.GetHashCode();
        }

        public bool Equals (Iceberg obj)
        {
           // System.Console.WriteLine($"{obj} id is {obj.UniqueId}, iceberg {this._iceberg} id is {this._iceberg.UniqueId}");
            return this._iceberg.Equals(obj);
        }

        public void SendPenguins(SmartIceberg destination, int amount)
        {
            if(amount <= 0){
                System.Console.WriteLine("you cant send negative nor zero pg");
            }
            if(this._iceberg.CanSendPenguins(destination._iceberg,amount)){
                this._iceberg.SendPenguins(destination._iceberg,amount);
            }
            else{
                System.Console.WriteLine("trying to send penguins when iceberg upgraded");
            }
        }

        public bool CanSendPenguins(SmartIceberg dest,int amount)
        {
            return this._iceberg.CanSendPenguins(dest._iceberg,amount);
        }

        public void Upgrade(){
            if(this._iceberg.CanUpgrade())
            {
                this._upgraded = true;
                this._iceberg.Upgrade();
            }
            else
            {
                System.Console.WriteLine("trying to upgrade iceberg that sent penguins");
            }
        }

        public int GetTurnsTillArrival(SmartIceberg destination)
        {
            if(destination == null){
                System.Console.WriteLine("error null instance");
                return -1;
            }
            return this._iceberg.GetTurnsTillArrival(destination._iceberg);
        }

        public int Id
        {
            get{return this._iceberg.Id;}
        }

        public int UniqueId
        {
            get{return this._iceberg.UniqueId;}
        }

    }

}

