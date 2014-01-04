using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgettoMMDS
{
    public class Schedule
    {
        List<int> _schedule;
        public List<int> schedule
        {
            get
            {
                return _schedule;
            }
            set
            {
                _schedule = new List<int>(value);
            }
        }

        List<Job> _jobs;
        public List<Job> jobs
        {
            get
            {
                return _jobs;
            }
        }

        public Schedule(List<Job> arg1)
        {
            _jobs = new List<Job>(arg1);
            _schedule = new List<int>();
        }
        public Schedule(Schedule scheduleToCopy)
        {
            _schedule = new List<int>(scheduleToCopy.schedule);
            _jobs = scheduleToCopy.jobs;
        }

        /// <summary>
        /// Costruisce uno Schedule EDD prendendo in input macchine e la lista dei job  
        /// </summary>
        /// <param name="m">Numero Macchine</param>
        /// <param name="n">Numero Job</param>
        /// <returns></returns>
        public void constructScheduleEDD(int m, int n)
        {
            //Ho creato qui una nuova lista temporanea -> la lista jobs non viene toccata.
            List<Job> tempJobs = new List<Job>(jobs);
            //Ordinamento qui -> In ordine di DueDate:
            tempJobs.Sort((x, y) => x.getDueDateTime().CompareTo(y.getDueDateTime()));
            //a questo punto tempJobs è ordinato in base alla DueDate...

            _schedule = new List<int>();
            int[] time = new int[m];
            int[] index = new int[m];
            for (int i = 0; i < m; i++)
            {
                index[i] = i;
                time[i] = 0;
            }
            int mi = 0;
            for (int i = 0; i < m - 1; i++)
            {
                _schedule.Add(mi--);
            }
            int bestMac = 0;
            for (int i = 0; i < n; i++)
            {
                Job j = jobs[i];

                _schedule.Insert(index[bestMac], j.getID());
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
        }

        /// <summary>
        /// Funzione che calcola la Tardiness totale di uno schedule dato
        /// </summary>
        /// <returns>Tardiness totale</returns>
        public int getTardiness()
        {
            int time = 0;
            int tardiness = 0;

            foreach (int i in _schedule)
            {
                if (i <= 0)
                    time = 0;
                else
                {
                    Job j = _jobs[i - 1];
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
        /// Restituisce il numero degli elementi presenti nello schedule
        /// Comprese le macchine
        /// </summary>
        /// <returns>numero degli elementi</returns>
        public int Count()
        {
            return _schedule.Count();
        }

        public void swap(int num1, int num2){
            int temp = _schedule[num1];
            _schedule[num1] = _schedule[num2];
            _schedule[num2] = temp;
        }
        /// <summary>
        /// Aggiunge un job alla fine dello schedule
        /// </summary>
        /// <param name="job">job da aggiungere</param>
        public void Add(int job)
        {
            _schedule.Add(job);
        }
        
        public override string ToString()
        {
            string output = "";
            for (int i = 0; i < schedule.Count; i++)
            {
                output += schedule[i] + " ";
            }
            return output;
        }
    }
}
