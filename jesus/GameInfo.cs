using PenguinGame;
using System.Collections.Generic;
using System.Linq;

namespace MyBot
{
    public static class GameInfo
    {
        public static class Icebergs
        {
            public static List<SmartIceberg> myIcebergs = new List<SmartIceberg>();
            public static List<SmartIceberg> enemyIcebergs= new List<SmartIceberg>();
            public static List<SmartIceberg> neutralIcebergs= new List<SmartIceberg>();
            public static List<SmartIceberg> allIcebergs= new List<SmartIceberg>();
            public static List<SmartIceberg> notMyIcebergs= new List<SmartIceberg>();
            public static List<SmartIceberg> threatenedIcebergs= new List<SmartIceberg>();
        }

        public static class PenguinGroups
        {
            public static List<PenguinGroup> myPenguinGroups = new List<PenguinGroup>();
            public static List<PenguinGroup> enemyPenguinGroups= new List<PenguinGroup>();
            public static List<PenguinGroup> allPenguinGroup= new List<PenguinGroup>();
        }
        public static class Game
        {
            public static int turn;
            public static int maxTurns;
            public static int turnsLeft;
        }

        public static class Players
        {
            public static List<Player> allPlayers = new List<Player>();
            public static Player enemyPlayer = new Player();
            public static Player mySelf = new Player();
            public static Player neutral = new Player();
        }

        public static class Groups
        {
            public static HashSet<IMission> allMissions = new HashSet<IMission>();
            public static HashSet<HashSet<IMission>> allMissionGroup = new HashSet<HashSet<IMission>>();
        }

    }
}