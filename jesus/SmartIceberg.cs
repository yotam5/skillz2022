using PenguinGame;
using System.Collections.Generic;
using System.Linq;

namespace MyBot
{
    public class SmartIceberg : Iceberg, System.IEquatable<SmartIceberg>
    {
        private Iceberg iceberg;
        public Iceberg pIceberg { get { return this.iceberg; } }

        private bool upgraded;
        public bool Upgraded{get{return this.upgraded;}}
        public new int UniqueId{get{return this.iceberg.UniqueId;}}
        private int savedPenguins;
        public int SavedPenguins { get { return this.savedPenguins; } }
        public new int PenguinAmount { get { return this.iceberg.PenguinAmount; } }
        public new int PenguinsPerTurn { get { return this.iceberg.PenguinsPerTurn; } }

        public new int UpgradeCost { get { return this.iceberg.UpgradeCost; } }

        public new int Level { get { return this.iceberg.Level; } }

        public new Player Owner { get { return this.iceberg.Owner; } }

        public new int UpgradeValue { get { return this.iceberg.UpgradeValue; } }
        public new int CostFactor { get { return this.iceberg.CostFactor; } }

        public new List<Bridge> Bridges {get{return this.iceberg.Bridges.ToList();}}
        public new int Id {get{return this.iceberg.Id;}}
        public new bool AlreadyActed {get{return this.iceberg.AlreadyActed;}}
        public SmartIceberg(Iceberg iceberg)
        {
            this.iceberg = iceberg;
            this.savedPenguins = 0;
            this.upgraded = false;
        }

        public bool BridgeConnected(SmartIceberg ice)
        {
            foreach(var b in this.Bridges.ToList())
            {
                var edges = b.GetEdges();
                foreach(var e in edges)
                {
                    if(ice.iceberg.Equals(e))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CanCreateBridge(SmartIceberg dest)
        {
            bool cond1 = this.iceberg.PenguinAmount > GameInfo.Bridge.bridgeCost && !this.iceberg.AlreadyActed &&
                !this.iceberg.Equals(dest.iceberg) ;
            foreach(var e in this.Bridges.ToList())
            {
                if(e.__edge2.Equals(this.iceberg) || e.__edge1.Equals(this.iceberg)){ //!note
                    return false;
                }
            }
            return true;
        }
        public override bool CanCreateBridge(Iceberg dest)
        {
            return this.iceberg.PenguinAmount > 4 && !this.iceberg.AlreadyActed &&
                !this.iceberg.Equals(dest);        }
        public void CreateBridge(SmartIceberg dest)
        {
            this.iceberg.CreateBridge(dest.iceberg);
        }

        /// <summary>
        /// return the amount of unused penguins, that can be actually used
        /// </summary>
        /// <returns>amount of penguins available for use</returns>
        public int GetUnusedPenguins()
        {
            return this.PenguinAmount - this.savedPenguins;
        }

        public void SavePenguins(int amount)
        {
            this.savedPenguins = amount;
        }

        public void ReleasePenguins(int amount)
        {
            this.savedPenguins -= amount;
        }
        /// <summary>
        /// return turns until arrival to another iceberg
        /// </summary>
        /// <param name="destination">destination iceberg</param>
        /// <returns>turns until arrival</returns>
        public int GetTurnsTillArrival(SmartIceberg destination)
        {
            return this.iceberg.GetTurnsTillArrival(destination.iceberg);
        }


        public override int GetHashCode()
        {
            return this.iceberg.GetHashCode();
        }


        /// <summary>
        /// return this iceberg owner id
        /// </summary>
        /// <returns>this iceberg owner id</returns>
        public int GetOwnerId()
        {
            return this.iceberg.Owner.Id;
        }

        public bool Equals(SmartIceberg other)
        {
            return this.iceberg.Equals(other.iceberg);
        }

        public bool Equals(Iceberg other)
        {
            return this.iceberg.UniqueId == other.UniqueId; 
        }
    
        /// <summary>
        /// return all of the incoming penguin groups to this iceberg
        /// </summary>
        /// <returns>all incoming groups to this iceberg</returns>
        public List<PenguinGroup> GetAllIncomingGroups()
        {
            var pgGroups = GameInfo.PenguinGroups.allPenguinGroup;
            var incoming = new List<PenguinGroup>();
            foreach (var pg in pgGroups.ToList())
            {
                if (pg.Destination.Equals(this.iceberg))
                {
                    incoming.Add(pg);
                }
            }
            return incoming;
        }

        /// <summary>
        /// return all incoming penguin groups that have different id that this iceberg owner
        /// </summary>
        /// <returns>all of the enemy incoming penguin groups</returns>
        public List<PenguinGroup> GetIncomingEnemyGroupsRelative()
        {
            var pgGroups = this.GetAllIncomingGroups();
            return (from pg in pgGroups where !pg.Owner.Id.Equals(this.Owner.Id) select pg).ToList(); //if id different from our enemy(relativly)

        }

        /// <summary>
        /// return all incoming penguin groups with this iceberg owner id
        /// </summary>
        /// <returns>all of the ally incoming penguins groups</returns>
        public List<PenguinGroup> GetIncomingFriendlyGroups()
        {
            var pgGroups = this.GetAllIncomingGroups();
            return (from pg in pgGroups where pg.Owner.Id.Equals(this.Owner.Id) select pg).ToList(); //if id is the same as our(relativly)     
        }

        public double AverageDistanceFromPlayer(int playerId) 
        {
            bool neutral = this.Owner.Id == -1;
            double totalDistance = 0;
            int counter = 0;
            foreach (var ice in GameInfo.Icebergs.allIcebergs)
            {
                if(ice.Owner.Id == playerId && !ice.Equals(this))
                {
                    totalDistance += this.GetTurnsTillArrival(ice);
                    ++counter;
                }
            }
            if(counter == 0){return 0;}
            return totalDistance / counter;
        }

        /// <summary>
        /// send penguin to destination with the given amount
        /// </summary>
        /// <param name="dest">destination to send</param>
        /// <param name="amount">amount to send</param>
        public void SendPenguins(SmartIceberg dest, int amount)
        {
            if(this.CanSendPenguins(dest,amount))
            {
                //this.savedPenguins += amount; //!note
            }
            this.iceberg.SendPenguins(dest.iceberg, amount);
        }

        /// <summary>
        /// check if icebergs can send penguins
        /// </summary>
        /// <param name="dest">destination</param>
        /// <param name="amount">amount to send</param>
        /// <returns>if the iceberg can send them</returns>
        public bool CanSendPenguins(SmartIceberg dest, int amount)
        {
            if (this.GetUnusedPenguins() >= amount && this.iceberg.CanSendPenguins(dest.iceberg, amount))
            {
                return true;
            }
            return false;
        }

        public override bool CanUpgrade()
        {
            return this.iceberg.CanUpgrade();
        }

        public override void Upgrade()
        {
            this.iceberg.Upgrade();
            this.upgraded = true;
        }


        /// <summary>
        /// return all the incoming groups to this iceberg
        /// </summary>
        /// <returns>all incoming penguin groups to this iceberg</returns>
        public List<PenguinGroup> AllIncomingPenguinGroups()
        {
            var incomingGroups = new List<PenguinGroup>();
            foreach (var pg in GameInfo.PenguinGroups.allPenguinGroup)
            {
                if (pg.Destination.Equals(this.iceberg))
                {
                    incomingGroups.Add(pg);
                }
            }
            return incomingGroups;
        }
        public double GetTurnsTillArrivalWithBridge(SmartIceberg iceberg)
        {
            int turns = iceberg.GetTurnsTillArrival(this);
            int speed = (int)GameInfo.Bridge.bridgeSpeed;
            int duration = GameInfo.Bridge.bridgeDuration;
            if(speed * duration >= turns)
            {
                return turns/speed;
            }
            return duration + (turns - speed * duration);
        }

        /// <summary>
        /// calculate the worst case senario of enemies when arriving to iceberg
        /// </summary>
        /// <param name="turnTillArrival">turn untill arrival</param>
        /// <returns>potencial amount of enemies when arriving</returns>
        public int PotentialBackup(int turnTillArrival,int ownerId) //todo: check if delay of 1 in calculation
        {
            int totalEnemies = this.PenguinAmount;

            var relativeFriendlyIcebergs = (from ice in GameInfo.Icebergs.allIcebergs
                                         where
                                         ice.Owner.Id != -1 &&
                                         ice.Owner.Id == ownerId &&
                                         ice.GetTurnsTillArrival(this) <= turnTillArrival &&
                                         !ice.Equals(this)
                                         select ice).ToList();
            //var onGoingGroups = this.GetAllIncomingGroups();
            var onGoingGroups = this.GetIncomingFriendlyGroups();
            onGoingGroups =
                            (from pg in onGoingGroups
                             where
                             pg.TurnsTillArrival <= turnTillArrival
                             select pg).ToList();
            onGoingGroups = onGoingGroups.OrderBy(pg=>pg.TurnsTillArrival).ToList();

            totalEnemies += onGoingGroups.Sum(x=>x.PenguinAmount);

            if (this.Owner.Id != -1)
            {
                totalEnemies += this.PenguinsPerTurn * turnTillArrival;
            }
            foreach (var relativeEnemy in relativeFriendlyIcebergs)
            {
                totalEnemies += relativeEnemy.PenguinAmount + (relativeEnemy.PenguinsPerTurn * turnTillArrival) -1;
            }
            totalEnemies += Utils.GetBonusUntillArrival(this,turnTillArrival);
            return totalEnemies;
        }


        /// <summary>
        /// return the iceberg who is the fartheest relativly to this
        /// </summary>
        /// <param name="icebergs">list to check distance from</param>
        /// <returns>the remotest iceberg</returns>
        public SmartIceberg Remotest(List<SmartIceberg> icebergs)
        {
            var l = icebergs.OrderBy(x => x.GetTurnsTillArrival(this)).ToList().Last();
            if (icebergs.Count() == 0 || l == null)
            {
                return this;
            }
            return l;
        }

        /// <summary>
        /// the penguin amount that comes directly from another ice to this
        /// </summary>
        /// <param name="iceberg">ice to check from</param>
        /// <returns>int, the amount that comes to this iceberg</returns>
        public int GetIncomingPenguinsFromIceberg(SmartIceberg iceberg)
        {
            var allIncomingPenguinGroups = this.GetAllIncomingGroups();
            int total = 0;
            foreach (var t in allIncomingPenguinGroups)
            {
                if (t.Destination.Equals(this))
                {
                    total += t.PenguinAmount;
                }
            }
            return total;
        }


        /// <summary>
        /// return a list with amount and time to deliver to prevent iceberg from being taken
        /// </summary>
        /// <param name="upgrade">take into account if iceberg is upgraded</param>
        /// <param name="addition">take into account if iceberg send penguins</param>
        /// <returns>(amount,turnsToDeliver) to save iceberg</returns>
        public List<(int, int)> PreventConqure(bool upgrade = false, int addition = 0,bool b=true)
        {
            //! need to take into account icebergs that still didnt send
            var incomingGroups = new List<(int, int)>();

            var helpData = new List<(int, int)>();

            int penguinPerTurn = this.PenguinsPerTurn;

            int icebergCounter = this.PenguinAmount;
            if (upgrade && iceberg.Level < 4)
            {
                addition += this.iceberg.UpgradeCost;
                penguinPerTurn = this.Level + 1;
            }
            icebergCounter -= addition;
            if(this.PenguinAmount - addition <= 0)
            {
                var tmp = new List<(int,int)>();
                tmp.Add((-1,-1));
                return tmp;
            }
            if (this.Owner.Id == -1)
            {
                //!need to check
            }

            foreach (var group in this.AllIncomingPenguinGroups())
            {
                
                if (group.Owner.Id != this.Owner.Id)
                {
                    incomingGroups.Add((-group.PenguinAmount, Utils.TurnsTillArrivalWithBridge(group,worst: b)));
                }
                else
                {
                    incomingGroups.Add((group.PenguinAmount, Utils.TurnsTillArrivalWithBridge(group,worst: false)));
                }
            }

            incomingGroups = incomingGroups.OrderBy(x => x.Item2).ToList();
            int totalDistance = 0;
            while (incomingGroups.Count() > 0)
            {
                int closestGroup = incomingGroups.First().Item2;
                totalDistance += closestGroup;
                for (int i = 0; i < incomingGroups.Count(); i++)
                {
                    incomingGroups[i] = (incomingGroups[i].Item1, incomingGroups[i].Item2 - closestGroup);
                }
                var arrived = (from pg in incomingGroups where pg.Item2 == 0 select pg.Item1).ToList();
                for (int i = arrived.Count(); i > 0; i--, incomingGroups.RemoveAt(0)) ;
                icebergCounter += closestGroup * penguinPerTurn + arrived.Sum();

                if (icebergCounter <= 0)
                {
                    helpData.Add((-1 * icebergCounter + 1, totalDistance));
                    penguinPerTurn *= -1;
                }
                else 
                {
                    penguinPerTurn = System.Math.Abs(penguinPerTurn);
                }
            }
            return helpData;
        }

        public override string toString()
        {
            return this.iceberg.ToString();
        }
        

    }
}
