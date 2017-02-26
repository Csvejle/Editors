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
        private readonly static string StartsWith = @"<td valign=""middle"" style=""line-height: 24px; font-size: 14px; word-break: break-word; -webkit-hyphens: none; -moz-hyphens: none; hyphens: none; border-collapse: collapse !important; vertical-align: top; text-align: left; color: #222; font-family: Helvetica,Arial,sans-serif; font-weight: 400; background-color: #e2e2e2; margin: 0; padding: 3px;"" height=""24"" align=""left"">";
        private readonly static string EndsWith = @"</td>&#13;";
        private readonly static string SplitString = @"<tr style=""vertical-align: top; text-align: left; padding: 0;"" align=""left"">";
        public static Dictionary<string, List<string>> Rapport { get; private set; }
        public static Dictionary<string, List<string>> AnonymousRapport { get; private set; }

        public static void GenerateAnonymousRapport(string fullPath)
        {
            Rapport = CreateRapportDictonary(fullPath);
            Dictionary<string, List<string>>  copy = Rapport.ToDictionary(entry => entry.Key, entry => entry.Value.ToList());
            AnonymousRapport = GenerateAnonymousNames(copy);

            int v1 = 1, v2 = 1, v3 = 1, v4 = 1;

            // Extra/IP
            List<String> keys = AnonymousRapport.Keys.ToList();

            for (int i = 0; i < keys.Count(); i++)
            {
                if(!AnonymousRapport[keys[i]].Equals(""))
                {

                }
            }
        }


        public static Dictionary<string, List<string>> CreateRapportDictonary(string fullPath)
        {
            Dictionary<string, List<string>> rapportDictonary = new Dictionary<string, List<string>>();

            bool isNewSchedule = false;
            string prevLine = "empty";

            foreach (string line in File.ReadAllLines(fullPath))
            {
                string trimmedLine = line.Trim();

                if (!prevLine.Equals("empty") && isNewSchedule && trimmedLine.StartsWith(StartsWith) && trimmedLine.EndsWith(EndsWith))
                {
                    string key = trimmedLine.Substring(StartsWith.Length, trimmedLine.Length - StartsWith.Length - EndsWith.Length);

                    if (!rapportDictonary.ContainsKey(key))
                    {
                        rapportDictonary[key] = new List<string>();
                    }

                    rapportDictonary[key].Add(prevLine);

                    prevLine = "empty";
                    isNewSchedule = false;
                }
                else
                {
                    if (isNewSchedule && trimmedLine.StartsWith(StartsWith) && trimmedLine.EndsWith(EndsWith))
                    {
                        prevLine = trimmedLine;
                    }

                    if (trimmedLine.StartsWith(SplitString) && !isNewSchedule)
                    {
                        isNewSchedule = true;
                    }
                }
            }

            List<string> keys = rapportDictonary.Keys.ToList<string>();
            for (int i = 0; i < keys.Count(); i++)
            {
                rapportDictonary[keys[i]] = FilterStrings(rapportDictonary[keys[i]], StartsWith, EndsWith);
            }

            bool found = false;

            if (rapportDictonary.ContainsKey(""))
            {
                for (int i = 0; i < rapportDictonary[""].Count; i++)
                {
                    List<string> keys2 = rapportDictonary.Keys.ToList();
                    for (int j = 0; j < keys2.Count() && !found; j++)
                    {
                        for (int k = 0; k < rapportDictonary[keys2[j]].Count && !keys2[j].Equals("") && !found; k++)
                        {
                            if (rapportDictonary[""][i].Equals(rapportDictonary[keys2[j]][k]))
                            {
                                rapportDictonary[""].Remove(rapportDictonary[""][i]);
                                found = true;
                            }
                        }
                    }
                    found = false;
                }
            }

            return rapportDictonary;
        }

        public static Dictionary<string, List<string>> GenerateAnonymousNames(Dictionary<string, List<string>> rapport)
        {
            List<int> counters = new List<int>() { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            List<string> extensions = new List<string>() { "_BA", "_EXC", "_SYS", "_SQL", "_VM" };

            string currentExtension = "";
            List<string> keys = rapport.Keys.ToList<string>();
            for (int i = 0; i < keys.Count(); i++)
            {
                for (int j = 0; j < rapport[keys[i]].Count; j++)
                {
                    bool found2 = false;
                    currentExtension = "_BA";

                    foreach (string ext in extensions)
                    {
                        if (rapport[keys[i]][j].EndsWith(ext))
                        {
                            currentExtension = ext;
                            found2 = true;
                            break;
                        }
                    }

                    if (found2)
                    {
                        rapport[keys[i]][j] = rapport[keys[i]][j].Substring(0, rapport[keys[i]][j].Length - currentExtension.Length);
                    }

                    List<int> indices = SearchChar(rapport[keys[i]][j], '_');

                    int currentIndex = 0;
                    int k = 0;
                    rapport[keys[i]][j] = "";

                    for (; k < counters.Count && k < indices.Count; k++)
                    {
                        rapport[keys[i]][j] += "" + counters[k] + "_";
                        currentIndex = indices[k];
                        counters[k] += 1;
                    }

                    rapport[keys[i]][j] += counters[k] + currentExtension;
                    counters[k] += 1;
                }
            }


            return rapport;
        }

        public static List<String> FilterStrings(List<string> strings, string startsWith, string endsWith)
        {
            List<string> filteredStrings = new List<string>();

            foreach (string value in strings)
            {
                string filteredValue = value.Trim();

                if (filteredValue.StartsWith(startsWith) && filteredValue.EndsWith(endsWith))
                {
                    filteredStrings.Add(filteredValue.Substring(startsWith.Length, filteredValue.Length - startsWith.Length - endsWith.Length));
                }
            }

            return filteredStrings;
        }

        private static List<int> SearchChar(string value, char search)
        {
            List<int> indices = new List<int>();

            for (int i = 0; i < value.Length; i++)
            {
                char v = value[i];

                if (v == search)
                {
                    indices.Add(i);
                }
            }

            return indices;
        }
    }
}
