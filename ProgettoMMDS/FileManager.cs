using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgettoMMDS
{
    class FileManager
    {
        private int numberofJob;
        private int numberofMachine;
        private string filename;
        private List<Job> jobsList;

        /// <summary>
        /// Istanzia un  oggetto che estrae le informazioni dal file e genera il file di output !DA FARE
        /// </summary>
        /// <param name="path">Percorso e nome del file</param>
        public FileManager(string path) 
        {

            //Apro il file e leggo ogni stringa
#if (DEBUG)
            System.Console.WriteLine("Apertura File "+ path);
#endif
            try
            {
                String[] lines = System.IO.File.ReadAllLines(path);

                //riga 3: macchine e jobs 
                //N : numero di Jobs;
                //M : numero di macchine;

                char[] charSeparators = new char[] { ' ' };
                String[] token = lines[3].Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                numberofJob = int.Parse(token[0]);
                numberofMachine = int.Parse(token[1]);
#if (DEBUG)
            System.Console.WriteLine("Numero di macchine: "+ numberofMachine+ "Numero di Job: "+ numberofJob);
#endif
                jobsList = new List<Job>();
                //riga 5: iniziano i job
                for (int i = 5; i < lines.Length; i++)
                {

                    token = lines[i].Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                    int id = Convert.ToInt32(token[0]);
                    int pt = Convert.ToInt32(token[1]);
                    int dd = Convert.ToInt32(token[2]);
                    jobsList.Add(new Job(id,pt, dd));

                }

#if (DEBUG)
            System.Console.WriteLine("Job Estratti");
#endif
            }catch(FileNotFoundException fnfe)
            {
                System.Console.WriteLine("File non Trovato\nPremere un tasto qualsiasi");
                System.Console.ReadKey();
                Environment.Exit(0);
            }
        }

        public int getNumberofMachine()
        {
            return numberofMachine;
        }
        public int getNumberofJobs()
        {
            return numberofJob;
        }
        public List<Job> getJobsList()
        {
            return jobsList;
        }
    }
}
