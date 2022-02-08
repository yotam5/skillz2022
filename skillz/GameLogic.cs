using PenguinGame;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace MyBot
{
    public static class GameLogic
    {
        //execute ur most important person infront of goku son's
        public static void execute(Game game)
        {
            //id left first 16
            //id right first 40
            if (game.Turn == 1)
            {
                game.GetMyIcebergs()[0].Upgrade();
                //System.Console.WriteLine($"{game.GetMyIcebergs()[0].UniqueId}");
                GameInfo.InitializeUpgradeDict(game);
                GameInfo.InitializeAttckedEnemyIcebergs(game);

            }
            else if (game.Turn == 7)
            {
                Defensive.DefendIcebergs(game);
                game.GetMyIcebergs()[0].SendPenguins(game.GetNeutralIcebergs()[0], 11);
            }
            else if (game.Turn == 12)
            {
                Defensive.DefendIcebergs(game);
                game.GetMyIcebergs()[0].SendPenguins(game.GetNeutralIcebergs()[1], 11);
            }
            else if (game.Turn == 19)
            {
                Defensive.DefendIcebergs(game);
                var nc = game.GetNeutralIcebergs().OrderBy(x => Utils.AverageDistanceFromMyIcbergs(game, x)).ToList().First();

                game.GetMyIcebergs()[0].SendPenguins(nc, 13);
                game.GetMyIcebergs()[1].SendPenguins(nc, 4);


            }
            else if (game.Turn == 22)
            {
                Defensive.DefendIcebergs(game);
                var nc = game.GetNeutralIcebergs().OrderBy(x => Utils.AverageDistanceFromMyIcbergs(game, x)).ToList().First();

                game.GetMyIcebergs()[1].SendPenguins(nc, 5);


            }
            else if (game.Turn >= 23)
            {
                GameInfo.UpdateAttackedByus(game);
                Defensive.DefendIcebergs(game);
                //Offensive.MultiThreadedAttack(game);
                Offensive.Attack(game);
                Defensive.DefendIcebergs(game);
                Offensive.test1(game);
                Defensive.DefendIcebergs(game);
                GameLogic.UpgradeRoutine(game);
                Defensive.DefendIcebergs(game);
                GameLogic.SendToWall(game);
                Defensive.DefendIcebergs(game);


            }
            GameInfo.EndTurn(game);

        }


        /// <summary>
        /// send penguins from all icebergs to the wall
        /// </summary>
        /// <param name="game"></param>
        public static void SendToWall(Game game)
        {
            var wallIce = Defensive.GetWall(game);
            if (wallIce.Length < 2) { return; }
            foreach (var myIce in game.GetMyIcebergs())
            {
                if (!myIce.Equals(wallIce[0]) && !myIce.Equals(wallIce[1]))
                {
                    int amountToSend = (myIce.PenguinAmount / 2 - 1) * 2;
                    if (myIce.Level > 1 && !GameInfo.UpgradedThisTurn(myIce.UniqueId) &&
                        Utils.HelpIcebergData(game, myIce, amountToSend).Count() == 0
                        && !GameInfo.UpgradedThisTurn(myIce.UniqueId))
                    {
                        if (amountToSend / 2 >= 1)
                        {
                            myIce.SendPenguins(wallIce[0], amountToSend / 2);
                            myIce.SendPenguins(wallIce[1], amountToSend / 2);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// return the difference in penguin rate
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public static int DeltaPenguinsRate(Game game)
        {
            int deltaRate = 0;
            foreach (var ice in game.GetMyIcebergs())
            {
                deltaRate += ice.Level;
            }
            foreach (var ice in game.GetEnemyIcebergs())
            {
                deltaRate -= ice.Level;
            }
            return deltaRate;
        }

        /// <summary>
        /// return the difference in the total penguins amount
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public static int DeltaPenguinAmount(Game game)
        {
            int deltaAmount = 0;
            foreach (var myIce in game.GetMyIcebergs())
            {
                deltaAmount += myIce.PenguinAmount;
            }
            foreach (var enemIce in game.GetEnemyIcebergs())
            {
                deltaAmount -= enemIce.PenguinAmount;
            }
            return deltaAmount;
        }


        /// <summary>
        /// upgrade  our icebergs if they can upgrade
        /// </summary>
        /// <param name="game"></param>
        public static void UpgradeRoutine(Game game)
        {
            //!if(GameLogic.DeltaPenguinAmount(game) > 400){return;}

            foreach (var myIceberg in game.GetMyIcebergs())
            {
                if (!myIceberg.AlreadyActed && myIceberg.CanUpgrade() &&
                    Utils.HelpIcebergData(game, myIceberg, 0, true).Count() == 0)
                {
                    myIceberg.Upgrade();
                    GameInfo.UpdateUpgradeDict(myIceberg.UniqueId);
                }
            }
            GameLogic.SendForUpgrade(game);
        }

        /// <summary>
        /// send penguins for icebergs so that they can upgrade themself
        /// </summary>
        /// <param name="game"></param>
        public static void SendForUpgrade(Game game)
        {
            const int maxUpgradesInTurn = 1; //!
            if (GameLogic.DeltaPenguinAmount(game) > 0)
            {
                var myIcebergs = game.GetMyIcebergs().ToList();
                //game.GetNeutralIcebergs().ToList().ForEach(x => myIcebergs.Add(x));
                myIcebergs = (from ice in myIcebergs where ice.Level < 4 select ice).ToList();
                myIcebergs = myIcebergs.OrderBy(x => Utils.AverageDistanceFromEnemy(game, x)).ToList();
                var selectedToUpgrade = new List<Iceberg>();
                foreach (var ice in myIcebergs)
                {
                    //System.Console.WriteLine($"ice upgrade is {GameInfo.UpgradedThisTurn(ice.UniqueId)}");
                    if (!GameInfo.UpgradedThisTurn(ice.UniqueId))
                    {
                        selectedToUpgrade.Add(ice);
                    }
                }
                int upgradeCounter = 0;
                foreach (var ice in selectedToUpgrade)
                {
                    if (upgradeCounter == maxUpgradesInTurn)
                    {
                        break;
                    }
                    int upgradeCost = ice.UpgradeCost;

                    //! to do if my deltapenguin amount is equal or bigger
                    var sendData = new List<(int, int)>();
                    sendData.Add((upgradeCost, 999));
                    Utils.SendAmountWithTurnsLimit(game, ice, sendData);
                    upgradeCounter++;
                }
            }

        }
    }
}
