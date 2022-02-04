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

            if (game.Turn == 1){
                game.GetMyIcebergs()[0].Upgrade();
                Start(game);
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
                foreach (var nc in game.GetNeutralIcebergs())
                {
                    if (nc.Id == 7)
                    {
                        game.GetMyIcebergs()[0].SendPenguins(nc, 13);
                        break;
                    }
                }
            }
            else if (game.Turn == 22)
            {
                foreach (var nc in game.GetNeutralIcebergs())
                {
                    if (nc.Id == 7)
                    {
                        game.GetMyIcebergs()[1].SendPenguins(nc, 5);
                        break;
                    }
                }
            }
            else if (game.Turn > 22)
            {
                Defensive.DefendIcebergs2(game);
                GameLogic.UpgradeRoutine(game);
                GameLogic.SendToWall(game);
                GameLogic.EndTurn(game);
            }
        }
        public static void SendToWall(Game game)
        {
            var wallIce = Defensive.GetWall(game);
            foreach (var myIce in game.GetMyIcebergs())
            {
                if (!myIce.Equals(wallIce) && Defensive.turtlemode[myIce.UniqueId] == 0)
                {
                    if (myIce.Level > 1 && !myIce.AlreadyActed && myIce.CanSendPenguins(wallIce[0], wallIce[0].Level) &&
                        Utils.HelpIcebergData(game, myIce, wallIce[0].Level).Count() == 0 && !myIce.Equals(wallIce))
                    {
                        myIce.SendPenguins(wallIce[0], myIce.PenguinAmount - 1);
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

        public static int WorstCaseEnemyReinforcment(Game game, Iceberg enemyIceberg, int turnsTillArrival)
        {
            int totalEnemies;
            var enemyIcebergs = game.GetEnemyIcebergs();
            if(enemyIceberg.Owner.Id == game.GetEnemy().Id){
                totalEnemies = enemyIceberg.PenguinAmount + enemyIceberg.Level * turnsTillArrival;
            }
            else{
                totalEnemies = enemyIceberg.PenguinAmount;
            }
            foreach (var reinforcmentIce in enemyIcebergs)
            {
                if (!reinforcmentIce.Equals(enemyIceberg) &&
                    reinforcmentIce.GetTurnsTillArrival(enemyIceberg) <= turnsTillArrival)
                {
                    totalEnemies +=reinforcmentIce.PenguinAmount + reinforcmentIce.PenguinsPerTurn * turnsTillArrival -1;
                }
            }
            return totalEnemies;
        }

        public static void UpgradeRoutine(Game game)
        {
            Iceberg theWall = Defensive.GetWall(game)[0];
            //if(GameLogic.DeltaPenguinsRate(game) > 0){return;}
            List<(Iceberg , int, int)> priority = new List<(Iceberg, int, int)>();
            foreach (var iceberg in game.GetMyIcebergs())
            {
                if(iceberg.Level < iceberg.UpgradeLevelLimit && Defensive.turtlemode[iceberg.UniqueId] > 0)
                {
                    System.Console.WriteLine("ABOUT TO UPGRADE");
                    if(!iceberg.AlreadyActed && Utils.BackupAtArrival(game, iceberg, 999) > iceberg.UpgradeCost){System.Console.WriteLine($"UPGRADING {iceberg}");iceberg.Upgrade();}

                    priority.Add((iceberg, 1 , iceberg.UpgradeCost));
                }
            }
            foreach(var iceberg in Utils.NClosestIcebergsToWall(game, 3)){

                priority.Add((iceberg, iceberg.Level, WorstCaseEnemyReinforcment(game, iceberg,
                WorstCaseEnemyReinforcment(game, iceberg, theWall.GetTurnsTillArrival(iceberg)))));

            }
            priority.OrderByDescending(u1 => u1.Item2);
            for(int i = 0; i < priority.Count(); i++){
                if (theWall.PenguinAmount - 10 >  priority[i].Item3 && !theWall.AlreadyActed){
                    if(!theWall.Equals(priority[i].Item1)){
                        theWall.SendPenguins(priority[i].Item1, priority[i].Item3 + 1);
                        Defensive.turtlemode[priority[i].Item1.UniqueId] += theWall.GetTurnsTillArrival(priority[i].Item1) + 1;
                        return;
                    }
                    else{
                        theWall.Upgrade();
                        return;
                    }
                }
            }
            
        }

        public static void Start(Game game) {
            foreach(var iceberg in game.GetAllIcebergs()){
                Defensive.turtlemode.Add(iceberg.UniqueId, 0);
            }
        }

        public static void EndTurn(Game game){
                foreach(var iceberg in game.GetAllIcebergs()){
                if(Defensive.turtlemode[iceberg.UniqueId] > 0){
                    Defensive.turtlemode[iceberg.UniqueId]--;
                }
            }
        }

    }
}

