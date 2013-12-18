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
            //Scheduler scheduler = new Scheduler();
            AbstractScheduler scheduler = new GreedyAntsScheduler();
            scheduler.run(args);
        }
       
    }
}
