using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProgettoMMDS
{
    /// <summary>
    /// Classe che effettua lo scheduling
    /// </summary>
    class Scheduler
    {
        List<Job> jobs = new List<Job>();
        static volatile bool fine = false;
        static long MTIME = 10000;
 
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
                //INIZIO CONTEGGIO SECONDI
                DateTime startTime = DateTime.Now;
                jobs = (fm.getJobsList());

                
                /* for (int i = 0; i < jobs.Count; i++)
                {
                    Console.WriteLine(jobs[i].ToString());
                }*/
                
                List<int> schedule = new List<int>(constructScheduleEDD(fm.getNumberofMachine(), fm.getNumberofJobs()));

                /*//Stampa EDD
                for (int i = 0; i < schedule.Count; i++)
                {
                    Console.WriteLine(schedule[i].ToString());
                }

                Console.WriteLine("Tardiness totale: " + getTardiness(schedule).ToString());
                //*/


                //TODO: SearchSolution(); (la chiamata a DateTime.Now si potrebbe fare con un thread che non fa altro... pensiamoci!
                schedule = SearchSolutionRandom(schedule);
                //Stampa Tardiness
                Console.WriteLine("Tardiness totale: " + getTardiness(schedule).ToString());
                

                //FINE CONTEGGIO SECONDI
                DateTime stopTime = DateTime.Now;
                TimeSpan elapsedTime = stopTime.Subtract(startTime);
                Console.WriteLine("Arrivato in " + elapsedTime.TotalMilliseconds + " ms");
                
                //OUTPUT

                fm.OutputSolution(schedule);
                fm.OutputResult(getTardiness(schedule), elapsedTime.TotalMilliseconds);
                fm.OutputProva(schedule, getTardiness(schedule), elapsedTime.TotalMilliseconds, "Prova");
               
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Ricerca locale di un minimo effettuando swap Random tra gli elementi.
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        List<int> SearchSolutionRandom(List<int> schedule)
        {
            Random r = new Random();
            int maxInt = schedule.Count();
            int bestTardy = getTardiness(schedule);
            Thread thread = new Thread(new ThreadStart(timer));
            thread.Start();
            List<int> currentSchedule = new List<int>(schedule);
            while (!fine)
            {                
                int num1 = r.Next(maxInt);
                int num2 = r.Next(maxInt);
                if (num2 == num1)
                    continue;
                int temp = currentSchedule[num1];
                currentSchedule[num1] = currentSchedule[num2];
                currentSchedule[num2] = temp;
                int currentTardy = getTardiness(currentSchedule);
                if (currentTardy < bestTardy)
                {
                    bestTardy = currentTardy;
                    schedule = new List<int>(currentSchedule);
                }
                else
                {
                    temp = currentSchedule[num1];
                    currentSchedule[num1] = currentSchedule[num2];
                    currentSchedule[num2] = temp;
                }
                
            }
            return schedule;
        }

        /// <summary>
        /// Costruisce uno Schedule EDD prendendo in input macchine e la lista dei job  
        /// </summary>
        /// <param name="m">Numero Macchine</param>
        /// <param name="n">Numero Job</param>
        /// <returns></returns>
        public List<int> constructScheduleEDD(int m, int n)
        {
            //Ordinamento qui -> In ordine di DueDate:
            jobs.Sort((x, y) => x.getDueDateTime().CompareTo(y.getDueDateTime()));
            //a questo punto jobs è ordinato in base alla DueDate...

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
            //Rimetto il vettore dei job in ordine
            jobs.Sort((x, y) => x.getID().CompareTo(y.getID()));
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
                    if (time > j.getDueDateTime())
                    {
                        tardiness += (time - j.getDueDateTime());
                    }
                }
            }
            return (tardiness);
        }

        public static void timer()
        {
            //Console.WriteLine("Counter partito");
            Thread.Sleep((int)MTIME);
            fine = true;
        }
       

    }
}
