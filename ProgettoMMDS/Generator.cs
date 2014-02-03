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
        uint[] MASKS;
        uint COMPLETEMASK;
        int lunghezzaSch;

        public Generator(List<Job> istanza, int NumberMachine)
        {
            jobs= istanza;
            macchine = NumberMachine;
            dummy = jobs.Count;
            generateMasks();
            lunghezzaSch = jobs.Count + macchine - 1;
            
        }

        
        
        public PartialSolution makePartialSolution(int idJob, int posizione)
        {
            int machine = 0;
            uint jmask = 0;
            if(idJob == 0)
            {
                machine = 1; //aumento conteggio macchine
            }
            else
            {
                jmask= MASKS[idJob];//setto 1 il job corrispondente
            }
            
            List<int> schedule = new List<int>();
            uint mask = MASKS[posizione]; //set 1 la posizione del job
            for (int i = 0; i < jobs.Count + macchine; i++)
			{
                if (i == posizione)
                    schedule.Add(idJob);
                else
                    schedule.Add(dummy);   
			}
            int tardiness = getTardiness(schedule);
            return (new PartialSolution(schedule, mask,jmask,machine, tardiness));
        }
        

        
        public PartialSolution[] mergeNseparate(PartialSolution ps1, PartialSolution ps2)
        {
            int offset;
           
            if ((offset=feasible(ps1.Mask,ps2.Mask))!=-1) //se è feasible posso fare la fusione
            {

                List<int> merge = new List<int>();
                uint maskmerge = ps1.Mask | (ps2.Mask >> offset);//??

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

                List<int> schedule1 = new List<int>();
                List<int> schedule2 = new List<int>();
                uint mask1 = 0;
                uint mask2 = 0;
                uint jobmask1 = 0;
                uint jobmask2 = 0;
                int machine1 = 0;
                int machine2 = 0;

                for (int i = 0; i < merge.Count; i++)
                {
                    int idjob = merge[i];
                    if(i<offset)
                    {
                        schedule1.Add(merge[i]);
                        if (idjob == 0)
                        {
                            machine1++;
                            mask1 =mask1| MASKS[i];
                        }
                        else if (idjob != dummy)
                        {
                            jobmask1 = MASKS[idjob];
                            mask1 = mask1| MASKS[i];
                        }
                    }
                    else if(i>=ps1.getSchedule().Count )
                    {
                        schedule2.Add(merge[i]);
                        if (idjob == 0)
                        {
                            machine2++;
                            mask2 =  mask2| MASKS[i-offset];
                        }
                        else if (idjob != dummy)
                        {
                            jobmask2 = MASKS[idjob];
                            mask2 = mask2 | MASKS[i - offset];
                        }
                    }
                    else
                    {
                        if(idjob==dummy)
                        {
                            schedule1.Add(dummy);
                            schedule2.Add(dummy);
                        }
                        else if(idjob==0)
                        {
                            if (machine1 < macchine)
                            {
                                machine1++;
                                schedule1.Add(idjob);
                                schedule2.Add(dummy);
                                mask1 = mask1 | MASKS[i];
                            }
                            else
                            {
                                machine2++;
                                schedule2.Add(idjob);
                                schedule1.Add(dummy);
                                mask2 = mask2 | MASKS[i - offset];
                            }
                        }
                        else
                        {
                            if((jobmask1 & MASKS[idjob])==0)
                            {
                                schedule1.Add(idjob);
                                schedule2.Add(dummy);
                                mask1 = mask1 | MASKS[i];
                                jobmask1 = jobmask1 | MASKS[idjob];
                            }
                            else
                            {
                                schedule2.Add(idjob);
                                schedule1.Add(dummy);
                                mask2 = mask2 | MASKS[i - offset];
                                jobmask2 = jobmask2 | MASKS[idjob];
                            }
                        }
                    }
                    
                }

                int t1 = getTardiness(schedule1);
                int t2 = getTardiness(schedule2);
                return new PartialSolution[]{new PartialSolution(schedule1, mask1, jobmask1,machine1,t1), new PartialSolution(schedule2,mask2,jobmask2,machine2,t2};
            }
            else
            {
                Console.WriteLine("Fusione non possibile");
                return null;
            }
        }
        public PartialSolution mergeSolution(PartialSolution ps1, PartialSolution ps2)
        {
            bool complete = true; 
            List<int> merge = new List<int>();
            uint maskmerge = ps1.Mask | ps2.Mask;
            uint jobmaskmerge = ps1.JobMask | ps2.JobMask;
            int machinemerge = ps1.Machine + ps2.Machine;
            for(int i = 0 ; i< lunghezzaSch; i++)
            {
                if (ps1.getSchedule()[i] != dummy)
                {
                    merge.Add(ps1.getSchedule()[i]);
                }
                else if (ps2.getSchedule()[i] != dummy)
                {
                    merge.Add(ps2.getSchedule()[i]);
                }
                else
                {
                    merge.Add(dummy);
                    complete = false;
                }
            }
            
            int t = getTardiness(merge);
            return new PartialSolution(merge, maskmerge,jobmaskmerge,machinemerge, t, complete);
        }

        public PartialSolution[] separate(PartialSolution instable)
        {
            List<int> schedule1 = new List<int>();
            List<int> schedule2 = new List<int>();
            uint mask1 = 0;
            uint mask2 = 0;
            uint jobmask1=0;
            uint jobmask2=0;
            int machine1=0;
            int machine2=0;
            Random r = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            for(int i =0; i<lunghezzaSch;i++)
            {
                int idjob= instable.getSchedule()[i];
                if(r.Next(2) == 1)
                {
                    //va su 1
                    schedule1.Add(idjob);
                    schedule2.Add(dummy);
                    mask1 = mask1 | MASKS[i];
                    if(idjob== 0)
                    {
                        machine1++;
                    }
                    else 
                    {
                        jobmask1 = jobmask1 | MASKS[idjob];
                    }
                }
                else
                {
                    //va su due
                    schedule2.Add(idjob);
                    schedule1.Add(dummy);
                    mask2 = mask2 | MASKS[i];
                    if (idjob == 0)
                    {
                        machine2++;
                    }
                    else
                    {
                        jobmask2 = jobmask2 | MASKS[idjob];
                    }
                }
            }

            int t1 = getTardiness(schedule1);
            int t2 = getTardiness(schedule2);

            return new PartialSolution[] { new PartialSolution(schedule1, mask1, jobmask1, machine1, t1), new PartialSolution(schedule2, mask2, jobmask2, machine2, t2) };

        }
        public List<PartialSolution> separateAtom(PartialSolution instable)
        {
            List<PartialSolution> pslist = new List<PartialSolution>();
            for (int i = 0; i < instable.getSchedule().Count; i++)
            {
                if (instable.getSchedule()[i] != dummy)
                {
                    List<int> schedule = new List<int>();
                    for (int j = 0; j < instable.getSchedule().Count; j++)
                    {
                        if (j == i)
                            schedule.Add(instable.getSchedule()[i]);
                        else
                            schedule.Add(dummy);
                    }
                    uint mask = MASKS[i];
                    uint jobmask = 0;
                    int machine = 0;
                    if (instable.getSchedule()[i] == 0)
                        machine++;
                    else
                        jobmask = MASKS[instable.getSchedule()[i]];
                    int t = getTardiness(schedule);
                    pslist.Add(new PartialSolution(schedule, mask, jobmask, machine, t));
                }
            }
            return pslist;
        }

        /*
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
            bool c1 = (mask1 == COMPLETEMASK) ? true : false;
            bool c2 = (mask2 == COMPLETEMASK) ? true : false;

            t1 = getTardiness(schedule1);
            t2 = getTardiness(schedule2);
            return new PartialSolution[] { new PartialSolution(schedule1, mask1, t1,c1), new PartialSolution(schedule2, mask2, t2,c2) };
        }

        */
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

        public bool feasible(PartialSolution ps1, PartialSolution ps2)
        {
            if((ps1.Mask & ps2.Mask) == 0)
            {
                if((ps1.JobMask & ps2.JobMask) == 0 )
                {
                    if(ps1.Machine+ ps2.Machine<=macchine)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        
        private void generateMasks()
        {
            uint one = 1;
            MASKS=new uint[32];
            COMPLETEMASK = 0;
 	        for(int i= 32-1 ;i>=0 ; i--)
            {
			    MASKS[i]=one;
                if(i< lunghezzaSch-1)
                    COMPLETEMASK = COMPLETEMASK | one;
                one=one<<1;
                
               // Console.WriteLine(i+":"+Convert.ToString((long)retval[i],2));
            }
        }

    }

}
