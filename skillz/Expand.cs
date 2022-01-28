using System.Collections.Generic;
using System.Text;
using PenguinGame;
using System.Linq;

namespace MyBot
{   
    /*
    deals with expansion, neutrals island for now
    */
    public static class Expand
    {   
        //return list of the closest neutrals islands
        //check the distance in reverse but its the same (source -> dest = dest -> source)
        public static List<Iceberg>GetClosestNeutral(Game game, Iceberg source_iceberg){
                return game.GetNeutralIcebergs().OrderBy(dest=>dest.GetTurnsTillArrival(source_iceberg)).ToList();
        }

        //check if its safe to send the specific amount of penguins with that the
        //iceberg wont be conqured
        //TODO: need to make it more complex, what if enemy iceberg are nearby
        public static bool SafeToSend(Game game,Iceberg source, int amountToSend)
        {  
            var AttackingGroups = Offensive.GetAttackingGroups(game, source,true);
            int GenerationRate = source.PenguinsPerTurn;
            
            int MyIcebergCounter = source.PenguinAmount - amountToSend;
            
            foreach(var attackingGroup in AttackingGroups){
                MyIcebergCounter += source.PenguinsPerTurn * attackingGroup.TurnsTillArrival;
                MyIcebergCounter -= attackingGroup.PenguinAmount;
                if(MyIcebergCounter <= 0){
                    return false;
                }

            }
            return true;
        }

        //TODO: funciton that check if can send to multiple neutrals icebergs
        public static List<Tuple<Iceberg,Iceberg,int>> ConqureNeutrals(Game game){
            var myIcebergs = game.GetMyIcebergs();
            var neutrals = game.GetNeutralIcebergs();
            var GetMyPenguinGroups = game.GetMyPenguinGroups();

            
        }

    }
}