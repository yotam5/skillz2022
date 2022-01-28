using PenguinGame;


namespace MyBot {

    /// <summary>
    /// class to handle the game resources
    /// </summary>
    public class ResourceManager
    {
        private Dictionary<int,SmartIceberg> _myIcebergs;
        private Game _gameHandler{get;}

        //TODO: update data when sending from iceberg?

        /// <summary>
        /// resource manager constructor
        /// </summary>
        /// <param name="game">game handler</param>
        public ResourceManager(Game game)
        {
            this._gameHandler = game;
            
            //hash map of my icebergs
            var myIcebergs = this._gameHandler.GetMyIcebergs();
            this._myIcebergs = new Dictionary<int, SmartIceberg>();
            foreach(var iceberg in myIcebergs)
            {
                this._myIcebergs.Add(iceberg.UniqueId,new SmartIceberg(iceberg));
            }
        }

    }

}
    
        