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
            //(new Scheduler()).run(args);
        }
       
    }
}
