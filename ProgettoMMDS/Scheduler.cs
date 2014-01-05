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
        Schedule schedule;
        int bestTardiness;
        int j = 0;
        int parallelThread = 4;
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
                //Schedule schedule = SearchSolutionMultistart(fm.getNumberofMachine(), fm.getNumberofJobs());
                SearchSolutionMultistartParallel(fm.getNumberofMachine(), fm.getNumberofJobs());

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
                Console.WriteLine("Iterazioni: " + j);
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Ricerca Locale multistart
        /// Multistart
        /// </summary>
        /// <param name="m">Numero di macchine</param>
        /// <param name="n">Numero di Job</param>
        /// <returns>Miglior schedule trovato</returns>
        void SearchSolutionMultistart(int m, int n)
        {
            Thread thread = new Thread(new ThreadStart(timer));
            thread.Start();
            //soluzione migliore -> Il metodo per ora fa solo una ricerca locale a partire dall'EDD
            schedule = new Schedule(jobs);
            schedule.constructScheduleEDD(m,n);
            bestTardiness = schedule.getTardiness();

            while (!fine)
            {
                Thread[] array = new Thread[1];
                for (int i = 0; i < array.Length; i++)
                {
                    // Start the thread with a ThreadStart.
                    array[i] = new Thread(new ThreadStart(Start));
                    array[i].Start();
                }
                //array[0].Join();
            }
            //Console.WriteLine(j);
            //return (schedule);
        }

        Schedule SearchSolutionMultistartParallel(int m, int n)
        {
            Thread thread = new Thread(new ThreadStart(timer));
            thread.Start();
            //soluzione migliore -> Il metodo per ora fa solo una ricerca locale a partire dall'EDD
            schedule = new Schedule(jobs);
            schedule.constructScheduleEDD(m,n);
            bestTardiness = schedule.getTardiness();
            while (!fine)
            {
                //Parallel.Invoke(() => Start(), () => Start());
                Parallel.For(0, parallelThread, i => Start());
                //array[0].Join();
            }
            //Console.WriteLine(j);
            return (schedule);
        }

        
        public void Start()
        {
            Schedule currentSchedule;
            Interlocked.Increment(ref j);
            lock (schedule)
            {
                 currentSchedule = new Schedule(schedule);
            }
            //Tiro un calcio alla soluzione
            Random r = new Random();
            for (int i = 0; i < currentSchedule.Count()/8; i++)
            {
                int num1 = r.Next(currentSchedule.Count());
                int num2 = r.Next(currentSchedule.Count());
                if ((num1 == num2) || (0 == currentSchedule.schedule[num1]) || (0 == currentSchedule.schedule[num2]))
                {
                    i--;
                    continue;
                }
                currentSchedule.swap(num1, num2);
            }
            currentSchedule = new Schedule(LocalSearchBestInsert(currentSchedule));
            int currentTardiness = currentSchedule.getTardiness();
            lock (schedule)
            {
                if (currentTardiness < bestTardiness)
                {
                    schedule = new Schedule(currentSchedule);
                    bestTardiness = currentTardiness;
                }
            }
        }
    }
}
