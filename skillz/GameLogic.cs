using PenguinGame;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace MyBot
{
    public static class GameLogic
    {
        public static void execute(Game game)
        {
            //id left first 16
            //id right first 40
            if (game.Turn == 1)
            {
                game.GetMyIcebergs()[0].Upgrade();
                System.Console.WriteLine($"{game.GetMyIcebergs()[0].UniqueId}");

            }
            else if (game.Turn == 7)
            {
                game.GetMyIcebergs()[0].SendPenguins(game.GetNeutralIcebergs()[0], 11);
            }
            else if (game.Turn == 12)
            {
                game.GetMyIcebergs()[0].SendPenguins(game.GetNeutralIcebergs()[1], 11);
            }
            else if (game.Turn == 19)
            {
                var nc = game.GetNeutralIcebergs().OrderBy(x=>Utils.AverageDistanceFromMyIcbergs(game,x)).ToList().First();
    
                game.GetMyIcebergs()[0].SendPenguins(nc, 13);
                game.GetMyIcebergs()[1].SendPenguins(nc, 4);


            }
            else if (game.Turn == 22)
            {
                var nc = game.GetNeutralIcebergs().OrderBy(x=>Utils.AverageDistanceFromMyIcbergs(game,x)).ToList().First();

                game.GetMyIcebergs()[1].SendPenguins(nc, 5);
                
                
            }
            else if ( game.Turn > 23)
            {
                Defensive.DefendIcebergs(game);
                Offensive.Attack(game);
                GameLogic.UpgradeRoutine(game);
                GameLogic.SendToWall(game);  
            }

        }
        public static void SendToWall(Game game)
        {
            var wallIce = Defensive.GetWall(game);
            if (wallIce.Length == 1) {return;}
            foreach (var myIce in game.GetMyIcebergs())
            {
                if (!myIce.Equals(wallIce[0]) && !myIce.Equals(wallIce[1]))
                {
                    if (myIce.Level > 1 && !myIce.AlreadyActed &&
                        Utils.HelpIcebergData(game, myIce, myIce.PenguinAmount - 1).Count() == 0)
                    {
                        int amountToSend = myIce.PenguinAmount /2 - 1;
                        if(amountToSend >= 1)
                        {
                            myIce.SendPenguins(wallIce[0], amountToSend);
                            myIce.SendPenguins(wallIce[1], amountToSend);
                        }
                    }
                }
            }
        }

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

        public static int DeltaPenguinAmount(Game game)
        {
            int deltaAmount = 0;
            foreach(var myIce in game.GetMyIcebergs())
            {
                deltaAmount += myIce.PenguinAmount;
            }
            foreach(var enemIce in game.GetEnemyIcebergs())
            {
                deltaAmount -= enemIce.PenguinAmount;
            }
            return deltaAmount;
        }

        public static int WorstCaseEnemyReinforcment(Game game, Iceberg enemyIceberg, int turnsTillArrival)
        {
            var enemyIcebergs = game.GetEnemyIcebergs();
            int totalEnemies = enemyIceberg.PenguinAmount + enemyIceberg.Level * turnsTillArrival;
            foreach (var reinforcmentIce in enemyIcebergs)
            {
                if (!reinforcmentIce.Equals(enemyIceberg) &&
                    reinforcmentIce.GetTurnsTillArrival(enemyIceberg) <= turnsTillArrival)
                {
                    totalEnemies += reinforcmentIce.PenguinAmount + (reinforcmentIce.PenguinsPerTurn * turnsTillArrival) -1;
                }
            }
            return totalEnemies;
        }


        public static void UpgradeRoutine(Game game)
        {
            //if(GameLogic.DeltaPenguinsRate(game) > 0){return;}
            foreach (var myIceberg in game.GetMyIcebergs())
            {
                if (!myIceberg.AlreadyActed && myIceberg.CanUpgrade() &&
                    Utils.HelpIcebergData(game, myIceberg, 0, true).Count() == 0)
                {
                    myIceberg.Upgrade();
                }
            }
        }

        //TODO: implement
        public static void SendForUpgrade(Game game)
        {
            var myIcebergs = game.GetMyIcebergs();
            
        }

    }
}
