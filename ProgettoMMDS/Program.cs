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
            if ((args.Length == 0)||(args[0] == "-i"))
            {
                Console.WriteLine("Scheduler.exe <istanza.dat> <tempo in secondi> <run>");
                return;
            }
            AbstractScheduler scheduler = new GeneticScheduler(10000);
            scheduler.run(args);

            //scheduler = new GeneticSchedulerMono(10000);
            //scheduler.run(args);
            //(new FileManager()).appendPopulationCount(-1);
            //(new Scheduler()).run(args);
        }
       
    }
}
