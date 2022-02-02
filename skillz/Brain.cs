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
            else if(game.Turn == 19)
            {
                var c = new Iceberg();
                foreach(var nc in game.GetNeutralIcebergs()){
                    if(nc.Id == 7){
                        c = nc;
                    }
                }
                game.GetMyIcebergs()[0].SendPenguins(c, 13);
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
                var defended = Defensive.DefendeIcebergs(game);
                var bestMove = Offensive.BestCombination(game);
                if(bestMove.Item3 != -999 && !bestMove.Item1.AlreadyActed){
                    bestMove.Item1.SendPenguins(bestMove.Item2,Offensive.EnemyPenguinsAtArrival(game,bestMove.Item1,bestMove.Item2) + 1);
                }
                var middleIceberg = Offensive.MiddleIceberg(game);
                System.Console.WriteLine($"MiddleIce is {middleIceberg}");
                foreach(var ice in game.GetMyIcebergs()){
                    if(!ice.Equals(middleIceberg) && !ice.AlreadyActed && Defensive.GetAttackingGroups(game,ice,enemy: true).Count() == 0){
                        if(ice.CanSendPenguins(middleIceberg,middleIceberg.PenguinsPerTurn))
                        ice.SendPenguins(middleIceberg,middleIceberg.PenguinsPerTurn);
                    }
                }
                foreach(var ice in game.GetMyIcebergs()) //TODO: check if safe to upgrade from near icebergs of the enemy or maxflow
                {
                    if(Defensive.GetAttackingGroups(game,ice).Count() == 0 && ice.CanUpgrade() && !ice.AlreadyActed)
                    {
                        ice.Upgrade();
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



    }
}