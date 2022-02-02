using PenguinGame;

namespace MyBot {
    /// <summary>
    /// This is an example for a bot.
    /// </summary>
    
    //TODO: track info about how the player plays in this bot and act accordingly
    //TODO: prevent enemy icebergs from upgrading icebergs by in sea collision or attacking
    //TODO: decoy attacks and to detect them
    //TODO: check if its smart and safe to upgrade iceberg
    //TODO: check when to evacuate iceberg
    //TODO: send penguins to icebergs for an upgrade
    //TODO: add in sea collision ?
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
    
        