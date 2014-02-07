﻿using System;
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
        private List<Job> jobsList;
        private string _path;

        private const string RISULTATI = "Risultati.txt";
        private const string PROVE = "Prove.csv";

        public FileManager()
        {
            return;
        }

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
                _path = path;

                //riga 3: macchine e jobs 
                //N : numero di Jobs;
                //M : numero di macchine;

                char[] charSeparators = new char[] { ' ', '\t' };
                String[] token = lines[3].Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                numberofJob = int.Parse(token[0]);
                numberofMachine = int.Parse(token[1]);
#if (DEBUG)
            Console.WriteLine("Numero di macchine: " + numberofMachine + " Numero di Job: " + numberofJob);
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
            Console.WriteLine("Job Estratti");
#endif
            }catch(FileNotFoundException fnfe)
            {
#if (DEBUG)
                Console.WriteLine(fnfe.StackTrace);
#endif
                Console.WriteLine("File non Trovato\nPremere un tasto qualsiasi : \n");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        public void OutputAll(List<int> schedule,int tardiness, double executionTime, string comment)
        {
            OutputProva(schedule, tardiness, executionTime, comment);
            OutputResult(tardiness, executionTime);
            OutputSolution(schedule);
        }
        /// <summary>
        /// Crea un file .sol con la soluzione
        /// </summary>
        /// <param name="schedule"></param>
        public void OutputSolution(List<int> schedule)
        {
            string filename = _path.Substring(0, _path.Length - 4);
            StreamWriter stream = System.IO.File.CreateText(filename + ".sol");
            //  StreamWriter stream = System.IO.File.AppendText();
            int mac=1;
            stream.Write("M1\t");
            for(int i=0;i<schedule.Count;i++)
            {
                if(schedule[i]<=0)
                {
                    stream.Write("\r\nM"+ ++mac +"\t");
                }
                else
                {
                    stream.Write(schedule[i] + "\t");
                }
            }
            stream.Close();
        }

        /// <summary>
        /// Crea un file .sol con la soluzione nel formato xx_yy_zz_iii_<paramref name="RUN">RUN</paramref>.sol
        /// </summary>
        /// <param name="schedule">schedule ottenuto</param>
        /// <param name="RUN">Numero della Run</param>
        public void OutputSolution(List<int> schedule, int RUN)
        {
            string filename = _path.Substring(0, _path.Length - 4);
            StreamWriter stream = System.IO.File.CreateText(filename + "_" + RUN + ".sol");
            //  StreamWriter stream = System.IO.File.AppendText();
            for (int i = 0; i < schedule.Count; i++)
            {
                if (schedule[i] <= 0)
                {
                    stream.Write("\r");
                }
                else
                {
                    stream.Write(schedule[i] + "\t");
                }
            }
            stream.Close();
        }

        /// <summary>
        /// Aggiunge al file Risultati.txt l'esecuzione
        /// </summary>
        /// <param name="tardiness">Valore della tardiness</param>
        /// <param name="executionTime">Tempo di esecuzione</param>
        public void OutputResult(int tardiness, double executionTime)
        {
            string filename = _path.Substring(0, _path.Length - 4);
            StreamWriter stream = System.IO.File.AppendText(RISULTATI);
            string output = filename + "\t" + tardiness + "\t" + executionTime;
            stream.WriteLine(output);
            stream.Close();
        }

        /// <summary>
        /// Aggiunge al file Risultati.txt l'esecuzione nella sottocartella <paramref name="RUN">RUN</paramref>
        /// </summary>
        /// <param name="tardiness">Valore della tardiness</param>
        /// <param name="executionTime">Tempo di esecuzione</param>
        /// <param name="RUN">Numero della Run</param>
        public void OutputResult(int tardiness, double executionTime, int RUN)
        {
            string filename = _path.Substring(0, _path.Length - 4);
            DirectoryInfo di = Directory.CreateDirectory(RUN.ToString());
            String pathString = System.IO.Path.Combine(RUN.ToString(), RISULTATI);
            StreamWriter stream = System.IO.File.AppendText(pathString);
            string output = filename + "\t" + tardiness + "\t" + (int)executionTime;
            stream.WriteLine(output);
            stream.Close();
        }
        /// <summary>
        /// Stampa il risultato dell'esecuzione sul file di prova
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="tardiness"></param>
        /// <param name="executionTime"></param>
        /// <param name="comment"></param>
        public void OutputProva(List<int> schedule,int tardiness, double executionTime, string comment)
        {
            string filename = _path.Substring(0, _path.Length - 4);
            StreamWriter stream = System.IO.File.AppendText(PROVE);
            string output = filename + ";" + tardiness + ";" + executionTime +";";
            for(int i =0; i<schedule.Count;i++)
            {
                output += schedule[i] + " "; 
            }
            output += ";" + comment + ";" + DateTime.Now;
            stream.WriteLine(output);
            stream.Close();
        }

        public void OutputPopolation(List<Schedule> population)
        {
            StreamWriter stream = System.IO.File.CreateText("popolazione.csv");
            //  StreamWriter stream = System.IO.File.AppendText();

            foreach (Schedule schedule in population){
                string output = "";
                for (int i = 0; i < schedule.schedule.Count; i++)
                {
                    output += schedule.schedule[i] + " ";
                }
                output += ";" + schedule.getTardiness();
                stream.WriteLine(output);
            }
            
            stream.Close();
        }
        public void OutputPopolation(List<Schedule> population, string testo)
        {
            StreamWriter stream = System.IO.File.CreateText("popolazione" + testo + ".csv");
            //  StreamWriter stream = System.IO.File.AppendText();

            foreach (Schedule schedule in population)
            {
                string output = "";
                for (int i = 0; i < schedule.schedule.Count; i++)
                {
                    output += schedule.schedule[i] + " ";
                }
                output += ";" + schedule.getTardiness();
                stream.WriteLine(output);
            }

            stream.Close();
        }

        internal void appendBestTardiness(int currentTardiness)
        {
            StreamWriter stream = System.IO.File.AppendText("Popolazioni.csv");
            stream.WriteLine(currentTardiness);
            stream.Close();
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
