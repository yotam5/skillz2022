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
            InitializeGameInfo(game);
            Utils.ConfigureIcebergs();

            var m = MissionManager.AllMissions().OrderBy(x=>x.Benefit()).Reverse().ToList();
            for(int i = 0; i < m.Count() && i < 2;i++)
            {
                var nn = m[i];
                System.Console.WriteLine(nn);
                var tmp = nn.GetExecutionWays();
                foreach(var w in tmp)
                {
                    if(w.CanBePerformed()){
                        System.Console.WriteLine("performe" + w);
                        var t2 = w.GetTasks();
                        foreach(var t3 in t2){
                            t3.Performe();
                        }
                        break;
                    }
                }
            }
            
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
            GameInfo.PenguinGroups.enemyPenguinGroups = game.GetEnemyPenguinGroups().ToList();
            GameInfo.PenguinGroups.allPenguinGroup = game.GetAllPenguinGroups().ToList();
            //GameInfo.Icebergs.threatenedIcebergs = Utils.ConvertToSmartIceberg(G)
            GameInfo.Game.maxTurns = game.MaxTurns;
            GameInfo.Game.turn = game.Turn;
            GameInfo.Game.turnsLeft = game.MaxTurns - game.Turn;
            //groups
            GameInfo.Groups.allMissions = MissionManager.AllMissions();

        }
    }
}
