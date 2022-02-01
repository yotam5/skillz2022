using System.Collections.Generic;
using System.Text;
using PenguinGame;
using System.Linq;

namespace MyBot
{
    /*
    deals with offensive yandres
    */
    public static class Offensive
    {

        public static void QuickAttack(ResourceManager resourceManager)
        {
            var myIcebergs = resourceManager.GetMyIcebergs().ToList();
            var enemyIcebergs = resourceManager.GetEnemyIcebergs().ToList();
            foreach(var c in myIcebergs)
            {
                enemyIcebergs.Sort((u1,u2)=>
                    (u1.GetTurnsTillArrival(c)).CompareTo(u2.GetTurnsTillArrival(c)));
                foreach(var k in enemyIcebergs){
                    int enemyAmount = k.PenguinAmount;
                    int distance = k.GetTurnsTillArrival(c);
                    enemyAmount+=distance * k.PenguinsPerTurn;
                    enemyAmount += 1;
                    if(c.PenguinAmount > enemyAmount)
                    {
                        if(c.CanSendPenguins(k,enemyAmount) && Defensive.RiskEvaluation(resourceManager,c,additionalAmount: -enemyAmount) > 0)
                        {
                            c.SendPenguins(k,enemyAmount);
                        }
                    }
                }
                
            }
        }
        public static void SmartAttack(ResourceManager resourceManager, double fa = 1.8, double lf = 2.4)
        {
            var enemyIcebergs = resourceManager.GetEnemyIcebergs().ToList();
            var myIcebergs = resourceManager.GetMyIcebergs().ToList();
            enemyIcebergs.Sort((u1, u2) =>
                (u1.PenguinAmount * fa + u1.Level * lf).CompareTo(u2.PenguinAmount * fa + u2.Level * lf)
            );
            var choosen = enemyIcebergs[0];
            int minimumToTakeOver = choosen.PenguinAmount + 4;
            var tk = new List<(SmartIceberg, SmartIceberg, int)>();

            foreach (var p in myIcebergs)
            {
                int startingAmount = 0;
                int safeToSend = Defensive.RiskEvaluation(resourceManager, p);
                System.Console.WriteLine($"safetosend is {safeToSend}");
                if (p.upgraded)
                {
                    System.Console.WriteLine("ice already did upgrde can send");
                    continue;
                }
                if (safeToSend < 0) //FIX?
                {
                    continue;
                }
            System.Console.WriteLine('F');

                while (Defensive.RiskEvaluation(resourceManager, p, additionalAmount: -startingAmount) > 0 && startingAmount < minimumToTakeOver)
                {
                    startingAmount++;
                }
                            System.Console.WriteLine('G');

                System.Console.WriteLine($"min achived is {startingAmount}");
                tk.Add((p, choosen, startingAmount));
                minimumToTakeOver -= startingAmount;
            }
            if (minimumToTakeOver <= 0)
            {
                foreach (var c in tk)
                {
                    System.Console.WriteLine($"c is {c.Item2.Id}");
                    System.Console.WriteLine($"iceberg sending {c.Item1.Id}");
                    c.Item1.SendPenguins(c.Item2, c.Item3);
                }
            }
        }
    }
}