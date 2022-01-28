using PenguinGame;

namespace MyBot {

    /// <summary>
    /// class to handle the game resources
    /// </summary>
    public class ResourceManager
    {
        private Dictionary<int,Iceberg> _myIcebergs;
        private Game _gameHandler{get;}

        //TODO: update data when sending from iceberg

        /// <summary>
        /// resource manager constructor
        /// </summary>
        /// <param name="game">game handler</param>
        public ResourceManager(Game game)
        {
            this._gameHandler = game;
            
            //hash map of my icebergs
            this._myIcebergs = this._gameHandler.GetMyIcebergs().ToDictionary(iceberg=>iceberg.UniqueId,Iceberg=>Iceberg);
            Iceberg[] myIcebergs = game.GetMyIcebergs();
            Iceberg[] neutralsIcbergs = game.GetNeutralIcebergs();
            Iceberg[] enemyIcbergs = game.GetEnemyIcebergs();
            PenguinGroup[] enemyPenguinsGroups = game.GetEnemyPenguinGroups();
            PenguinGroup[] myPenguinsGroup = game.GetMyPenguinGroups();
        }
    }

}
    
        