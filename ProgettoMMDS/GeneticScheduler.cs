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
        //60
        //800
        //Ho provato a mettere se ho più tempo una popolazione più grande con meno iterazioni nella local search, se ho meno tempo popolazione piccola e local search più profonda
        static float fattore = ((float)MTIME / 10000);
        int populationCount =(int)(60 + fattore * 200);

        int sogliaMutazione = 20;
        List<Schedule> population;
        Schedule schedule;
        int bestTardiness;
        int iterazioni = 0;

        public GeneticScheduler()
        {
            population = new List<Schedule>();
        }
        /// <summary>
        /// Istanza di uno scheduler che utilizza un algoritmo genetico e un numero di secondi <paramref name="time"> time </paramref>
        /// </summary>
        /// <param name="time">Numero di secondi per l'algoritmo genetico</param>
        public GeneticScheduler(int time)
        {
            population = new List<Schedule>();
            MTIME = time;
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
                if (args.Length > 1)
                {
                    MTIME = int.Parse(args[1]) * 1000;
                }
                if (args.Length > 2)
                {
                    RUN = int.Parse(args[2]);
                }                

                DateTime startTime = DateTime.Now;
                //QUI l'algoritmo di ricerca del minimo ->
                genetic(fm.getNumberofMachine(), fm.getNumberofJobs());
                //ATTENZIONE! AGGIUNGO una ricerca locale alla fine di tutto!                
                schedule = LocalSearchBestInsert(schedule, 100);
                //FINE CONTEGGIO SECONDI
                DateTime stopTime = DateTime.Now;
                TimeSpan elapsedTime = stopTime.Subtract(startTime);
                double timeToWrite = elapsedTime.TotalMilliseconds;
                //Stampa Tardiness e tempo totale
                Console.WriteLine("Tardiness totale: " + schedule.getTardiness().ToString());
                Console.WriteLine("Arrivato in " + timeToWrite + " ms");
                
                //OUTPUT
                fm.OutputSolution(schedule.schedule,RUN);
                fm.OutputResult(schedule.getTardiness(), timeToWrite, RUN);
                //fm.OutputProva(schedule.schedule, schedule.getTardiness(), timeToWrite, "Prova");
                Console.WriteLine("Popolazioni generate: " + iterazioni);
                //Console.ReadKey();
            }
        }

        void genetic(int m, int n)
        {
            Thread timerThread = new Thread(new ThreadStart(timer));
            timerThread.Start();
            Console.WriteLine("Popolazione:" + populationCount);
            schedule = new Schedule(jobs);
            schedule.constructScheduleEDD(m,n);
            bestTardiness = schedule.getTardiness();
            for (int i = 0; i < populationCount; i++)
            {
                population.Add(new Schedule(jobs));               
            }
            //Generazione della popolazione iniziale
            Parallel.For(0, populationCount, i => generateInitialSolution(i));
            while (!fine)
            {
                //Ordino la popolazione in ordine di Tardiness -> Tengo la metà migliore e li accoppio due a due per ottenere altre soluzioni
                population.Sort((x, y) => x.getTardiness().CompareTo(y.getTardiness()));
                int currentTardiness = population[0].getTardiness();
                //Console.WriteLine(currentTardiness);
                if (currentTardiness < bestTardiness)
                {
                    schedule = new Schedule(population[0]);
                    bestTardiness = currentTardiness;
                    if (0 == currentTardiness)
                    {
                        timerThread.Abort();
                        fine = true;
                        return;
                    }
                }
                //Combino le soluzioni migliori
                Parallel.For(0, populationCount / 2 -1, i => combineSolution(i, (populationCount / 2) - i -1));
                //Effettuo una mutazione (probabilistica) su tutta la popolazione --> l'ho messa in cima, ma è uguale
                //Console.WriteLine("Mutazione");
                Parallel.For(1, populationCount, i => mutateSolutionNext(i));
                //Parallel.For(populationCount/2, populationCount, i => mutateSolution(i));
                Interlocked.Increment(ref iterazioni);
            }
        }

        private void generateInitialSolution(int i)
        {
            lock (population[i])
            {
                population[i] = LocalSearchBestInsert(schedule);
                //population[i] = new Schedule(schedule);
            }
        }

        private void combineSolution(int first, int second)
        {
            //Genero il primo figlio che metto nella ultima posizione libera della popolazione
            Schedule currentSchedule = generateChildren(first, second);
            lock (population[populationCount - first - 1])
            {
                population[populationCount - first - 1] = currentSchedule;
            }
            //genero il secondo figlio che metto nella prima casella libera della seconda metà della popolazione.
            currentSchedule = generateChildren2(second, first);           
            lock (population[populationCount / 2 + first])
            {
                population[populationCount / 2 + first] = (currentSchedule);
            }
        }

        /// <summary>
        /// Muta la soluzione facendo uno swap a caso
        /// </summary>
        /// <param name="index">Posizione della soluzione che muta</param>
        private void mutateSolution(int index)
        {
            lock (population[index])
            {
                Random r = new Random();
                Schedule currentSchedule = population[index];
                if (r.Next(100) < sogliaMutazione)
                {
                    int num1 = r.Next(currentSchedule.Count());
                    int num2 = r.Next(currentSchedule.Count());
                    if (!((num1 == num2) || (0 >= currentSchedule.schedule[num1]) || (0 >= currentSchedule.schedule[num2])))
                    {
                        currentSchedule.swap(num1, num2);
                    }

                }
                currentSchedule = LocalSearchBestInsert(currentSchedule);
            }
        }

        /// <summary>
        /// Muta la soluzione facendo uno swap con un elemento vicino
        /// </summary>
        /// <param name="index">Posizione della soluzione che muta</param>
        private void mutateSolutionNext(int index)
        {
            lock (population[index])
            {
                Random r = new Random();
                Schedule currentSchedule = population[index];
                if (r.Next(100) < sogliaMutazione)
                {
                    int num1 = r.Next(currentSchedule.Count()-1);
                                     
                    currentSchedule.swap(num1, num1 + 1);
                    
                }
            }
        }

        /// <summary>
        /// Genero un figlio a partire da due soluzioni
        /// La tecnica utilizzata è un order Crossover
        /// </summary>
        /// <param name="first">Primo genitore</param>
        /// <param name="second">Secondo genitore</param>
        /// <returns>Nuovo figlio</returns>
        private Schedule generateChildren(int first, int second)
        {
            Random r = new Random();
            //Order Crossover
            //Genero i due punti di taglio
            int num1 = r.Next(schedule.Count());
            int num2 = r.Next(schedule.Count());
            if (num1 == num2)
            {
                if (num1 == schedule.Count() - 1)
                    num1--;
                else
                    num2++;
            }
            if (num2 < num1)
            {
                int temp = num1;
                num1 = num2;
                num2 = temp;
            }
            //Nello schedule figlio nel taglio metto gli elmenti di first e fuori second
            Schedule currentSchedule = new Schedule(population[first]);
            Schedule firstSchedule = population[first];
            Schedule secondSchedule = population[second];
            //bit vector: se gli elementi di second sono all'interno del taglio di first li segno da eliminare (false)
            bool[] vector = new bool[currentSchedule.Count()];
            for (int j = 0; j < currentSchedule.Count(); j++)
            {
                vector[j] = true;
            }
            //Ordinate Crossover
            for (int i = num1; i <= num2; i++)
            {
                for (int j = 0; j < currentSchedule.Count(); j++)
                {
                    if (currentSchedule.schedule[i] == population[second].schedule[j])
                        vector[j] = false;
                }
            }
            //parte sx
            int currentIndex = 0;
            int secondIndex = 0;
            while ((currentIndex < num1) && (secondIndex < currentSchedule.Count()))
            {
                if (vector[secondIndex])
                {
                    currentSchedule.schedule[currentIndex] = population[second].schedule[secondIndex];
                    currentIndex++;
                    secondIndex++;
                }
                else
                {
                    secondIndex++;
                }
            }
            //parte a dx
            currentIndex = currentSchedule.Count() - 1;
            secondIndex = currentIndex;
            while ((currentIndex > num2)&&(secondIndex >= 0))
            {
                if (vector[secondIndex])
                {
                    currentSchedule.schedule[currentIndex] = population[second].schedule[secondIndex];
                    currentIndex--;
                    secondIndex--;
                }
                else
                {
                    secondIndex--;
                }
            
            }
            currentSchedule = LocalSearchBestInsert(currentSchedule);
            return currentSchedule;
        }
        //Piccola modifica considerando le macchine
        private Schedule generateChildren2(int first, int second)
        {
            Schedule currentSchedule = new Schedule(population[first]);
            Schedule firstSchedule = population[first];
            Schedule secondSchedule = population[second];
            Random r = new Random();
            //Order Crossover
            //Genero i due punti di taglio
            int num1 = r.Next(schedule.Count());
            int num2 = num1;
            for (int i = num1; i < schedule.Count(); i++)
            {
                if ((i == schedule.Count() - 1) || (firstSchedule.schedule[i + 1] <= 0) || (secondSchedule.schedule[i + 1] <= 0))
                {
                    num2 = i;
                }
            }
            if (num1 == num2)
            {
                if (num1 == schedule.Count() - 1)
                    num1--;
                else
                    num2++;
            }

            //Nello schedule figlio nel taglio metto gli elmenti di first e fuori second
            //bit vector: se gli elementi di second sono all'interno del taglio di first li segno da eliminare (false)
            bool[] vector = new bool[currentSchedule.Count()];
            for (int j = 0; j < currentSchedule.Count(); j++)
            {
                vector[j] = true;
            }
            //Ordinate Crossover
            for (int i = num1; i <= num2; i++)
            {
                for (int j = 0; j < currentSchedule.Count(); j++)
                {
                    if (currentSchedule.schedule[i] == population[second].schedule[j])
                        vector[j] = false;
                }
            }
            //parte sx
            int currentIndex = 0;
            int secondIndex = 0;
            while ((currentIndex < num1) && (secondIndex < currentSchedule.Count()))
            {
                if (vector[secondIndex])
                {
                    currentSchedule.schedule[currentIndex] = population[second].schedule[secondIndex];
                    currentIndex++;
                    secondIndex++;
                }
                else
                {
                    secondIndex++;
                }
            }
            //parte a dx
            currentIndex = currentSchedule.Count() - 1;
            secondIndex = currentIndex;
            while ((currentIndex > num2)&&(secondIndex >= 0))
            {
                if (vector[secondIndex])
                {
                    currentSchedule.schedule[currentIndex] = population[second].schedule[secondIndex];
                    currentIndex--;
                    secondIndex--;
                }
                else
                {
                    secondIndex--;
                }
            
            }
            currentSchedule = LocalSearchBestInsert(currentSchedule);
            return currentSchedule;
        }

    }
}
