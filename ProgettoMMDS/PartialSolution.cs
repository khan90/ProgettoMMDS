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
        uint _mask;  //32bit-->     0 se dummy 
                     //             1 se job è presente
        public uint Mask
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

        uint _jobmask;
        public uint JobMask
        {
            get
            {
                return _jobmask;
            }
            set
            {
                _jobmask = value;
            }
        }

        public int _machine;
        public int Machine
        {
            get
            {
                return _machine;
            }
            set
            {
                _machine = value;
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

        bool _complete;
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
        public PartialSolution(List<int> schedule, uint mask, uint jobmask , int machine,int tardiness, bool complete = false)
        {
            _schedule = schedule;
            Mask = mask;
            JobMask = jobmask;
            Machine = machine;
            Tardiness = tardiness;
            _complete = complete;
        }


        
        public bool isComplete()
        {
            return _complete;
        }
       
 
        public override String ToString()
        {
            String ret = "|";
            for (int i = 0; i < _schedule.Count; i++)
			{
			    ret+=_schedule[i]+"|";
			}
            ret += "\nTardiness:" + _tardiness + " Mask:"+_mask + " " + _complete;
            return (ret);
        }
        
    }
}
