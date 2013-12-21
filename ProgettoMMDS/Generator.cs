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
        ulong[] MASKS;
        int lunghezzaSch;

        public Generator(List<Job> istanza, int NumberMachine)
        {
            jobs= istanza;
            macchine = NumberMachine;
            dummy = jobs.Count;
            MASKS = generateMasks();
            lunghezzaSch = jobs.Count + macchine - 1;
            
        }

        
        
        public PartialSolution makePartialSolution(int idJob, int posizione)
        {
            
            List<int> schedule = new List<int>();
            ulong mask = 0UL; //maschera vuota
            mask = mask | MASKS[posizione]; //set del bit interssato
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
        


        public PartialSolution mergeSolution(PartialSolution ps1, PartialSolution ps2)
        {
            int offset;
           
            if ((offset=feasible(ps1.Mask,ps2.Mask))!=-1) //se è feasible posso fare la fusione
            {

                List<int> merge = new List<int>();
                ulong maskmerge = ps1.Mask | (ps2.Mask >> offset);

                for (int i = 0; i < ps1.getSchedule().Count + offset; i++)
                {
                    if((i<ps1.getSchedule().Count)&&(ps1.getSchedule()[i])!=dummy)
                    {
                        merge.Add(ps1.getSchedule()[i]);
                    }
                    else if(i>=offset)
                    {
                        merge.Add(ps2.getSchedule()[i-offset]);
                    } 
                    else
                    {
                        merge.Add(dummy);
                    }
                }
                int t = getTardiness(merge);
                return new PartialSolution(merge, maskmerge, t);
            }
            else
            {
                Console.WriteLine("Fusione non possibile");
                return null;
            }
        }

        public PartialSolution[] separate(PartialSolution instable)
        {
            int offset = instable.getSchedule().Count - lunghezzaSch-1;
            List<int> schedule1 = new List<int>();
            List<int> schedule2 = new List<int>();
            ulong mask1 = 0UL;
            ulong mask2 = 0UL;
            BitArray jobpresenti = new BitArray(lunghezzaSch, false);
            int t1, t2, m1=1;

            for(int i = 0; i<instable.getSchedule().Count;i++)//controllo macchine è diverso
            {
                int job = instable.getSchedule()[i];
                if(i<offset)
                {
                    schedule1.Add(job);
                    if (job != dummy)
                    {
                        if (job != 0)
                        {
                            jobpresenti[job] = true;
                            mask1 = mask1 | MASKS[instable.getSchedule()[schedule1.Count - 1]];
                        }
                        else
                        {
                            m1++;
                            mask1 = mask1 | MASKS[instable.getSchedule()[schedule1.Count - 1]];
                        }
                    }
                }
                else if (i > lunghezzaSch)
                {
                    schedule2.Add(instable.getSchedule()[i]);
                    if (instable.getSchedule()[i] != dummy)
                        mask2 = mask2 | MASKS[instable.getSchedule()[schedule2.Count-1]];
                }
                else
                {
                    if (job == 0)
                    {
                        Console.WriteLine("MACCHINA");
                        if (m1 < macchine)
                        {
                            schedule1.Add(job);
                            schedule2.Add(dummy);
                            m1++;
                            mask1 = mask1 | MASKS[schedule1.Count - 1];
                        }
                        else
                        {
                            schedule1.Add(dummy);
                            schedule2.Add(job);
                            mask2 = mask2 | MASKS[schedule2.Count - 1];
                        }
                    }
                    else if(job != dummy)
                    {
                        if (jobpresenti[job] == true)
                        {
                            schedule1.Add(dummy);
                            schedule2.Add(job);
                            mask2 = mask2 | MASKS[schedule2.Count - 1];
                        }
                        else
                        {
                            schedule1.Add(job);
                            schedule2.Add(dummy);
                            jobpresenti[job] = true;
                            mask1 = mask1 | MASKS[schedule1.Count - 1];
                        }
                    }
                    else 
                    {
                        schedule1.Add(dummy);
                        schedule2.Add(dummy);
                    }
                }
            }
            t1 = getTardiness(schedule1);
            t2 = getTardiness(schedule2);
            return new PartialSolution[] { new PartialSolution(schedule1, mask1, t1), new PartialSolution(schedule2, mask2, t2) };
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
        private int feasible(ulong bitArray1, ulong bitArray2)
        {

            ulong shift = bitArray2;
            ulong fusion;

            for (int i = 0; i< lunghezzaSch ; i++)
            {
                fusion = bitArray1 & shift;
                
                if (fusion == 0)
                    return i;
                shift= shift >> 1;
            }
            return -1;
        }
        
        private ulong[] generateMasks()
        {
            ulong one = 1UL;
            ulong[] retval=new ulong[64];
 	        for(int i= 64-1 ;i>=0 ; i--)
            {
			    retval[i]=one;
                one=one<<1;
               // Console.WriteLine(i+":"+Convert.ToString((long)retval[i],2));
            }
            return retval;
        }

    }

}
