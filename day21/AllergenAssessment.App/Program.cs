using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AllergenAssessment.App
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<string> ingredients = new List<string> { 
                "mxmxvkd kfcds sqjhc nhms (contains dairy, fish)",
                "trh fvjkl sbzzf mxmxvkd (contains dairy)",
                "sqjhc fvjkl (contains soy)",
                "sqjhc mxmxvkd sbzzf (contains fish)",
            };

            string canonical = null;
            Debug.Assert(WhatDoesNotKillMe(ingredients, out canonical) == 5);
            Debug.Assert(canonical == "mxmxvkd,sqjhc,fvjkl");

            ingredients = File.ReadAllLines(@"ingredients.txt");
            Debug.Assert(WhatDoesNotKillMe(ingredients, out canonical) == 2280);
            Debug.Assert(canonical == "vfvvnm,bvgm,rdksxt,xknb,hxntcz,bktzrz,srzqtccv,gbtmdb");

            Console.WriteLine("shangri-la");
        }

        private static int WhatDoesNotKillMe(IList<string> ingredients, out string canonical)
        {
            IDictionary<string, IList<string>> allergens = new Dictionary<string, IList<string>>();
            string pattern = @"(.*) \(contains (.*)\)";
            Regex re = new Regex(pattern, RegexOptions.IgnoreCase);

            IList<string> bigListOfIngredients = new List<string>();
            foreach(string entry in ingredients)
            {
                Match m = re.Match(entry);
                if (m.Success)
                {
                    IList<string> items = m.Groups[1].Value.Split(' ').ToList();
                    foreach(string i in items) bigListOfIngredients.Add(i);
                    foreach(string a in m.Groups[2].Value.Split(", ")) {
                        items = items.Select(i => i).ToList();
                        if(!allergens.ContainsKey(a))
                            allergens[a] = items;
                        else
                            allergens[a] = allergens[a].Intersect(items).ToList();
                    }
                }
            }

            IList<string> identified = new List<string>();
            bool amended = true;
            while(amended) {
                string next = allergens.Where(kvp => !identified.Contains(kvp.Key) && kvp.Value.Count == 1).Select(kvp => kvp.Key).FirstOrDefault();
                if(string.IsNullOrEmpty(next)) {
                    amended = false;
                    continue;
                }
                string foreign = allergens[next][0];
                identified.Add(next);
                foreach(KeyValuePair<string, IList<string>> kvp in allergens) {
                    if(kvp.Key != next)
                        kvp.Value.Remove(foreign);
                    bigListOfIngredients = bigListOfIngredients.Where(i => i != foreign).Select(s => s).ToList();          
                }
            }
 
            canonical = identified.OrderBy(i => i).Select(a => allergens[a][0]).Aggregate((h,t) => string.Format("{0},{1}",h,t));

            return bigListOfIngredients.Count;
        }
    }
}
