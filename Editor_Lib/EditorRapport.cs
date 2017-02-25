using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor_Lib
{
    public static class EditorRapport
    {
        public static void GenerateAnonymousRapport(string path)
        {
            StreamReader sr = new StreamReader(path);
        }

        public static List<String> FilterStrings(List<string> strings, string startsWith, string endsWith)
        {
            List<string> filteredStrings = new List<string>();

            foreach(string value in strings)
            {
                if(value.StartsWith && value.EndsWith)
                {

                }
            }
        }
    }
}
