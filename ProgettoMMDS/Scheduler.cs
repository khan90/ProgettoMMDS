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
    class Scheduler : AbstractScheduler
    {
        public override void run(string[] args)
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
                //INIZIO CONTEGGIO SECONDI
                DateTime startTime = DateTime.Now;

                //QUI l'algoritmo di ricerca del minimo ->
                Schedule schedule = SearchSolutionMultistart(fm.getNumberofMachine(), fm.getNumberofJobs());               

                //FINE CONTEGGIO SECONDI
                DateTime stopTime = DateTime.Now;
                TimeSpan elapsedTime = stopTime.Subtract(startTime);
                //Stampa Tardiness e tempo totale
                Console.WriteLine("Tardiness totale: " + schedule.getTardiness().ToString());
                Console.WriteLine("Arrivato in " + elapsedTime.TotalMilliseconds + " ms");
                
                //OUTPUT
                fm.OutputSolution(schedule.schedule);
                fm.OutputResult(schedule.getTardiness(), elapsedTime.TotalMilliseconds);
                fm.OutputProva(schedule.schedule, schedule.getTardiness(), elapsedTime.TotalMilliseconds, "Prova");               
                //Console.ReadKey();
            }
        }
        /// <summary>
        /// Ricerca Locale multistart
        /// Multistart
        /// </summary>
        /// <param name="m">Numero di macchine</param>
        /// <param name="n">Numero di Job</param>
        /// <returns>Miglior schedule trovato</returns>
        Schedule SearchSolutionMultistart(int m, int n)
        {
            Thread thread = new Thread(new ThreadStart(timer));
            thread.Start();
            //soluzione migliore -> Il metodo per ora fa solo una ricerca locale a partire dall'EDD
            Schedule schedule = new Schedule(jobs);
            schedule.constructScheduleEDD(m,n);
            int bestTardiness = schedule.getTardiness();
            Schedule currentSchedule = new Schedule(schedule);
            int j = 0;
            while (!fine)
            {
                currentSchedule = new Schedule(LocalSearchBestInsert(currentSchedule));
                j++;
                int currentTardiness = currentSchedule.getTardiness(); 
                if (currentTardiness < bestTardiness)
                {
                    schedule = new Schedule(currentSchedule);
                    bestTardiness = currentTardiness;
                }
                Random r = new Random();
                for (int i = 0; i < schedule.Count(); i++)
                {
                    int num1 = r.Next(schedule.Count());
                    int num2 = r.Next(schedule.Count());
                    if ((num1 == num2)||(0 == currentSchedule.schedule[num1])||(0 == currentSchedule.schedule[num2]))
                    {
                        i--;
                        continue;
                    }
                    currentSchedule.swap(num1, num2);
                }
            }
            //Console.WriteLine(j);
            return (schedule);
        }

        void timer()
        {
            //Console.WriteLine("Counter partito");
            Thread.Sleep((int)MTIME);
            fine = true;
        }
       

    }
}
