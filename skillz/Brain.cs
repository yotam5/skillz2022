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
            else
            {
                Defensive.DefenseMehcanisem(resourceManager);
                Expand.ConqureNeutrals(resourceManager);
                Offensive.QuickAttack(resourceManager);
            }


        }

    }
}