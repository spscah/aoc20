using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HandyHaversacks.App
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<string> rules = File.ReadAllLines(@"rules.txt").ToList();
            IDictionary<string, IList<ColourBags>> bagContains = new Dictionary<string, IList<ColourBags>> ();    
            IDictionary<string, SortedSet<string>> canBeIn = new Dictionary<string, SortedSet<string>>();
            
            string outerpattern = @"([a-z ]+) bags contain(.*)$";
            string innerpattern =  @"((\d) (.*?)bags?[,|\.])";

            Regex reO = new Regex(outerpattern, RegexOptions.IgnoreCase);
            Regex reI = new Regex(innerpattern, RegexOptions.IgnoreCase);
            foreach(string rule in rules){
                Match m = reO.Match(rule);                
                if (m.Success)
                {
                    IList<ColourBags> cblist = new List<ColourBags>();
                    MatchCollection mc = reI.Matches(m.Groups[2].Value);
                    string outer = m.Groups[1].Value.Trim();
                    foreach(Match m2 in mc) {           
                        string inner = m2.Groups[3].Value.Trim();                        
                        int number = Int32.Parse(m2.Groups[2].Value);
                        if(canBeIn.ContainsKey(inner))
                            canBeIn[inner].Add(outer);
                        else    
                            canBeIn.Add(inner, new SortedSet<string>{outer});
                        cblist.Add(new ColourBags(number, inner));
                    }
                    bagContains.Add(outer, cblist);
                }
            }
            Queue<string> q = new Queue<string>();
            SortedSet<string> canbeinside = new SortedSet<string> (canBeIn["shiny gold"].Select(c => c));
            foreach(string c in canbeinside)
                q.Enqueue(c);
            while(q.Count > 0) {
                string thisone = q.Dequeue();
                if(canBeIn.ContainsKey(thisone))
                    foreach(string colour in canBeIn[thisone]){
                        if(canbeinside.Add(colour))
                            q.Enqueue(colour);
                }
            }

            Console.WriteLine(canbeinside.Count); //348 
            Console.WriteLine(HowManyInside("shiny gold", bagContains)); //18885
        }
        static int HowManyInside(string colour, IDictionary<string, IList<ColourBags>> bagContains)
        {
            if(!bagContains.ContainsKey(colour))
                return 0;
            int total = 0;
            foreach(ColourBags cb in bagContains[colour])
                total += cb.Number + cb.Number * HowManyInside(cb.Colour, bagContains);
            return total;
        }
    }

    class ColourBags { 
        public int Number;
        public string Colour;

        public ColourBags(int n, string c)
        {
            Number = n;
            Colour = c;
        }
    }
}
