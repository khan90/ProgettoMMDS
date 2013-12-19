using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgettoMMDS
{
    class PartialSolution
    {
        List<int> _schedule;
        BitArray _mask;
        public BitArray Mask
        {
            get
            {
                return _mask;
            }
            set
            {
                _mask = value;
            }
        }
      
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
        /// <summary>
        /// Restituisce lo schedule della PartialSolution
        /// </summary>
        /// <returns><see cref="List"/></returns>
        public List<int> getSchedule()
        {
            return _schedule;
        }

        public void setSchedule(List<int> sch)
        {
            _schedule = new List<int>(sch);
        }

        //COSTRUTTORE
        /// <summary>
        /// Costruttore della classe PartialSolution
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="mask"></param>
        /// <param name="tardiness"></param>
        public PartialSolution(List<int> schedule, BitArray mask, int tardiness)
        {
            _schedule = schedule;
            Mask = mask;
            Tardiness = tardiness;
        }


        

 
        public String ToString()
        {
            String ret = "|";
            for (int i = 0; i < _schedule.Count; i++)
			{
			    ret+=_schedule[i]+"|";
			}
            ret += "Tardiness:" + _tardiness;
            return (ret);
        }
        
    }
}
