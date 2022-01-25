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
            // Go over all of my icebergs.
            Iceberg[] myIcebergs = game.GetMyIcebergs();
            Iceberg[] neutralsIcbergs = game.GetNeutralIcebergs();

            foreach(var myIceberg in myIcebergs) {
                // The iceberg we are going over.

                // The amount of penguins in my iceberg.
                int myPenguinAmount = myIceberg.PenguinAmount;

                // Initializing the iceberg we want to send penguins to.
                Iceberg destination = null;

                // If there are any neutral icebergs.


                if (neutralsIcbergs.Length > 0) 
                {
                    // Target a neutral iceberg.
                    destination = Expand.GetClosestNeutral(game,game.GetMyIcebergs()[0],neutralsIcbergs)[0];
                } 
                else 
                {
                    // Target an enemy iceberg.
                    destination = game.GetEnemyIcebergs()[0];
                }
                int destinationPenguinAmount = destination.PenguinAmount;
                // If my iceberg has more penguins than the target iceberg.
                if (myPenguinAmount > destinationPenguinAmount) 
                {
                    // Send penguins to the target.
                    // In order to take control of the iceberg we need to send one penguin more than is currently in the iceberg.
                    System.Console.WriteLine(myIceberg + " sends "+ (destinationPenguinAmount + 1) + " penguins to " + destination);
                    myIceberg.SendPenguins (destination, destinationPenguinAmount + 1);
                }
            }
        }
    }
}
    
        