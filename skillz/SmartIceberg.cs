using PenguinGame;

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
        public SmartIceberg(Iceberg iceberg)
        {
            this._iceberg = iceberg;
            this._sent = false;
            this._upgraded = false;
        }

        public int PenguinAmount
        {
            get { return this._iceberg.PenguinAmount; }
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
            System.Console.WriteLine($"{obj} id is {obj.UniqueId}, iceberg {this._iceberg} id is {this._iceberg.UniqueId}");
            return this._iceberg.Equals(obj);
        }

        public void SendPenguins(SmartIceberg destination, int amount)
        {
            if(!this._upgraded){
                this._sent = true;
                this._iceberg.SendPenguins(destination._iceberg,amount);
            }
            else{
                System.Console.WriteLine("trying to send penguins when iceberg upgraded");
            }
        }

        public void Upgrade(){
            if(!this._sent)
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

