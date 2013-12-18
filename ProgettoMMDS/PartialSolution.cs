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
        List<int> schedule;
        BitArray _mask;
        int _dummy;
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
            return schedule;
        }

        /// <summary>
        /// Crea una partial solution con un unico job o unica macchina
        /// </summary>
        /// <param name="idJob">Identificativo Job</param>
        /// <param name="dummy">Identificativo Job Dummy</param>
        /// <param name="posizione">Posizione in cui inserire il job nello schedule</param>
        /// <param name="lunghezza">Lunghezza dello schedule</param>
        public PartialSolution(int idJob,int dummy, int posizione, int lunghezza)
        {
            _dummy = dummy;
            schedule = new List<int>();
            _mask = new BitArray(lunghezza);
            _mask=_mask.Not();
            _mask.Set(posizione,false); // 0 se ho un job
            for (int i = 0; i < lunghezza; i++)
			{
                if(i==posizione)
                    schedule.Add(idJob);  
                else
			        schedule.Add(dummy);   
			}
        }

        public void mergeSolution(PartialSolution ps)
        {
            if(ps._mask.Or(_mask) ==(new  BitArray(_mask.Count))) //se or mi da risultato zero posso fare la fusione
            {
                List<int> lista = ps.getSchedule();
                for (int i = 0; i < _mask.Count; i++)
                {
                    if(lista[i] != _dummy)
                    {
                        schedule[i] = lista[i];
                    }
                }
            }
            else
            {
                Console.WriteLine("Fusione non possibile");
            }
        }

 
        public String ToString()
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
