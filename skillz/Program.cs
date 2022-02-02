using PenguinGame;

namespace MyBot {
    /// <summary>
    /// This is an example for a bot.
    /// </summary>
    
    //TODO: track info about how the player plays in this bot and act accordingly
    //TODO: prevent enemy icebergs from upgrading is possibole by in sea collision or attacking
    //TODO: decoy attacks and to detect them
    //TODO: check if its smart and safe to upgrade iceberg
    public class TutorialBot : ISkillzBot {
        /// <summary>
        /// Makes the bot run a single turn.
        /// </summary>
        /// <param name="game">the current game state</param>
        public void DoTurn (Game game) {
            double t1 = game.GetTimeRemaining();
            Brain.execute(game);
            System.Console.WriteLine($"The run took: {t1 - game.GetTimeRemaining()}");

        }
    }
}
    
        