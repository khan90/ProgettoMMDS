using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgettoMMDS
{
    class PartialSolution
    {
        List<int> schedule;
        bool[] mask;
        int _tardiness;
        public int Tardiness
        {
            get
            {
                return _tardiness;
            }
            set
            {
                _tardiness = value;
            }
        }

        public List<int> getSchedule()
        {
            return schedule;
        }
        public PartialSolution(int idJob,int dummy, int posizione, int lunghezza)
        {
            schedule = new List<int>();
            mask = new bool[lunghezza];
            mask[posizione]=true;
            for (int i = 0; i < lunghezza; i++)
			{
                if(i==posizione)
                    schedule.Add(idJob);  
                else
			        schedule.Add(dummy);   
			}
        }


 
        public String toString()
        {
            String ret = "|";
            for (int i = 0; i < schedule.Count; i++)
			{
			    ret+=schedule[i]+"|";
			}
            ret += "Tardiness:" + _tardiness;
            return (ret);
        }
        
    }
}
