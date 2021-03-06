﻿using System;
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
        static long MTIME = 10000;

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
                List<int> schedule = SearchBestSolutionRandom(fm.getNumberofMachine(), fm.getNumberofJobs());               

                //FINE CONTEGGIO SECONDI
                DateTime stopTime = DateTime.Now;
                TimeSpan elapsedTime = stopTime.Subtract(startTime);
                //Stampa Tardiness e tempo totale
                Console.WriteLine("Tardiness totale: " + getTardiness(schedule).ToString());
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
        /// <returns></returns>
        List<int> SearchSolutionRandom(int m, int n)
        {            
            Thread thread = new Thread(new ThreadStart(timer));
            thread.Start();
            //Ho spostato questo qui nel metodo di scheduling (non è detto che tutti i metodi utilizzino EDD
            //quindi metto questo qui dentro in modo che sia parte integrante del metodo)
            Random r = new Random();
            //soluzione migliore
            List<int> schedule = new List<int>(constructScheduleEDD(m, n));
            int maxInt = schedule.Count(); //forse basta n+m
            int bestTardy = getTardiness(schedule);
            //soluzione corrente
            List<int> currentSchedule = new List<int>(schedule);            
            while (!fine)
            {                
                int num1 = r.Next(maxInt);
                int num2 = r.Next(maxInt);
                while (num2 == num1)
                {
                    num2 = r.Next(maxInt);
                } 
                //SWAP   
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
                    //UNSWAP
                    temp = currentSchedule[num1];
                    currentSchedule[num1] = currentSchedule[num2];
                    currentSchedule[num2] = temp;
                }
                
            }
            return schedule;
        }
        /// <summary>
        /// Questa è come la ricerca locale sopra ma sposta un elemento a caso nella migliore posizione.
        /// Tra tutti gli spostamenti possibili di un elemento a caso prende quello che migliora di più la soluzione
        /// Multistart
        /// </summary>
        /// <param name="m">Numero di macchine</param>
        /// <param name="n">Numero di Job</param>
        /// <returns>Miglior schedule trovato</returns>
        List<int> SearchBestSolutionRandom(int m, int n)
        {
            Thread thread = new Thread(new ThreadStart(timer));
            thread.Start();
            //soluzione migliore -> Il metodo per ora fa solo una ricerca locale a partire dall'EDD
            List<int> schedule = new List<int>(constructScheduleEDD(m, n));
            int bestTardiness = getTardiness(schedule);
            List<int> currentSchedule = new List<int>(schedule);
            int j = 0;
            while (!fine)
            {
                currentSchedule = new List<int>(LocalSearchBestInsert(currentSchedule));
                j++;
                if (getTardiness(currentSchedule) < bestTardiness)
                {
                    schedule = new List<int>(currentSchedule);
                }
                Random r = new Random();
                for (int i = 0; i < schedule.Count(); i++)
                {
                    int num1 = r.Next(schedule.Count());
                    int num2 = r.Next(schedule.Count());
                    if ((num1 == num2)||(0 == currentSchedule[num1])||(0 == currentSchedule[num2]))
                    {
                        i--;
                        continue;
                    }
                    int temp = currentSchedule[num1];
                    currentSchedule[num1] = currentSchedule[num2];
                    currentSchedule[num2] = temp;
                }
            }
            Console.WriteLine(j);
            return (schedule);
        }


        List<int> LocalSearchBestInsert(List<int> schedule)
        {
            //Variabili dell'algoritmo
            int maxIterations = 150;
            int minImprovment = 1;
            int tabuCapacity = 3;
            //Inizializzazione
            Random r = new Random();
            int maxInt = schedule.Count();
            int bestTardy = getTardiness(schedule);
            //soluzione corrente
            List<int> currentSchedule = new List<int>(schedule);
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
                    int temp = currentSchedule[num1];
                    currentSchedule[num1] = currentSchedule[num2];
                    currentSchedule[num2] = temp;
                    
                    currentTardy = getTardiness(currentSchedule);
                    if ((currentTardy < bestTardy))
                    {
                        improvment = bestTardy - currentTardy;
                        bestTardy = currentTardy;
                        schedule = new List<int>(currentSchedule);
                    }
                    else if (r.NextDouble()*10 < 0.5)            //Nel 20% dei casi effettuo lo swap anche se non mi conviene.
                    {
                        //num1 = num2;        //Piccola modifica
                        //IDEA: Qui si può aggiungere una piccola tabù list per evitare che ritorni indietro al passo successivo
                        //Console.Write("*");
                        continue;
                    }
                    //UNSWAP
                    temp = currentSchedule[num1];
                    currentSchedule[num1] = currentSchedule[num2];
                    currentSchedule[num2] = temp;
                }
                //alla fine di una iterazione aggiorno il currentSchedule
                currentSchedule = new List<int>(schedule);
                
                //DA CANCELLARE STAMPA:
                //Console.WriteLine("\t" + i + " " + improvment);
            }
            return schedule;
        }

        /// <summary>
        /// Costruisce uno Schedule EDD prendendo in input macchine e la lista dei job  
        /// </summary>
        /// <param name="m">Numero Macchine</param>
        /// <param name="n">Numero Job</param>
        /// <returns></returns>
        List<int> constructScheduleEDD(int m, int n)
        {
            //Ho creato qui una nuova lista temporanea -> la lista jobs non viene toccata.
            List<Job> tempJobs = new List<Job>(jobs);
            //Ordinamento qui -> In ordine di DueDate:
            tempJobs.Sort((x, y) => x.getDueDateTime().CompareTo(y.getDueDateTime()));
            //a questo punto tempJobs è ordinato in base alla DueDate...

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
            //jobs.Sort((x, y) => x.getID().CompareTo(y.getID()));
            return schedule;
        }


        /// <summary>
        /// Funzione che calcola la Tardiness totale di uno schedule dato
        /// </summary>
        /// <param name="schedule">schedule di cui si vuole calcolare la Tardiness totale</param>
        /// <returns>Tardiness totale</returns>
        int getTardiness(List<int> schedule)
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


        void timer()
        {
            //Console.WriteLine("Counter partito");
            Thread.Sleep((int)MTIME);
            fine = true;
        }
       

    }
}
