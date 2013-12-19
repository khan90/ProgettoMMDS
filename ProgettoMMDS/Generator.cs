using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgettoMMDS
{
    class Generator
    {
        private List<Job> jobs;
        int macchine; 
        int dummy;


        public Generator(List<Job> istanza, int NumberMachine)
        {
            jobs=istanza;
            macchine = NumberMachine;
            dummy = jobs.Count + 1;
        }

        public PartialSolution makePartialSolution(int idJob, int posizione)
        {
            
            List<int> schedule = new List<int>();
            BitArray mask = new BitArray(jobs.Count+macchine);
            mask.Not();
            mask.Set(posizione,false); // 0 se ho un job
            for (int i = 0; i < jobs.Count + macchine; i++)
			{
                if(i==posizione)
                    schedule.Add(idJob);  
                else
			        schedule.Add(dummy);   
			}
            int tardiness = getTardiness(schedule);
            return (new PartialSolution(schedule, mask, tardiness));
        }



        public List<PartialSolution> mergeSolution(PartialSolution ps1, PartialSolution ps2)
        {
            int offset;
            List<PartialSolution> retval= new List<PartialSolution>();
            if ((offset=feasible(ps1.Mask,ps2.Mask))!=-1) //se è feasible posso fare la fusione
            {

                List<int> newPSSchedule1 = new List<int>();
                List<int> newPSSchedule2 = new List<int>();

                BitArray newPSMask1 = new BitArray(jobs.Count + macchine);
                newPSMask1.Not();
                BitArray newPSMask2 = new BitArray(jobs.Count + macchine);
                newPSMask2.Not();
                BitArray jobschedulati = new BitArray(jobs.Count);
                int macc;
                int jobdainserire;
                bool ammisible = true;

                for (int i = 0; i < ps1.getSchedule().Count; i++)
                {
                    if(ps1.getSchedule()[i]!=dummy)
                    {
                        jobdainserire = ps1.getSchedule()[i];
                    }
                    else if (ps2.getSchedule()[i]!=dummy)
                    {
                        jobdainserire = ps2.getSchedule()[i];
                    }
                    else
                    {
                        jobdainserire = dummy;
                    }
                    //devo inserire il job
                    if (jobdainserire == 0)
                    {
                        //controllo se è ammissibile su macchine
                        if (macc < macchine)
                        {
                            newPSSchedule1.Add(0);
                            newPSMask1.Set(i, false);
                            macc++;
                        }
                        else
                        {
                            newPSSchedule2.Add(0);
                            newPSMask2.Set(i, false);
                        }
                    }
                    else
                    {
                        //è un job 
                    }

                }

                for (int i = 0; i < ps2.getSchedule().Count; i++)
                {
                    if (ps2.getSchedule()[i] != dummy)
                    {
                        newPSSchedule[i] = ps2.getSchedule()[i];
                        newPSMask.Set(i, false);
                    }
                }
            }
            else
            {
                Console.WriteLine("Fusione non possibile");
                return null;
            }
        }
        int getTardiness(List<int> schedule)
        {
            int time = 0;
            int tardiness = 0;

            foreach (int i in schedule)
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



        /// <summary>
        /// Verifica se le due molecole possono essere fuse. Questo si verifica se OR tra le due maschere da un vettore di 1
        /// </summary>
        /// <param name="bitArray1"></param>
        /// <param name="bitArray2"></param>
        /// <returns>-1 se non è possibile, altrimenti mi ritorna l'offset</returns>
        private int feasible(BitArray bitArray1, BitArray bitArray2)
        {

            BitArray fusion = new BitArray(bitArray1);
            fusion.Or(bitArray2);
            foreach (bool value in fusion)
            {
                if (!value)
                    return -1;
            }
            return 0;
        }
    }
}
