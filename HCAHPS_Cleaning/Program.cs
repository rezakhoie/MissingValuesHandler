using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections;
using System.Threading;
using System.IO;


namespace HCAHPS_Cleaning
{
    class Program
    {

        static string ConvertStringArrayToString(string[] array)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string value in array)
            {
                builder.Append(value);
                builder.Append(',');
            }
            return builder.ToString();
        }
        public static double calcDistance(string[] a, string[] b)
        {
            if (a.Length != b.Length)
                return -1;
            else
            {
                double dist = 0;
                for (int i = 0; i < a.Length; i++)
                {
                    if (a[i].All(char.IsDigit))
                        dist += Math.Pow(int.Parse(a[i]) - int.Parse(b[i]), 2);
                    else if (a[i] != b[i])
                        dist += 25;
                }
                return Math.Sqrt(dist);
            }
        }
        
        static void Main(string[] args)
        {
            DataTable data = new DataTable();
            data = MySqlDB.Query("SELECT * FROM `" + "h2007" + "`", "h2007");
            int columnNo = data.Columns.Count;
            int dataCount=data.Rows.Count;
            string[] inputRow = new string[columnNo];
            string filePath = "C:/Users/Reza/Desktop/" + "handled.csv";
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
            List<string[]> tempRecords = new List<string[]>();
            List<string[]> completeRecords = new List<string[]>();
            for (int i = 0; i < dataCount; i++)
            {
                inputRow = data.Rows[i].ItemArray.Select(x => x.ToString()).ToArray();
                tempRecords.Add(inputRow);
            }

            for (int i = 0; i < dataCount; i++)
            {
                if (!tempRecords[i].Contains("NA"))
                {
                    completeRecords.Add(tempRecords[i]);
                }
            }
            int completeDataCount = completeRecords.Count;
            Parallel.For(0, dataCount, i =>
                {
                    if (tempRecords[i].Contains("NA"))
                    {
                        double[] distances = new double[completeDataCount];
                        for (int j = 0; j < completeDataCount; j++)
                        {
                            distances[j] = calcDistance(tempRecords[i], completeRecords[j]);
                        }
                        string[] tempstr = completeRecords[Array.IndexOf(distances, distances.Min())];
                        for (int j = 0; j < columnNo; j++)
                        {
                            if (tempRecords[i][j].Equals("NA"))
                            {
                                tempRecords[i][j] = tempstr[j];
                            }
                        }
                    }
                    if (i % 1000 == 0)
                    {
                        double a = (double)(100 * i) / ((double)dataCount);
                        Console.WriteLine("%"+a+" cleaned");
                    }
                });
            for (int i = 0; i < dataCount; i++)
            {
                File.AppendAllText(filePath, ConvertStringArrayToString(tempRecords[i]));
            }
          
            Console.ReadLine();
        }
    }
}
