using PenguinGame;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MyBot {
   public static class Defensive
   {
       public static Iceberg GetWall(Game game)
       {
           var distances = new List<(Iceberg,double)>();
           foreach(var myIceberg in game.GetMyIcebergs())
           {
               distances.Add((myIceberg,Utils.AverageDistanceFromEnemy(game,myIceberg)));
           }

           distances.Sort((x,y)=>x.Item2.CompareTo(y.Item2));
           foreach(var k in distances)
           {
               System.Console.WriteLine($"ice {k.Item1} dis {k.Item2}");
           }
           return distances[0].Item1;
       }

       
   }

}
    
        