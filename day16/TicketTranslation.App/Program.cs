using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TicketTranslation.App
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<string> rules = new List<string> { "class: 1-3 or 5-7","row: 6-11 or 33-44", "seat: 13-40 or 45-50","","your ticket:","7,1,14","","nearby tickets:","7,3,47","40,4,50","55,2,20","38,6,12"};
            
            Debug.Assert(PartOne(rules) == 71);
            Debug.Assert(PartOne(File.ReadAllLines(@"rules.txt")) == 25984);

            rules = new List<string> { "class: 0-1 or 4-19","row: 0-5 or 8-19","seat: 0-13 or 16-19","","your ticket:","11,12,13","","nearby tickets:","3,9,18","15,1,5","5,14,9"};

            Debug.Assert(PartTwo(rules) == 1);
            Debug.Assert(PartTwo(File.ReadAllLines(@"rules.txt")) == 1265347500049);

            Console.WriteLine("happy place");
        }

        static long PartTwo(IList<string> rules) {
            string pattern = @"^([a-z ]+): (\d+)-(\d+) or (\d+)-(\d+)";
            Regex re = new Regex(pattern, RegexOptions.IgnoreCase);

            IDictionary<string, Range> validityRanges = new Dictionary<string, Range>();
            IList<Range> validity = new List<Range>();
            int linecount = 0;
            for(; ; ++linecount) {
                string rule = rules[linecount];
                if(string.IsNullOrEmpty(rule))
                    break;
                Match m = re.Match(rule);
                if (m.Success) {
                    int f1 = int.Parse(m.Groups[2].Value);
                    int t1 = int.Parse(m.Groups[3].Value);
                    int f2 = int.Parse(m.Groups[4].Value);
                    int t2 = int.Parse(m.Groups[5].Value);
                    Range r = new Range(f1, t1, f2, t2);
                    validity.Add(r);

                    validityRanges[m.Groups[1].Value] = r;                    
                }
            }

            Debug.Assert(string.IsNullOrEmpty(rules[linecount]));
            ++linecount;
            Debug.Assert(rules[linecount].StartsWith("your ticket"));
            ++linecount;
           int myticketline = linecount;
 
            IList<int> values = rules[linecount].Split(',').Select(v => int.Parse(v)).ToList();
            IList<IList<string>> options = new List<IList<string>>();
            foreach(int value in values)
            {
                IList<string> thisOption = new List<string>();
                options.Add(thisOption);
                foreach(KeyValuePair<string, Range> kvp in validityRanges)
                    if(kvp.Value.InRange(value))
                        thisOption.Add(kvp.Key);
            }

            ++linecount;
            Debug.Assert(string.IsNullOrEmpty(rules[linecount]));
            ++linecount;
            Debug.Assert(rules[linecount].StartsWith("nearby"));
            ++linecount;

            int bookmark = linecount;
            IList<string> validTickets = new List<string>();
            for(; linecount < rules.Count; ++linecount) {
                bool add = true;
                foreach(int value in rules[linecount].Split(',').Select(r => int.Parse(r)))
                    if(!validity.Any(v => v.InRange(value)))
                        add = false;
                if(add) validTickets.Add(rules[linecount]);
            }

            linecount = 0;
            for(; linecount < validTickets.Count; ++linecount) {
                values = validTickets[linecount].Split(',').Select(r => int.Parse(r)).ToList();
                for(int i = 0; i < values.Count; ++i) {
                    IList<string> currentOptions = options[i];
                    IList<string> newOptions = currentOptions.Where(o => validityRanges[o].InRange(values[i])).ToList();
                    options[i] = newOptions;
                    if(newOptions.Count == 1) 
                        RemoveOptions(options, newOptions[0], i);
                }
            }

            long prod = 1;
            IList<int> myticket = rules[myticketline].Split(',').Select(v => int.Parse(v)).ToList();

            for(int i = 0; i < options.Count; ++i)
                if(options[i][0].StartsWith("departure"))
                    prod *= myticket[i];
            

            return prod;

        }
        static void RemoveOptions(IList<IList<string>> options, string solo, int avoid)
        {
            for(int i = 0; i < options.Count; ++i) { 
                if(i != avoid) { 
                    if(options[i].Count > 1) {
                        options[i].Remove(solo);
                        if(options[i].Count == 1)
                            RemoveOptions(options, options[i][0], i);
                    }
                }
            }
        }

        static int PartOne(IList<string> rules) {
            string pattern = @"(\w+): (\d+)-(\d+) or (\d+)-(\d+)";
            Regex re = new Regex(pattern, RegexOptions.IgnoreCase);

            IDictionary<int,bool> validity = new Dictionary<int,bool>();
            int linecount = 0;
            for(; ; ++linecount) {
                string rule = rules[linecount];
                if(string.IsNullOrEmpty(rule))
                    break;
                Match m = re.Match(rule);
                if (m.Success) {
                    int f1 = int.Parse(m.Groups[2].Value);
                    int t1 = int.Parse(m.Groups[3].Value);
                    int f2 = int.Parse(m.Groups[4].Value);
                    int t2 = int.Parse(m.Groups[5].Value);
                    for(int i = f1; i <= t1; ++i)
                        validity[i] = true;
                    for(int i = f2; i <= t2; ++i)
                        validity[i] = true;                    
                }
            }

            pattern = @"(\d+),(\d+),(\d+)";
            re = new Regex(pattern, RegexOptions.IgnoreCase);

            IList<int> invalids = new List<int> ();
            for(; linecount < rules.Count; ++linecount) {
                if(!string.IsNullOrEmpty(rules[linecount]) && rules[linecount][0] >= '0' && rules[linecount][0] <= '9')
                    foreach(int value in rules[linecount].Split(',').Select(r => int.Parse(r)))
                        if(!validity.ContainsKey(value))
                            invalids.Add(value);
            }            

            return invalids.Sum();
        }
    }
    class Range
    {
        readonly int _f1;
        readonly int _f2;
        readonly int _t1;
        readonly int _t2;

        public Range(int f1, int t1, int f2, int t2) 
        {
            _f1 = f1; _t1 = t1; _f2 = f2; _t2 = t2;
        }

        public bool InRange(int value) {
            return (value >= _f1 & value <= _t1) || (value >= _f2 && value <= _t2);
        }
    }
}
