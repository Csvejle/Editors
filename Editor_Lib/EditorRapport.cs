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
        public static void GenerateAnonymousRapport(string fullPath)
        {
            
        }

        public static Dictionary<string, List<string>> CreateRapportDictonary(string fullPath)
        {
            Dictionary<string, List<string>> rapportDictonary = new Dictionary<string, List<string>>();
            foreach (string line in File.ReadAllLines(fullPath))
            {
                // TR TD Search Criteria
                if (true) {
                    if (rapportDictonary.ContainsKey("All"))
                    {
                        rapportDictonary["All"].Add(line);
                    }
                    else
                    {
                        rapportDictonary.Add("All", new List<string>());
                        rapportDictonary["All"].Add(line);
                    }
                }
                
            }
        }

        public static List<String> FilterStrings(List<string> strings, string startsWith, string endsWith)
        {
            List<string> filteredStrings = new List<string>();

            foreach (string value in strings)
            {
                if (value.StartsWith(startsWith) && value.EndsWith(endsWith))
                {
                    filteredStrings.Add(value.Substring(value.IndexOf(startsWith), value.LastIndexOf(endsWith) - value.IndexOf(startsWith)));
                }
            }

            return filteredStrings;
        }
    }
}
