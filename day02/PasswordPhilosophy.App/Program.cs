using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PasswordPhilosophy.App
{
    class Program
    {
        static void Main(string[] args)
        {
            string pattern = @"(\d+)-(\d+) (\w): (\w+)";
            Regex re = new Regex(pattern, RegexOptions.IgnoreCase);
            int inrangeA = 0;
            int inrangeB = 0;

            IList<string> entries = new List<string> {"1-3 a: abcde", "1-3 b: cdefg", "2-9 c: ccccccccc"};
            entries = File.ReadAllLines(@"codes.txt");
            foreach(string entry in entries)
            {

                Match m = re.Match(entry);
                if (m.Success)
                {
                    int lower = Convert.ToInt32(m.Groups[1].Value);
                    int upper = Convert.ToInt32(m.Groups[2].Value);
                    char target = m.Groups[3].Value[0];
                    int instances = m.Groups[4].Value.Count(c => c == target);

                    
                    if(instances >= lower && instances <= upper)
                        ++inrangeA;
                    
                    if(m.Groups[4].Value[lower-1] == target ^ m.Groups[4].Value[upper-1] == target)
                        ++inrangeB;
                }
            }
            Console.WriteLine("number in range: {0}", inrangeA);
            Console.WriteLine("characters in positions: {0}", inrangeB);
        }
    }
}
