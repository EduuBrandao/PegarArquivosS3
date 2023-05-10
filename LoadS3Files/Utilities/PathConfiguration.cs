using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadS3Files.Utilities
{
    public static class PathConfiguration
    {
        public static string GetPath(string pathName)
        {
            return $"{pathName}/{GetFormattedDate()}";
        }

        public static string GetPath(string pathName, string fileName)
        {
            return $"{pathName}/{GetFormattedDate()}/{fileName}";
        }

        private static string GetFormattedDate()
        {
            return DateTime.Now.ToString("yyyy/MM/dd");
        }
    }
}