using PenguinGame;
using System.Collections.Generic;
using System.Linq;

namespace MyBot
{
    public class SmartIceberg : Iceberg
    {
        private Iceberg iceberg;
        public Iceberg Iceberg { get { return this.iceberg; } }

        private int savedPenguins;
        public int SavedPenguin { get { return this.savedPenguins; } }
        public new int PenguinAmount { get { return this.iceberg.PenguinAmount; } }
        public new int PenguinsPerTurn { get { return this.iceberg.PenguinsPerTurn; } }

        public new int UpgradeCost { get { return this.iceberg.UpgradeCost; } }

        public new int Level { get { return this.iceberg.Level; } }

        public new Player Owner { get { return this.iceberg.Owner; } }

        public new int UpgradeValue { get { return this.iceberg.UpgradeValue; } }
        public new int CostFactor { get { return this.iceberg.CostFactor; } }

        public SmartIceberg(Iceberg iceberg)
        {
            this.iceberg = iceberg;
            this.savedPenguins = 0;
        }

        public int GetUnusedPenguins()
        {
            return this.PenguinAmount - this.savedPenguins;
        }

        public void SavePenguins(int amount)
        {
            this.savedPenguins = amount - this.PenguinsPerTurn;
        }

        public int GetTurnsTillArrival(SmartIceberg destination)
        {
            return this.iceberg.GetTurnsTillArrival(destination.iceberg);
        }


        public override int GetHashCode()
        {
            return this.iceberg.GetHashCode();
        }

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
            return this.iceberg.Equals(other);
        }

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

        public List<PenguinGroup> GetIncomingEnemyGroups()
        {
            var pgGroups = this.GetAllIncomingGroups();
            return (from pg in pgGroups where !pg.Owner.Id.Equals(this.Owner.Id) select pg).ToList(); //if id different from our enemy(relativly)

        }

        public List<PenguinGroup> GetIncomingFriendlyGroups()
        {
            var pgGroups = this.GetAllIncomingGroups();
            return (from pg in pgGroups where pg.Owner.Id.Equals(this.Owner.Id) select pg).ToList(); //if id is the same as our(relativly)     
        }

        public double AverageDistanceFromEnemy()
        {
            double totalDistance = 0;
            int counter = 0;
            foreach (var ice in GameInfo.Icebergs.allIcebergs)
            {
                if (this.iceberg.Owner.Id != ice.iceberg.Owner.Id && this.iceberg.Owner.Id != -1)
                {
                    totalDistance += this.iceberg.GetTurnsTillArrival(ice.iceberg);
                    ++counter;
                }
            }
            return totalDistance / counter;
        }

        public void SendPenguins(SmartIceberg dest, int amount)
        {
            this.iceberg.SendPenguins(dest.iceberg, amount);
        }

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
        }

        public double AverageDistanceFromAlly()
        {
            double totalDistance = 0;
            int counter = 0;
            foreach (var ice in GameInfo.Icebergs.allIcebergs)
            {
                if (ice.iceberg.Owner.Id != -1 && !ice.Equals(this.iceberg) && ice.iceberg.Owner.Id ==
                    this.iceberg.Owner.Id)
                {
                    totalDistance += this.iceberg.GetTurnsTillArrival(ice.iceberg);
                    ++counter;
                }
            }
            return totalDistance / counter;
        }

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

        public int minPenguinsToTakeOver(int turnLimit) //!should use worst case senario, probably
        {
            var icebergOwner = this.GetOwnerId();
            var allIncomingPenguinGroups = this.GetAllIncomingGroups().OrderBy(x => x.TurnsTillArrival).ToList();
            if (allIncomingPenguinGroups.Count() == 0)
            {
                System.Console.WriteLine("no incoming groups for " + this.iceberg);
                if (icebergOwner == -1)
                {
                    return this.PenguinAmount + 1;
                }
                else if (icebergOwner == GameInfo.Players.enemyPlayer.Id)
                {
                    return this.PenguinAmount + (this.PenguinsPerTurn * turnLimit) + 1;
                }
                else
                {
                    System.Console.WriteLine("unexpected player id");
                }
            }
            int icebergPgCounter = 0;
            int penguinTurnsTillArrival = turnLimit;
            int previousTurns = 0;
            int startTurn = turnLimit;
            int turnsSinceTaken = turnLimit;

            foreach (var penguinGroup in allIncomingPenguinGroups)
            {
                penguinTurnsTillArrival = penguinGroup.TurnsTillArrival - previousTurns;
                turnsSinceTaken = startTurn - previousTurns;
                if (penguinTurnsTillArrival <= turnLimit)
                {
                    if (!icebergOwner.Equals(GameInfo.Players.neutral.Id))
                    {
                        icebergPgCounter += penguinGroup.TurnsTillArrival * this.iceberg.PenguinsPerTurn;
                    }
                    if (icebergOwner.Equals(penguinGroup.Owner.Id))
                    {
                        icebergPgCounter += penguinGroup.PenguinAmount;
                    }
                    else
                    {
                        icebergPgCounter -= penguinGroup.PenguinAmount;
                        if (icebergPgCounter < 0)
                        {
                            icebergPgCounter = System.Math.Abs(icebergPgCounter);
                            icebergOwner = penguinGroup.Owner.Id;
                            turnsSinceTaken = turnLimit - penguinGroup.TurnsTillArrival;
                        }
                    }
                }
                previousTurns += penguinTurnsTillArrival;
            }
            return icebergPgCounter + this.iceberg.PenguinsPerTurn * turnsSinceTaken + 1; ;
        }

        public int PotentialBackup(int turnTillArrival)
        {
            var relativeEnemyIcebergs = (from ice in GameInfo.Icebergs.allIcebergs
                                         where
                                         ice.Owner.Id != -1 &&
                                         ice.Owner.Id != this.Owner.Id &&
                                         ice.GetTurnsTillArrival(this) <= turnTillArrival
                                         select ice).ToList();

            int totalEnemies = this.PenguinAmount;

            if (this.Owner.Id != -1)
            {
                totalEnemies += this.PenguinsPerTurn * turnTillArrival;
            }
            foreach (var relativeEnemy in relativeEnemyIcebergs)
            {
                totalEnemies += relativeEnemy.PenguinAmount +
                    relativeEnemy.PenguinsPerTurn * (turnTillArrival - relativeEnemy.GetTurnsTillArrival(this));
            }
            return totalEnemies;
        }

        public SmartIceberg Remotest(List<SmartIceberg> icebergs)
        {
            var l = icebergs.OrderBy(x => x.GetTurnsTillArrival(this)).ToList().Last();
            if (icebergs.Count() == 0 || l == null)
            {
                return this;
            }
            return l;
        }

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

        public List<(int, int)> FutureState(bool upgrade = false, int addition = 0)
        {
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

            if (this.Owner.Id == -1)
            {
                //! need to do something ...
            }

            foreach (var group in this.AllIncomingPenguinGroups())
            {
                if (group.Owner.Id == this.Owner.Id)
                {
                    incomingGroups.Add((group.PenguinAmount, group.TurnsTillArrival));
                }
                else
                {
                    incomingGroups.Add((-group.PenguinAmount, group.TurnsTillArrival));
                }
            }
            incomingGroups = incomingGroups.OrderBy(x => x.Item2).ToList();
            if (incomingGroups.Count() == 0) { System.Console.WriteLine("no incoming gorups"); }
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
