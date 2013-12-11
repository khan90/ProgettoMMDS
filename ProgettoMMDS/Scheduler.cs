using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgettoMMDS
{
    /// <summary>
    /// Classe che effettua lo scheduling
    /// </summary>
    class Scheduler
    {
        List<Job> jobs = new List<Job>();

        public void run(string[] args)
        {


            if (!(args.Length > 0))
            {
                Console.WriteLine("PATH non specificato");
                Console.ReadKey();
                return;
            }
            else
            {
                FileManager fm = new FileManager(args[0]);
                jobs = (fm.getJobsList());
                //Ordinamento qui -> In ordine di DueDate:
                jobs.Sort((x, y) => x.getDueDateTime().CompareTo(y.getDueDateTime()));
                //a questo punto jobs è ordinato in base alla DueDate...
                for (int i = 0; i < jobs.Count; i++)
                {
                    Console.WriteLine(jobs[i].ToString());
                }                
                List<int> schedule = new List<int>(constructScheduleEDD(fm.getNumberofMachine(), fm.getNumberofJobs()));
                /* //Stampa EDD
                for (int i = 0; i < schedule.Count; i++)
                {
                    Console.WriteLine(schedule[i].ToString());
                }*/
                //TODO: SearchSolution(); (la chiamata a DateTime.Now si potrebbe fare con un thread che non fa altro... pensiamoci!
                //Stampa Lateness
                Console.WriteLine("Tardiness totale: " + getTardiness(schedule).ToString());

                Console.ReadKey();
            }
        }

        List<int> SearchSolution(List<int> schedule)
        {
            return schedule;
        }

        public List<int> constructScheduleEDD(int m, int n)
        {
            List<int> schedule = new List<int>();
            int[] time = new int[m];
            int[] index = new int[m];
            for (int i = 0; i < m; i++)
            {
                index[i] = i;
                time[i] = 0;
            }
            for (int i = 0; i < m - 1; i++)
            {
                schedule.Add(0);
            }
            int bestMac = 0;
            for (int i = 0; i < n; i++)
            {
                Job j = jobs[i];
                schedule.Insert(index[bestMac], j.getID());
                for (int k = bestMac; k < m; k++)
                    index[k]++;
                time[bestMac] += j.getProcessingTime();
                for (int k = 0; k < m; k++)
                {
                    if (time[k] < time[bestMac])
                    {
                        bestMac = k;
                    }
                }
            }
            return schedule;
        }
        /// <summary>
        /// Funzione che calcola la Tardiness totale di uno schedule dato
        /// </summary>
        /// <param name="schedule">schedule di cui si vuole calcolare la Tardiness totale</param>
        /// <returns>Tardiness totale</returns>
        public int getTardiness(List<int> schedule)
        {
            int time = 0;
            int tardiness = 0;
            
            foreach(int i in schedule)
            {
                if (i == 0)
                    time = 0;
                else
                {
                    Job j = jobs[i - 1]; 
                    time += j.getProcessingTime();
                    if (time >= j.getDueDateTime())
                    {
                        tardiness += (time - j.getDueDateTime());
                    }
                }
            }
            return (tardiness);
        }
       

    }
}
