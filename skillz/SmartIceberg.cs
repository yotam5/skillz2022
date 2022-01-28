using PenguinGame;

namespace MyBot
{

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

        public bool CanUpgrade
        {
            get{return this._iceberg.CanUpgrade();}
        }

        public override int GetHashCode()
        {
            return this._iceberg.GetHashCode();
        }

        public bool Equals(Iceberg obj)
        {
            return obj == this._iceberg;
        }

        public void SendPenguins(Iceberg destination, int amount)
        {
            if(!this._upgraded){
                this._sent = true;
                this._iceberg.SendPenguins(destination,amount);
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

        public int GetTurnsTillArrival(Iceberg destination)
        {
            return this._iceberg.GetTurnsTillArrival(destination);
        }

    }

}

