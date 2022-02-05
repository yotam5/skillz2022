using PenguinGame;

namespace MyBot {
    /// <summary>
    /// This is an example for a bot.
    /// </summary>
    
    //lvl2 21
    //lvl3 28
    //lvl4 35?

    //! add in real time reinforcment!
    //TODO: decoy attacks and to detect them
    //TODO: track info about how the player plays in this bot and act accordingly
    //TODO: prevent enemy icebergs from upgrading icebergs by in sea collision or attacking
    //TODO: check if its smart and safe to upgrade iceberg
    //TODO: check when to evacuate iceberg
    //TODO: send penguins to icebergs for an upgrade
    //TODO: add in sea collision ?
    //TODO: improved attacking also add duel attacking at the same time
    //TODO: find what iceberg is liklyto upgrade and attack
    //! fix upgrading system its pretty broken
    public class TutorialBot : ISkillzBot {
        /// <summary>
        /// Makes the bot run a single turn.
        /// </summary>
        /// <param name="game">the current game state</param>
        
        public void DoTurn (Game game) {
            double t1 = game.GetTimeRemaining();
            GameLogic.execute(game);
            System.Console.WriteLine($"The run took: {t1 - game.GetTimeRemaining()}");

        }
    }
}
    
        