using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgettoMMDS
{
    class GreedyAntsScheduler : AbstractScheduler
    {
        List<Job> jobs = new List<Job>();
        volatile static bool fine = false;
        static long MTIME = 1000;
        Schedule schedule;

        //Matrice della traccia di ferormoni:
        double[,] traceMatrix;

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
                Schedule schedule = greedyAnts(fm.getNumberofMachine(), fm.getNumberofJobs());

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
                Console.ReadKey();
            }
        }

        private Schedule greedyAnts(int m, int n)
        {
            schedule = new Schedule(jobs);
            schedule.constructScheduleEDD(m, n);
            //Inizializzazione matrice delle traccie
            traceMatrix = new double[schedule.Count(), schedule.Count()];
            for (int i = 0; i < schedule.Count(); i++)
            {                
                for (int j = 0; j < schedule.Count(); j++)
                {
                    traceMatrix[i, j] = 50;
                }
            }
            int iteration = 0;
            while (!fine)
            {
                constructGreedysolutions(iteration);
            }
            return schedule;
        }

        private void constructGreedysolutions(int i)
        {
            //NUMERO DI FORMICHE
            int ants = 10;
            Stack<Schedule> solutionList = new Stack<Schedule>();
            //faccio partire le formiche
            for (int j = 0; j < ants; j++)
            {
                solutionList.Push(antRun(i));
            }
            int bestTardiness = schedule.getTardiness();
            
            while (solutionList.Count != 0)
            {
                Schedule currentSchedule = solutionList.Pop();
                if (currentSchedule.getTardiness() < bestTardiness)
                {
                    schedule = new Schedule(currentSchedule);
                    int currentTardiness = currentSchedule.getTardiness();
                    if (currentTardiness < bestTardiness)
                    {
                        schedule = new Schedule(currentSchedule);
                        bestTardiness = currentTardiness;
                    }
                }
            }            
        }

        private Schedule antRun(int i)
        {
            throw new NotImplementedException();
        }
    }
}
