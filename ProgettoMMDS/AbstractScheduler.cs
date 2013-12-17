﻿using System;
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
        protected volatile static bool fine = false;
        protected static long MTIME = 1000;

        public abstract void run(string[] args);
        
        /// <summary>
        /// Algoritmo di ricerca locale che sposta un elemento a caso nella migliore posizione.
        /// Tra tutti gli spostamenti possibili di un elemento scelto a caso prende quello che migliora di più la soluzione
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        protected Schedule LocalSearchBestInsert(Schedule schedule)
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

        protected void timer()
        {
            //Console.WriteLine("Counter partito");
            Thread.Sleep((int)MTIME);
            fine = true;
        }
    }
}
