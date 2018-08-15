using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistrictAnalysis
{
    public static class Data
    {
        public static string[][] Loaded;
        public static List<LineState> States = new List<LineState>();

        public const int DISTRICT_NAME_INDEX = 0;
        public const int GENERAL_EMAIL_INDEX = 1;
        public const int FIRST_NAME_INDEX = 2;
        public const int TERM_ENDS_INDEX = 6;
        public const int KNOWN_EMAIL_INDEX = 4;

        public static void Load(string path, int startTrim, int endTrim)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new Exception("Path given was null or empty! Cannot load data!");

            if (!File.Exists(path))
                throw new Exception(string.Format("File '{0}' does not exist, cannot load data!", path));

            var oldColour = Console.ForegroundColor;            
            Console.ForegroundColor = ConsoleColor.Cyan;

            var lines = File.ReadLines(path).ToList();
            lines.RemoveRange(0, startTrim);
            for (int i = 0; i < endTrim; i++)
            {
                lines.RemoveAt(lines.Count - 1);
            }

            // Go through the lines and seperate by commas.
            List<string[]> temp = new List<string[]>();

            int whitespace = 0;
            int notCSV = 0;

            const char SEP = ',';

            foreach(var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    whitespace++;
                    continue;
                }

                if (!line.Contains(SEP))
                {
                    notCSV++;
                    continue;
                }

                string[] split = line.Split(SEP);

                temp.Add(split);
            }

            // Now put that data into the permanent array.
            Loaded = temp.ToArray();
            temp.Clear();
            temp = null;

            // Print some stats.
            int bad = lines.Count - Loaded.Length;
            Console.WriteLine("Loaded " + lines.Count + " total lines - " + bad + " were unusable.");
            if (whitespace > 0)
                WriteColour(whitespace + " were blank lines.", ConsoleColor.Red);
            if (notCSV > 0)
                WriteColour(notCSV + " were not in CSV format.", ConsoleColor.Red);

            Console.ForegroundColor = oldColour;
        }

        public static void WriteColour(string text, ConsoleColor colour, params object[] other)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = colour;
            Console.WriteLine(text, other);
            Console.ForegroundColor = old;
        }

        public static LineState GetState(string[] line)
        {
            bool hasName = !string.IsNullOrWhiteSpace(line[FIRST_NAME_INDEX]);
            bool hasDates = !string.IsNullOrWhiteSpace(line[TERM_ENDS_INDEX]);
            bool hasGeneralEmail = !string.IsNullOrWhiteSpace(line[GENERAL_EMAIL_INDEX]);

            int[] indices = new int[] {5, 10, 15, 20, 25, 30, 35, 40, 45 };
            bool hasGuessed = false;
            foreach (var i in indices)
            {
                if (!string.IsNullOrWhiteSpace(line[i]) && line[i].Trim() != "---" && line[i].Trim() != "-")
                {
                    hasGuessed = true;
                    break;
                }
            }
            bool hasKnownEmail = false;
            foreach (var i in indices)
            {
                if (!string.IsNullOrWhiteSpace(line[i - 1]))
                {
                    hasKnownEmail = true;
                    break;
                }
            }

            if (hasName && hasDates && hasKnownEmail)
                return LineState.COMPLETE;

            if (hasName && hasKnownEmail && !hasDates)
                return LineState.MISSING_TERM_ENDS;

            if (hasName && !hasKnownEmail)
            {
                if (hasGeneralEmail)
                {
                    if (hasGuessed)
                        return LineState.MISSING_CERTAIN_EMAILS;
                    else
                        return LineState.ONLY_GENERAL_CONTACT;
                }
                else
                {
                    if (hasGuessed)
                        return LineState.MISSING_CERTAIN_EMAILS;
                    else
                        return LineState.MISSING_ANY_CONTACT;
                }
            }

            return LineState.INCOMPLETE;
        }

        public static void Analyze(string[][] data)
        {
            States.Clear();
            foreach(var line in Loaded)
            {
                var state = GetState(line);
                string name = line[DISTRICT_NAME_INDEX];

                States.Add(state);

                string district = name.Replace("school board", "").Trim();
                string stateString = state.ToString();

                WriteColour("{0, -45} {1, 25}", state.GetColour(), district, stateString);
            }

            int satisfying = States.Count(x => (byte)x <= 2);
            int incomplete = States.Count(x => x == LineState.INCOMPLETE);
            int percentage = (int)Math.Floor((double)satisfying / Loaded.Length * 100f);

            Console.WriteLine();
            Console.WriteLine();

            WriteColour("Overall " + percentage + "% satisfactory state: guessed emails or better.", ConsoleColor.White);
            WriteColour("There are " + incomplete + " left to complete.", ConsoleColor.White);

            // 8 for 6 lines

            float eurosPerDollar = ExchangeRateChecker.GetEurosPerDollar();
            float dollars = 10f;
            float lines = 6f;

            float moneyPerLine = dollars / lines;
            const int JAMES_START_LINE = 135;
            const int JAMES_END_LINE = 451;

            int jamesSat = 0;
            int jamesNotSat = 0;

            for (int i = JAMES_START_LINE; i < JAMES_END_LINE; i++)
            {
                if (i > States.Count)
                    break;

                if((byte)States[i] <= 2)
                {
                    jamesSat++;
                }
                else if(States[i] != LineState.INCOMPLETE)
                {
                    jamesNotSat++;
                }
            }

            Console.WriteLine();
            WriteColour("So far James has successfully completed " + jamesSat + " lines.", ConsoleColor.Blue);
            WriteColour("So far James has partially completed " + jamesNotSat + " lines.", ConsoleColor.DarkCyan);

            float money = jamesSat * moneyPerLine * eurosPerDollar;

            Console.WriteLine();
            WriteColour("{0, -20} {1, 4} {2, 7}", ConsoleColor.White, "Item", "QTY", "TOTAL");
            Console.WriteLine();
            WriteColour("{0, -20} {1, 4} {2, 7:N2}", ConsoleColor.DarkGray, "Completed Lines", "x" + jamesSat, money);
            Console.WriteLine();

        }
    } 
}
