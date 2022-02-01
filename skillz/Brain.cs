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
        public static void execute(ResourceManager resourceManager)
        {
            System.Console.WriteLine($"Game turn is {resourceManager.Turn}");

            if(resourceManager.Turn == 1){
                resourceManager.GetMyIcebergs()[0].Upgrade();
            }

            if(resourceManager.GetMyIcebergs().Count() < 3 && resourceManager.Turn < 25)
            {
                Expand.ConqureNeutrals(resourceManager);
            }

            else if(resourceManager.CountIcebergLevelMine(2).Count() < 3 && resourceManager.Turn < 60){
                System.Console.WriteLine("nlp22");
                foreach(var c in resourceManager.GetMyIcebergs())
                {
                    System.Console.WriteLine("nlp4");
                    if(Defensive.RiskEvaluation(resourceManager,c,true) > 0)
                    {
                        if(c.CanUpgrade()){
                            c.Upgrade();
                        }
                    }
                    System.Console.WriteLine("nlp5");
                }
                System.Console.WriteLine("nlp");
                Offensive.QuickAttack(resourceManager);
                System.Console.WriteLine("nlp1");
                Expand.ConqureNeutrals(resourceManager);
                System.Console.WriteLine("nlp2");
                Offensive.SmartAttack(resourceManager);
                System.Console.WriteLine("nlp3");
                                

            }
            else
            {
                Defensive.DefenseMehcanisem(resourceManager);
                if(Brain.DeltaPenguinGeneration(resourceManager) < 0)
                {
                    foreach(var c in resourceManager.GetMyIcebergs())
                    {
                        if(Defensive.RiskEvaluation(resourceManager,c,true) > 0)
                        {
                            if(c.CanUpgrade()){
                                c.Upgrade();
                            }
                        }
                    }
                }
                Offensive.QuickAttack(resourceManager);
                Offensive.SmartAttack(resourceManager);
                Expand.ConqureNeutrals(resourceManager);
            }


        }
        public static int DeltaPenguinGeneration(ResourceManager game)
        {
            int enemySum = game.GetEnemyIcebergs().Sum(iceberg=>iceberg.PenguinsPerTurn);
            int mySum = game.GetMyIcebergs().Sum(iceberg=>iceberg.PenguinsPerTurn);
            return mySum - enemySum;
        }

    }
}