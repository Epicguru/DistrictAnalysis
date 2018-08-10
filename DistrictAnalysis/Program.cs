using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistrictAnalysis
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hey there! Analysis software by James B.");
            Console.WriteLine();
            Console.WriteLine();

            string dataPath = @"C:\Users\James\Downloads\Data.csv";

            try
            {
                Data.Load(dataPath, 1, 7);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Something went wrong when loading the data from '" + dataPath + "'!");
                Console.WriteLine(e);
            }


            Console.WriteLine();
            Console.WriteLine();

            Data.Analyze(Data.Loaded);

            Console.WriteLine();

            Console.ReadLine();
        }
    }
}
