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
        private readonly static List<string> StartsWithList = new List<string>() { @"<td valign=""middle"" style=""line-height: 24px; font-size: 14px; word-break: break-word; -webkit-hyphens: none; -moz-hyphens: none; hyphens: none; border-collapse: collapse !important; vertical-align: top; text-align: left; color: #222; font-family: Helvetica,Arial,sans-serif; font-weight: 400; background-color: #e2e2e2; margin: 0; padding: 3px;"" height=""24"" align=""left"">", @"<td valign=""middle"" style=""line-height: 24px; font-size: 14px; word-break: break-word; -webkit-hyphens: none; -moz-hyphens: none; hyphens: none; border-collapse: collapse !important; vertical-align: top; text-align: left; color: #222; font-family: Helvetica,Arial,sans-serif; font-weight: 400; background-color: #f3f3f3; margin: 0; padding: 3px;"" height=""24"" align=""left"">" };
        private readonly static string EndsWith = @"</td>&#13;";
        private readonly static string SplitString = @"<tr style=""vertical-align: top; text-align: left; padding: 0;"" align=""left"">";
        public static Dictionary<string, List<string>> Rapport { get; private set; }
        public static Dictionary<string, List<string>> AnonymousRapport { get; private set; }
        public static Dictionary<string, string> AnonymousNodes { get; private set; }
        public static Dictionary<string, string> AnonymousExtra { get; private set; }
        public static List<int> Counters = new List<int>() { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        public static List<string> Extensions = new List<string>() { "_BA", "_EXC", "_SYS", "_SQL", "_VM" };
        public static bool DisableExtension = false;

        public static void GenerateAnonymousRapport(string fullPath)
        {
            Rapport = CreateRapportDictonary(fullPath);
            AnonymousNodes = new Dictionary<string, string>();
            Dictionary<string, List<string>> copy = Rapport.ToDictionary(entry => entry.Key, entry => entry.Value.ToList());
            AnonymousRapport = GenerateAnonymousNames(copy);

            int v1 = 1, v2 = 1, v3 = 1, v4 = 1;

            // Extra/IP
            List<String> keys = AnonymousRapport.Keys.ToList();

            // <old partId, new partId>
            Dictionary<string, string> ip1 = new Dictionary<string, string>();
            Dictionary<string, string> ip2 = new Dictionary<string, string>();
            Dictionary<string, string> ip3 = new Dictionary<string, string>();
            Dictionary<string, string> ip4 = new Dictionary<string, string>();
            // <old ip, new ip>
            Dictionary<string, string> extra = new Dictionary<string, string>();

            for (int i = 0; i < keys.Count(); i++)
            {
                if (!keys[i].Equals(""))
                {
                    if (keys[i].Contains('.'))
                    {
                        string[] ips2 = keys[i].Split('.');


                        if (!ip1.ContainsKey(ips2[0]))
                        {
                            string newIp = "" + (v1);
                            ip1.Add(ips2[0], newIp);
                            v1++;
                        }

                        if (!ip2.ContainsKey(ips2[1]))
                        {
                            string newIp = "" + (v2);
                            ip2.Add(ips2[1], newIp);
                            v2++;
                        }

                        if (!ip3.ContainsKey(ips2[2]))
                        {
                            string newIp = "" + (v3);
                            ip3.Add(ips2[2], newIp);
                            v3++;
                        }

                        if (!ip4.ContainsKey(ips2[3]))
                        {
                            string newIp = "" + (v4);
                            ip4.Add(ips2[3], newIp);
                            v4++;
                        }

                        string ip_string = ip1[ips2[0]] + "." + ip2[ips2[1]] + "." + ip3[ips2[2]] + "." + ip4[ips2[3]];

                        extra.Add(keys[i], ip_string);
                    }
                    else
                    {
                        string oldValue = keys[i];

                        bool found2 = false;
                        string currentExtension = "_BA";

                        foreach (string ext in Extensions)
                        {
                            if (keys[i].EndsWith(ext))
                            {
                                found2 = true;
                                currentExtension = ext;
                                break;
                            }
                        }

                        if (found2 && !DisableExtension)
                        {
                            keys[i] = keys[i].Substring(0, keys[i].Length - currentExtension.Length);
                        }

                        List<int> indices = SearchChar(keys[i], '_');

                        int currentIndex = 0;
                        int k = 0;
                        keys[i] = "";

                        for (; k < Counters.Count && k < indices.Count; k++)
                        {
                            keys[i] += "" + Counters[k] + "_";
                            currentIndex = indices[k];
                            Counters[k] += 1;
                        }

                        keys[i] += Counters[k];

                        if (!DisableExtension)
                        {
                            keys[i] += currentExtension;
                        }

                        Counters[k] += 1;

                        extra.Add(oldValue, keys[i]);
                    }
                }
            }
            extra.Add("", "");

            AnonymousExtra = extra;

            ChangeNodes(fullPath, AnonymousRapport, AnonymousExtra);
        }


        public static Dictionary<string, List<string>> CreateRapportDictonary(string fullPath)
        {
            Dictionary<string, List<string>> rapportDictonary = new Dictionary<string, List<string>>();

            bool isNode = false;
            bool isIp = false;
            string prevLine = "empty";

            foreach (string line in File.ReadAllLines(fullPath))
            {
                foreach (string startsWith in StartsWithList)
                {
                    string trimmedLine = line.Trim();

                    if (trimmedLine.StartsWith(SplitString) && !isNode && !isIp)
                    {
                        isNode = true;
                    }
                    else
                    {
                        if (isNode && trimmedLine.StartsWith(startsWith) && trimmedLine.EndsWith(EndsWith))
                        {
                            prevLine = trimmedLine;
                            isNode = false;
                            isIp = true;
                        }
                        else
                        {
                            if (isIp && trimmedLine.StartsWith(startsWith) && trimmedLine.EndsWith(EndsWith))
                            {
                                isIp = false;
                                string key = trimmedLine.Substring(startsWith.Length, trimmedLine.Length - startsWith.Length - EndsWith.Length);

                                if (!rapportDictonary.ContainsKey(key))
                                {
                                    rapportDictonary[key] = new List<string>();
                                }
                                rapportDictonary[key].Add(prevLine);
                            }
                        }
                    }
                }
            }

            List<string> keys = rapportDictonary.Keys.ToList<string>();
            for (int i = 0; i < keys.Count(); i++)
            {
                rapportDictonary[keys[i]] = FilterStrings(rapportDictonary[keys[i]], StartsWithList, EndsWith);
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
            string currentExtension = "";
            List<string> keys = rapport.Keys.ToList<string>();
            for (int i = 0; i < keys.Count(); i++)
            {
                for (int j = 0; j < rapport[keys[i]].Count; j++)
                {
                    string oldNode = rapport[keys[i]][j];

                    bool found2 = false;
                    currentExtension = "_BA";

                    foreach (string ext in Extensions)
                    {
                        if (rapport[keys[i]][j].EndsWith(ext))
                        {
                            found2 = true;
                            currentExtension = ext;
                            break;
                        }
                    }

                    if (found2 && !DisableExtension)
                    {
                        rapport[keys[i]][j] = rapport[keys[i]][j].Substring(0, rapport[keys[i]][j].Length - currentExtension.Length);
                    }

                    List<int> indices = SearchChar(rapport[keys[i]][j], '_');

                    int currentIndex = 0;
                    int k = 0;
                    rapport[keys[i]][j] = "";

                    for (; k < Counters.Count && k < indices.Count; k++)
                    {
                        rapport[keys[i]][j] += "" + Counters[k] + "_";
                        currentIndex = indices[k];
                        Counters[k] += 1;
                    }

                    rapport[keys[i]][j] += Counters[k];
                    
                    if(!DisableExtension)
                    {
                        rapport[keys[i]][j] += currentExtension;
                    }

                    Counters[k] += 1;

                    if (!AnonymousNodes.ContainsKey(oldNode))
                    {
                        AnonymousNodes.Add(oldNode, rapport[keys[i]][j]);
                    }
                }
            }


            return rapport;
        }



        public static List<String> FilterStrings(List<string> strings, List<string> startsWithList, string endsWith)
        {
            List<string> filteredStrings = new List<string>();

            foreach (string value in strings)
            {
                foreach (string startsWith in startsWithList)
                {
                    string filteredValue = value.Trim();

                    if (filteredValue.StartsWith(startsWith) && filteredValue.EndsWith(endsWith))
                    {
                        filteredStrings.Add(filteredValue.Substring(startsWith.Length, filteredValue.Length - startsWith.Length - endsWith.Length));
                    }
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



        private static void ChangeNodes(string fullPath, Dictionary<string, List<string>> rapport, Dictionary<string, string> ips)
        {
            List<string> file = new List<string>();

            foreach (string line in File.ReadAllLines(fullPath))
            {
                file.Add(line);
            }

            for (int i = 0; i < file.Count(); i++)
            {
                string trimmedLine = file[i].Trim();
                foreach (string startsWith in StartsWithList)
                    if (trimmedLine.StartsWith(startsWith) && trimmedLine.EndsWith(EndsWith))
                    {
                        int firstIndex = file[i].IndexOf(startsWith);
                        int lastIndex = file[i].LastIndexOf(EndsWith);
                        string oldValue = file[i].Substring(firstIndex + startsWith.Length, lastIndex - firstIndex - startsWith.Length);
                        string newValue = null;

                        if (AnonymousNodes.ContainsKey(oldValue))
                        {
                            newValue = AnonymousNodes[oldValue];
                        }

                        if (AnonymousExtra.ContainsKey(oldValue))
                        {
                            newValue = AnonymousExtra[oldValue];
                        }

                        if (newValue != null)
                        {
                            int index = i;
                            file[i] = file[i].Substring(0, firstIndex + startsWith.Length) + newValue + file[i].Substring(lastIndex, EndsWith.Length);
                        }
                    }
            }

            File.Delete(fullPath);
            StreamWriter sw = File.CreateText(fullPath);

            foreach (string line in file)
            {
                sw.WriteLine(line);
            }

        }
    }
}
