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
    public class Scheduler
    {
        /// <summary>
        /// Lista dei Job in ordine di ID
        /// </summary>
        List<Job> jobs = new List<Job>();
        volatile static bool fine = false;
        static long MTIME = 1000;

        public Scheduler()
        {
            fine = false;
        }

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
            Console.WriteLine(j);
            return (schedule);
        }

        /// <summary>
        /// Algoritmo di ricerca locale che sposta un elemento a caso nella migliore posizione.
        /// Tra tutti gli spostamenti possibili di un elemento scelto a caso prende quello che migliora di più la soluzione
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        Schedule LocalSearchBestInsert(Schedule schedule)
        {
            //Variabili dell'algoritmo
            int maxIterations = 150;
            int minImprovment = 0;
            int tabuCapacity = 3;
            //Inizializzazione
            Random r = new Random();
            int maxInt = schedule.Count();
            int bestTardy = schedule.getTardiness();
            //soluzione corrente
            Schedule currentSchedule = new Schedule(schedule);
            int currentTardy = int.MaxValue;
            //miglioramento
            int improvment = int.MaxValue;
            //numero di iterazioni
            int i = 0;
            //Tabu list
            int[] tabuList = new int[tabuCapacity];
            int tabuIndex = 0;
            
            while ((improvment > minImprovment) || (i < maxIterations))
            {                
                i++;
                improvment = 0;
                //GENERAZIONE JOB ESTRATTO
                int num1 = r.Next(maxInt);
                for (int j = 0; j < tabuCapacity; j++)
                {
                    if (num1 == tabuList[j])
                    {
                        j = 0;
                        num1 = r.Next(maxInt);
                        //DA CANCELLARE STAMPA
                        //Console.WriteLine("Non posso spostare questo elemento");
                    }
                }
                // QUi aggiungo l'elemento che ho appena spostato alla tabù List
                tabuList[tabuIndex] = num1;
                tabuIndex = (tabuIndex + 1) % tabuCapacity;

                int num2;              
                for (num2 = 0; num2 < maxInt; num2++)
                {
                    if (num2 == num1)
                        continue;
                    //SWAP
                    currentSchedule.swap(num1, num2);
                    
                    currentTardy = currentSchedule.getTardiness();
                    if ((currentTardy < bestTardy))
                    {
                        improvment = bestTardy - currentTardy;
                        bestTardy = currentTardy;
                        schedule = new Schedule(currentSchedule);
                    }
                    else if (r.Next(100) < 5)            //Ogni tanto effettuo uno swap anche se non mi miglioro, ma non salvo la soluzione
                    {
                        //num1 = num2;        //Piccola modifica
                        //IDEA: Qui si può aggiungere una piccola tabù list per evitare che ritorni indietro al passo successivo
                        //Console.Write("*");
                        continue;
                    }
                    //UNSWAP
                    currentSchedule.swap(num1, num2);
                }
                //alla fine di una iterazione aggiorno il currentSchedule
                currentSchedule = new Schedule(schedule);
                
                //DA CANCELLARE STAMPA:
                //Console.WriteLine("\t" + i + " " + improvment);
            }
            return schedule;
        }

        void timer()
        {
            //Console.WriteLine("Counter partito");
            Thread.Sleep((int)MTIME);
            fine = true;
        }
       

    }
}
