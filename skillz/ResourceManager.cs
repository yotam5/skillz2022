using PenguinGame;


namespace MyBot {

    /// <summary>
    /// class to handle the game resources
    /// </summary>
    public class ResourceManager
    {
        //private Dictionary<int,SmartIceberg> _myIcebergs;
        private SmartIceberg[] _myIcebergs;

        private SmartIceberg[] _neutralIcebergs;
        private SmartIceberg[] _enemyIcebergs;
        private Game _gameHandler{get;}

        public int Turn
        {
            get{return this._gameHandler.Turn;}
        }

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
            this._myIcebergs = new SmartIceberg[myIcebergs.Length];
            int index = 0;
            
            foreach(var iceberg in myIcebergs)
            {
                this._myIcebergs[index++] = new SmartIceberg(iceberg);
            }
            
            index = 0;
            var neutralIceberg = this._gameHandler.GetNeutralIcebergs();
            this._neutralIcebergs = new SmartIceberg[neutralIceberg.Length];
            
            foreach(var iceberg in neutralIceberg)
            {
                this._neutralIcebergs[index++] = new SmartIceberg(iceberg);
            }
            
            index = 0;

            var enemyIcebergs = this._gameHandler.GetEnemyIcebergs();
            this._enemyIcebergs = new SmartIceberg[enemyIcebergs.Length];
            foreach(var iceberg in enemyIcebergs)
            {
                this._enemyIcebergs[index++] = new SmartIceberg(iceberg);
            }
            index = 0;
        }

        public PenguinGroup[] GetEnemyPenguinGroups()
        {
            return this._gameHandler.GetEnemyPenguinGroups();
        }

        public PenguinGroup[] GetMyPenguinGroups()
        {
            return this._gameHandler.GetMyPenguinGroups();
        }


        public SmartIceberg[] GetNeutralIcebergs()
        {
            return this._neutralIcebergs;
        }
        public SmartIceberg[] GetEnemyIcebergs()
        {
            return this._enemyIcebergs;
        }


        public SmartIceberg[] GetMyIcebergs()
        {
            return this._myIcebergs;
        }

    }

}
    
        