using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectSPA_ATS.Helpers
{
    class FileHelper
    {
        public static string? LoadFileContent(string[] args, string defaultFileName = "SimpleExample.txt")
        {
            string fileName = args.Length > 0 ? args[0] : defaultFileName;

            if (File.Exists(fileName))
            {
                try
                {
                    return File.ReadAllText(fileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Load file error: {ex.Message}");
                    return null;
                }
            }
            else
            {
                Console.WriteLine("File does not exist.");
                return null;
            }
        }
    }
}