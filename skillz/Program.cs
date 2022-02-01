using PenguinGame;

namespace MyBot {
    /// <summary>
    /// This is an example for a bot.
    /// </summary>
    
    //TODO: track info about how the player plays in this bot and act accordingly
    public class TutorialBot : ISkillzBot {
        /// <summary>
        /// Makes the bot run a single turn.
        /// </summary>
        /// <param name="game">the current game state</param>
        public void DoTurn (Game game) {
            Brain.execute(game);
            game.Debug($"i is {i++}");
        }
        public static int i = 0;
    }
}
    
        