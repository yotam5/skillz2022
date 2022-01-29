using PenguinGame;

namespace MyBot {
    /// <summary>
    /// This is an example for a bot.
    /// </summary>
    
    public class TutorialBot : ISkillzBot {
        /// <summary>
        /// Makes the bot run a single turn.
        /// </summary>
        /// <param name="game">the current game state</param>
        public void DoTurn (Game game) {
            var resourceManager = new ResourceManager(game);
            //var n = game.GetMyIcebergs()[0];
            //System.Console.WriteLine($"{n.Id}={n.UniqueId}={n.GetHashCode()}");

            /*foreach(var c in Defensive.GetMyAttackedIcebergs(resourceManager))
            {
                Defensive.RiskEvaluation(resourceManager,c);
            }*/
            Brain.execute(resourceManager);
        }
    }
}
    
        