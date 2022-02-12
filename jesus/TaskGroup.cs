using PenguinGame;
using System.Collections.Generic;
using System.Linq;

namespace MyBot
{
    public class TaskGroup
    {
        private HashSet<ITask> tasks;

        public TaskGroup()
        {
            this.tasks = new HashSet<ITask>();
        }
        public int GetTotalLoss()
        {
            int total = 0;
            foreach(var task in this.tasks)
            {
                total += task.Loss();
            }
            return total;
        }

        public void AddTask(ITask task)
        {
            this.tasks.Add(task);
        }

        public HashSet<ITask> GetTasks()
        {
            return this.tasks;
        }

        public bool CanBePerformed()
        {
            int canBeUsed = 0;
            int used = 0;
            List<SmartIceberg> upgrades = new List<SmartIceberg>();
            foreach(var task in this.tasks)
            {
                used += task.PenguinsRequired();
                if(task.GetType() == typeof(Upgrade)){
                    upgrades.Add(task.GetActor());
                }
            }

            foreach(var task in this.tasks){
                if(upgrades.Contains(task.GetActor()) && !(task.GetType() == typeof(Upgrade))){
                    return false;
                }
            }
            foreach(var iceberg in this.UsedIcebergs()){
                canBeUsed += iceberg.GetUnusedPenguins();
            }
            return canBeUsed >= used;
        }

        public bool CommonTasks(TaskGroup other) //!need implementation
        {
            foreach(var task in other.GetTasks())
            {
                if(this.tasks.Contains(task))
                {
                    return true;
                }
            }
            return false;
        }

        public List<SmartIceberg> UsedIcebergs()
        {
            var usedIcebergs = new List<SmartIceberg>();
            foreach(var task in this.tasks){
                usedIcebergs.Add(task.GetActor());
            }
            return usedIcebergs;
        }

    }
}