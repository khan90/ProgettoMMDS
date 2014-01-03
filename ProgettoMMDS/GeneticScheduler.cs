using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProgettoMMDS
{
    class GeneticScheduler : AbstractScheduler
    {
        int populationCount = 80;
        List<Schedule> population;
        Schedule schedule;
        int bestTardiness;
        int validi = 0;
        int iterazioni = 0;

        public GeneticScheduler()
        {
            population = new List<Schedule>();
        }
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
                genetic(fm.getNumberofMachine(), fm.getNumberofJobs());

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
                Console.WriteLine("Validi: " + validi + " su " + iterazioni);
                Console.ReadKey();
            }
        }

        void genetic(int m, int n)
        {
            Thread thread = new Thread(new ThreadStart(timer));
            thread.Start();
            schedule = new Schedule(jobs);
            schedule.constructScheduleEDD(m,n);
            bestTardiness = schedule.getTardiness();
            for (int i = 0; i < populationCount; i++)
            {
                population.Add(new Schedule(jobs));               
            }
            Parallel.For(0, populationCount, i => generateInitialSolution(i));
            while (!fine)
            {
                population.Sort((x, y) => x.getTardiness().CompareTo(y.getTardiness()));
                int currentTardiness = population[0].getTardiness();
                //Console.WriteLine(currentTardiness);
                if (currentTardiness < bestTardiness)
                {
                    schedule = new Schedule(population[0]);
                    bestTardiness = currentTardiness;
                }
                Parallel.For(0, populationCount/2, i => combineSolution(i,(populationCount/2)-i));
                Parallel.For(0, populationCount, i => mutateSolution(i));
            }
        }

        private void generateInitialSolution(int i)
        {
            lock (population[i])
            {
                population[i] = LocalSearchBestInsert(schedule);
            }
        }

        private void combineSolution(int first, int second)
        {
            Random r = new Random();
            //Order Crossover
            //Genero i due punti di taglio
            int num1 = r.Next(schedule.Count());
            int num2 = r.Next(schedule.Count());
            if (num2 < num1)
            {
                int temp = num1;
                num1 = num2;
                num2 = temp;
            }
            Schedule currentSchedule = new Schedule(population[first]);
            Schedule firstSchedule = population[first];
            Schedule secondSchedule = population[second];
            //QUI devo fare il crossover
            for (int i = num1; i <= num2; i++)
            {
                currentSchedule.schedule[i] = secondSchedule.schedule[i];
            }
            currentSchedule = LocalSearchBestInsert(currentSchedule);
            bool valido = true;
            for (int i = 0; i < currentSchedule.Count()-1; i++)
            {
                for (int j = i+1; j < currentSchedule.Count(); j++)
                {
                    if (currentSchedule.schedule[i] == currentSchedule.schedule[j])
                        valido = false;
                }
            }
            Interlocked.Increment(ref iterazioni);
            if (valido)
            {
                Interlocked.Increment(ref validi);
                lock (population[populationCount / 2 + first])
                {
                    population[populationCount - first - 1] = currentSchedule;
                }
            }
        }

        private void mutateSolution(int index)
        {
            Random r = new Random();
            Schedule currentSchedule = population[index];
            if (r.Next(100) < 20)
            {
                for (int i = 0; i < currentSchedule.Count(); i++)
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
            }
        }



    }
}
