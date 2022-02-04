using System.Collections.Generic;
using System.Text;
using PenguinGame;
using System.Linq;

namespace MyBot
{
    /*
    deals with moves decisons
    */
    public static class Brain
    {

        /// <summary>
        /// execute brain
        /// </summary>
        /// <param name="game"></param>
        public static void execute(Game game)
        {
            game.Debug($"turn number {game.Turn}\n");
            if(game.Turn == 1)
            {
                game.GetMyIcebergs()[0].Upgrade();
            }
            else if(game.Turn == 7)
            {
                game.GetMyIcebergs()[0].SendPenguins(game.GetNeutralIcebergs()[0],11);
            }
            else if(game.Turn == 12){
                game.GetMyIcebergs()[0].SendPenguins(game.GetNeutralIcebergs()[1],11);
            }

            else if(game.Turn == 22)
            {
                var c = new Iceberg();
                foreach(var nc in game.GetNeutralIcebergs()){
                        c = nc;
                    }
                game.GetMyIcebergs()[1].SendPenguins(c, 5);            
            }
            else if(game.Turn > 22) 
            {
                if(Brain.DeltaPenguinGeneration(game) < 0)
                {
                    foreach(var ice in game.GetMyIcebergs().OrderByDescending(x=>x.Level)) //TODO: upgrade icbergs depending on flow potencial
                    {
                        if(!ice.AlreadyActed && Defensive.HelpIcebergData(game,ice,true).Count() ==0 && ice.CanUpgrade())
                        {
                            ice.Upgrade();
                            break;
                        }
                    }
                }
                var defended = Defensive.DefendeIcebergs(game);
                var bestMove = Offensive.BestCombination(game);
                if(bestMove.Item3 != -999 && !bestMove.Item1.AlreadyActed){
                    System.Console.WriteLine($"best move is {bestMove.Item1} to {bestMove.Item2} amount of {bestMove.Item3}");

                    if(bestMove.Item1.CanSendPenguins(bestMove.Item2,System.Math.Abs(Offensive.EnemyPenguinsAtArrival(game,bestMove.Item1,bestMove.Item2)) + 1)) //! fix  this?
                    {
                        System.Console.WriteLine($"enem at arrival {Offensive.EnemyPenguinsAtArrival(game,bestMove.Item1,bestMove.Item2)}");
                        bestMove.Item1.SendPenguins(bestMove.Item2,System.Math.Abs(Offensive.EnemyPenguinsAtArrival(game,bestMove.Item1,bestMove.Item2)) + 1);            
                    }
                    else
                    {
                        System.Console.WriteLine($"best move failed1");
                    }
                }
                else
                {
                    System.Console.WriteLine($"best move failed2");
                }

                var middleIceberg = Offensive.MiddleIceberg(game);
                System.Console.WriteLine($"MiddleIce is {middleIceberg}");
                
                foreach(var ice in game.GetMyIcebergs()){
                    if(!ice.Equals(middleIceberg) && !ice.AlreadyActed && Defensive.GetAttackingGroups(game,ice,enemy: true).Count() == 0){
                        if(ice.CanSendPenguins(middleIceberg,middleIceberg.PenguinsPerTurn))
                        ice.SendPenguins(middleIceberg,middleIceberg.PenguinsPerTurn);
                    }
                }

            }
        }

        public static int DeltaPenguinGeneration(Game game)
        {
            int enemyRateSum = game.GetEnemyIcebergs().Sum(iceberg=>iceberg.PenguinsPerTurn);
            int myRateSum = game.GetMyIcebergs().Sum(iceberg=>iceberg.PenguinsPerTurn);
            return myRateSum - enemyRateSum;
        }

        
        public static bool SafeToUpgrade(Game game,Iceberg icebergToUpgrade)
        {
            return false;
        }

    }
}