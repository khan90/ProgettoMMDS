#define VERBOSE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProgettoMMDS
{
    class Program
    {
        static List<Job> jobs = new List<Job>();
        static void Main(string[] args)
        {
            AbstractScheduler scheduler = new GeneticScheduler(10000);
            scheduler.run(args);

            //Le formiche non vanno perchè ci ho aggiunto la macchina -1....
            //(new GreedyAntsScheduler()).run(args);
            //(new Scheduler()).run(args);
        }
       
    }
}
