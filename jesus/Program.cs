using PenguinGame;
using System.Collections.Generic;
using System.Linq;

namespace MyBot
{
    /// <summary>
    /// This is an example for a bot.
    /// </summary>


    public class TutorialBot : ISkillzBot
    {
        /// <summary>
        /// Makes the bot run a single turn.
        /// </summary>
        /// <param name="game">the current game state</param>
        public void DoTurn(Game game)
        {

            System.Console.WriteLine("line1");
            InitializeGameInfo(game);
            System.Console.WriteLine("line2");

            if(extream())
            {
                return;
            }
            
            if(game.Turn == 1){
                var k = game.GetMyIcebergs()[0];
                if(k.CanUpgrade()){
                    k.Upgrade();
                    return;
                }
            }
            System.Console.WriteLine("line3");

            //Utils.ConfigureIcebergs();
            if(game.Turn>=280 && game.GetMyself().Score <= game.GetEnemy().Score)
            {
                Utils.allornothing();
            }
            System.Console.WriteLine("line4");

            Utils.CalculateMissions();
            System.Console.WriteLine("line5");

            MissionManager.UpdateActiveMissions();
                    System.Console.WriteLine("line6");

            MissionManager.DistributionTasksForIcebergs();
            System.Console.WriteLine("line7");


            var w = Utils.GetWalls();

            foreach (var ice in GameInfo.Icebergs.myIcebergs.ToList())
            {
                if(!w.Contains(ice) && ice.Level >= 4 ||
                    (game.Turn < 36 && ice.Level >= 2 && ice.PenguinAmount >= 6))
                {
                    int amount = ice.PenguinAmount - 1;
                    if(ice.Level == 4){
                        amount = ice.PenguinAmount - 1;
                    }
                    var s = w.OrderBy(x=>x.GetIncomingFriendlyGroups().Count());
                    foreach(var ww in s)
                    {
                        if(ice.CanSendPenguins(ww,amount) && ice.PreventConqure(addition:amount).Count() == 0
                        && !ice.Upgraded)
                        {
                            ice.SendPenguins(ww,amount);
                        }
                    }
                }
            }

        }

        public bool extream()
        {
            var myIce = GameInfo.Icebergs.myIcebergs.ToList();
            var enemyIce = GameInfo.Icebergs.enemyIcebergs.ToList();
            var ss = myIce.First();
            if(myIce.Count() == 1){
                var closestNeutral = GameInfo.Icebergs.neutralIcebergs.OrderBy(x=>x.GetTurnsTillArrival(ss)).ToList().First();
                if(closestNeutral.GetTurnsTillArrival(ss) >= ss.GetTurnsTillArrival(enemyIce.First()))
                {
                    int tta = ss.GetTurnsTillArrival(enemyIce.First());
                    if(enemyIce.First().PotentialBackup(tta,enemyIce.First().Owner.Id) + 1 < ss.PenguinAmount)
                    {
                        ss.SendPenguins(enemyIce.First(),enemyIce.First().PotentialBackup(tta,enemyIce.First().Owner.Id) + 1);
                    }
                    return true;
                }
            }
            return false;
        }
        
        public void InitializeGameInfo(Game game)
        {
            //players
            GameInfo.Players.allPlayers = game.GetAllPlayers().ToList();
            GameInfo.Players.mySelf = game.GetMyself();
            GameInfo.Players.neutral = game.GetNeutral();
            GameInfo.Players.enemyPlayer = game.GetEnemy();

            //icebergs
            GameInfo.Icebergs.allIcebergs = Utils.ConvertToSmartIceberg(game.GetAllIcebergs());
            GameInfo.Icebergs.enemyIcebergs = Utils.ConvertToSmartIceberg(game.GetEnemyIcebergs());
            GameInfo.Icebergs.myIcebergs = Utils.ConvertToSmartIceberg(game.GetMyIcebergs());
            GameInfo.Icebergs.notMyIcebergs = Utils.NotMyIcebergs(game);
            GameInfo.Icebergs.neutralIcebergs = Utils.ConvertToSmartIceberg(game.GetNeutralIcebergs());
            GameInfo.Icebergs.threatenedIcebergs = Utils.GetMyDangeredIcebergs();
            //penguins groups
            GameInfo.PenguinGroups.myPenguinGroups = game.GetMyPenguinGroups().ToList();
            GameInfo.PenguinGroups.enemyPenguinGroups = (from pg in game.GetEnemyPenguinGroups() where !pg.Decoy select pg).ToList();
            GameInfo.PenguinGroups.allPenguinGroup = (from pg in game.GetAllPenguinGroups() where !pg.Decoy select pg).ToList();
            //GameInfo.Icebergs.threatenedIcebergs = Utils.ConvertToSmartIceberg(G)
            GameInfo.Game.maxTurns = game.MaxTurns;
            GameInfo.Game.turn = game.Turn;
            GameInfo.Game.turnsLeft = game.MaxTurns - game.Turn;
            //groups
            GameInfo.Groups.allMissions = MissionManager.AllMissions();
            //bonus
            GameInfo.Bonus.bonusIceberg = game.GetBonusIceberg();
            GameInfo.Bonus.bonusCycle = game.BonusIcebergMaxTurnsToBonus;
            GameInfo.Bonus.bonusAmount = game.BonusIcebergPenguinBonus;
            GameInfo.Bonus.turnsLeftToBonus = game.GetBonusIceberg().TurnsLeftToBonus;
            //bridges
            GameInfo.Bridge.bridgeCost = game.IcebergBridgeCost;
            GameInfo.Bridge.bridgeSpeed = game.IcebergBridgeSpeedMultiplier;
            GameInfo.Bridge.bridgeDuration = game.IcebergMaxBridgeDuration;
        }
    }
}
