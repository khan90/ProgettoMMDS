using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ProgettoMMDS
{
    abstract class AbstractScheduler
    {
        /// <summary>
        /// Lista dei Job in ordine di ID
        /// </summary>
        protected List<Job> jobs = new List<Job>();
        protected volatile bool fine = false;
        protected static int MTIME = 1000;
        protected static int RUN = 0;
        protected volatile bool fine2 = false;

        public abstract void run(string[] args);

        //public abstract void run(string[] args, int time);
        
        /// <summary>
        /// Algoritmo di ricerca locale che sposta un elemento a caso nella migliore posizione.
        /// Tra tutti gli spostamenti possibili di un elemento scelto a caso prende quello che migliora di più la soluzione
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        /// MaxIteration prima era 150, prova a diminuire un po'
        protected Schedule LocalSearchBestInsert(Schedule schedule)
        {

            //Variabili dell'algoritmo
            //60 
            //int maxIterations = 10 + (10000 / MTIME) * 15;
            int maxIterations = 85;
            //Console.WriteLine(maxIterations);
            int minImprovment = 0;
            int tabuCapacity = 5;
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
                /*for (int j = 0; j < tabuCapacity; j++)
                {
                    if (num1 == tabuList[j])
                    {
                        j = 0; ;    //Un while non è più elegante!!??!?!?
                        num1 = r.Next(maxInt);
                    }
                }*/
                while (tabuList.Contains(num1))
                    num1 = r.Next(maxInt);
                // QUi aggiungo l'elemento che ho appena selezionato alla tabù List
                tabuList[tabuIndex] = num1;
                tabuIndex = (tabuIndex + 1) % tabuCapacity;
                int num2;
                for (num2 = 0; num2 < maxInt; num2++)
                {
                    if ((num2 == num1)||(tabuList.Contains(num2)))   //e se facessimo il controllo su entrambi i numeri se appartengono alla tabù list?
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
                    else if (r.Next(1000) < 25)      //Ogni tanto effettuo uno swap anche se non mi miglioro, ma non salvo la soluzione
                    {
                        num1 = num2;        //Piccola modifica
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

        protected Schedule LocalSearchBestInsert(Schedule schedule, int maxIterations)
        {
            //Qui gli passo io il numero di iterazioni
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
                /*for (int j = 0; j < tabuCapacity; j++)
                {
                    if (num1 == tabuList[j])
                    {
                        j = 0; ;    //Un while non è più elegante!!??!?!?
                        num1 = r.Next(maxInt);
                    }
                }*/
                while (tabuList.Contains(num1))
                    num1 = r.Next(maxInt);
                // QUi aggiungo l'elemento che ho appena selezionato alla tabù List
                tabuList[tabuIndex] = num1;
                tabuIndex = (tabuIndex + 1) % tabuCapacity;
                int num2;
                for (num2 = 0; num2 < maxInt; num2++)
                {
                    if ((num2 == num1) || (tabuList.Contains(num2)))   //e se facessimo il controllo su entrambi i numeri se appartengono alla tabù list?
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
                    else if (r.Next(1000) < 25)      //Ogni tanto effettuo uno swap anche se non mi miglioro, ma non salvo la soluzione
                    {
                        num1 = num2;        //Piccola modifica
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

        protected void timer()
        {
            Console.WriteLine("Counter partito 1");
            Thread.Sleep(MTIME);
            fine = true;
        }

        protected void timer2()
        {
            Console.WriteLine("Counter partito 2");
            Thread.Sleep(MTIME/10);
            fine2 = true;
        }
    }
}
