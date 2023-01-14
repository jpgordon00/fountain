using System.CommandLine;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace fountain
{
    /// <summary>
    /// Contains generic utilities relating to strings.
    /// </summary>
    public static class StringUtils
    {
        
        private static System.Threading.Timer _timer;

    public static string PrintMessage;
    public static void StartPrinting(int interval)
    {
        _timer = new System.Threading.Timer((state) =>
        {
            Console.WriteLine(PrintMessage);
        }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(interval));
    }

    public static void StopPrinting()
    {
        _timer.Dispose();
    }

        public static string[] ParseCommaOrSpaceSeperatedString(string input, char delim1 = ' ', char delim2 = ',')
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            // Split the input string on spaces
            string[] splitOnSpaces = input.Split(new char[] { delim1 }, StringSplitOptions.RemoveEmptyEntries);

            // Create a list to store the resulting strings
            List<string> result = new List<string>();

            // Split each string on commas and add the resulting strings to the list
            foreach (string s in splitOnSpaces)
            {
                string[] splitOnCommas = s.Split(new char[] { delim2 }, StringSplitOptions.RemoveEmptyEntries);
                if (s.Trim() == "") continue;
                result.AddRange(splitOnCommas);
            }

            // Return the resulting list as an array
            return result.ToArray();
        }
    }
}