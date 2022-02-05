using PenguinGame;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MyBot
{
    public static class Offensive
    {
        public static void Attack(Game game)
        {
            var enemyIcebergs = game.GetEnemyIcebergs().ToList();
            game.GetNeutralIcebergs().ToList().ForEach(x=>enemyIcebergs.Add(x));
            var closest = enemyIcebergs.OrderBy(x=>Utils.AverageDistanceFromMyIcbergs(game,x)).First();
            System.Console.WriteLine($"selected ice to attack is {closest}");
            foreach(var myIce in game.GetMyIcebergs())
            {
                int turnsTillArrival = myIce.GetTurnsTillArrival(closest);
                int enemyAmountAtArrival = Utils.EnemyPenguinAmountAtArrival(game,closest,turnsTillArrival)  +1;
                int deltaPenguins = myIce.PenguinAmount - enemyAmountAtArrival;
                System.Console.WriteLine($"delta {deltaPenguins} enemyarrival {enemyAmountAtArrival} turns is {turnsTillArrival}");
                if(deltaPenguins > 1)
                {
                    System.Console.WriteLine($"iceberg {myIce} amount {myIce.PenguinAmount} acted {myIce.AlreadyActed} send {enemyAmountAtArrival}");
                    if(myIce.CanSendPenguins(closest,enemyAmountAtArrival))
                    {
                        System.Console.WriteLine("sentttt");
                        myIce.SendPenguins(closest,enemyAmountAtArrival);
                        break;
                    }

                }
            }
        }
    }
}