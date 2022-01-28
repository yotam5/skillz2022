using PenguinGame;


namespace MyBot {

    /// <summary>
    /// class to handle the game resources
    /// </summary>
    public sealed class ResourceManager
    {
        //private Dictionary<int,SmartIceberg> _myIcebergs;
        private List<SmartIceberg> _myIcebergs;
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
            this._myIcebergs = new List<SmartIceberg>();
            foreach(var iceberg in myIcebergs)
            {
                this._myIcebergs.Add(new SmartIceberg(iceberg));
            }
        }


        public List<SmartIceberg> GetMyIcebergsList()
        {
            return this._myIcebergs;
        }

        public SmartIceberg[] GetMyIcebergsArray()
        {
            return this._myIcebergs.ToArray();
        }

    }

}
    
        