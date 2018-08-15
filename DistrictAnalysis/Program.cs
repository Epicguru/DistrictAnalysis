using System;
using DistrictAnalysis;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace DistrictAnalysis
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hey there! Analysis software by James B.");
            Console.WriteLine();
            Console.WriteLine();

            string filePath = null;
            bool downloading = false;
            if(args.Length == 0)
            {
                // Testing downloading...
                string url = @"https://docs.google.com/spreadsheets/d/e/2PACX-1vSERHA2mstub6RYfg0YhigF0rbum4C3fnPhGbdUjAu6ao8iNPbL4ALKLVrFcwCxzIzIG5NOZHhCdU80/pub?gid=1471730449&single=true&output=csv";

                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = "chrome.exe";
                info.Arguments = url + " --window-size=0,0";
                info.WindowStyle = ProcessWindowStyle.Minimized;
                var process = System.Diagnostics.Process.Start(info);
                const int SLEEP_TIME = 3000;

                new Thread(() => { Thread.Sleep(SLEEP_TIME); if(!process.HasExited) process.Kill(); }).Start();
                string downloadFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

                Thread.Sleep(SLEEP_TIME);

                string[] filePaths = Directory.GetFiles(downloadFolder);
                bool found = false;
                foreach(var file in filePaths)
                {
                    Console.WriteLine("  >" + Path.GetFileName(file));
                    if(Path.GetFileName(file) == "District Data - School Board.csv")
                    {
                        found = true;
                        filePath = file;
                        break;
                    }
                }

                if (!found)
                    downloading = false;
                else
                    downloading = true;
            }

            string dataPath = null;

            if (!downloading)
            {
                if(args.Length == 0)
                {
                    Data.WriteColour("Failed to download latest file, try to load a local copy...", ConsoleColor.Magenta);
                }

                if (args.Length == 0)
                {
                    Data.WriteColour("Type the full path to the CSV file and press enter.", ConsoleColor.Cyan);
                    Data.WriteColour("You can also drag the file onto the console window to automatically paste the file path.", ConsoleColor.Cyan);
                    dataPath = Console.ReadLine();
                }
                else
                {
                    dataPath = args[0].Trim();
                }
            }
            else
            {
                dataPath = filePath;
            }

            bool cont = true;
            if (string.IsNullOrWhiteSpace(dataPath))
            {
                Data.WriteColour("Blank file path, invalid!", ConsoleColor.Red);
                cont = false;
            }
            else if (!File.Exists(dataPath))
            {
                Data.WriteColour("Could not find that CSV file, did you type the path correctly?", ConsoleColor.Red);
                cont = false;
            }

            if (cont)
            {
                try
                {
                    Data.Load(dataPath, 1, 7);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Something went wrong when loading the data from '" + dataPath + "'!");
                    Console.WriteLine(e);
                    cont = false;
                }
            }
            

            if (!cont)
            {
                Console.ReadLine();
                return;
            }

            Console.WriteLine();
            Console.WriteLine();

            Data.Analyze(Data.Loaded);

            // Delete the downloaded file.
            File.Delete(filePath);

            Console.WriteLine();

            Console.ReadLine();
        }
    }
}
