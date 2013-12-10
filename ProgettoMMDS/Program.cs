#define VERBOSE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgettoMMDS
{
    class Program
    {
        static void Main(string[] args)
        {

            if(!(args.Length > 0))
            {
                System.Console.WriteLine("PATH non specificato");
                System.Console.ReadKey();
                return;
            }
            else
            {
                FileManager fm = new FileManager(args[0]);
                List<Job> jobs = fm.getJobsList();
                for(int i=0;i<jobs.Count;i++)
                {
                    System.Console.WriteLine(jobs[i].ToString());
                }

                System.Console.WriteLine("Premi un tasto qualsiasi!!!!");
                System.Console.WriteLine("Premi un tasto qualsiasi!!!!");

                System.Console.ReadKey();
            }


        }
       
    }
}
