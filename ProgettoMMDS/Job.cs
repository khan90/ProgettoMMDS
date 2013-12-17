using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgettoMMDS
{
    public class Job
    {
        private int id;
        private int processingtime;
        private int duedatetime;

        public Job(int jobID, int processing_time, int due_date_time)
        {
            id = jobID;
            processingtime = processing_time;
            duedatetime = due_date_time;
        }

        public int getID()
        {
            return id;
        }

        public int getProcessingTime()
        {
            return processingtime;
        }
        public int getDueDateTime()
        {
            return duedatetime;
        }


        public string ToString()
        {
            return "id: "+id+"\tProcTime: " + processingtime + "\tDD: " + duedatetime;
        }

       private static int CompareDueDate(Job j1, Job j2)//1 se j1>j2   -1 se j2>j1     0 se j1=j2
        {
            int dd1= j1.getDueDateTime();
            int dd2 = j2.getDueDateTime();
            if (dd1 == dd2)
                return 0;
            else if (dd1 > dd2)
                return 1;
            else
                return -1;
        }
       private static int CompareProcTime(Job j1, Job j2)//1 se j1>j2   -1 se j2>j1     0 se j1=j2
       {
           int pt1 = j1.getProcessingTime();
           int pt2 = j2.getProcessingTime();
           if (pt1 == pt2)
               return 0;
           else if (pt1 > pt2)
               return 1;
           else
               return -1;
       }
    }

  /*  public class CompareDueDate : IComparer
    {
        // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
        int IComparer.Compare(Object x, Object y)
        {

            return ((new CaseInsensitiveComparer()).Compare(y, x));
        }
    }*/

}
