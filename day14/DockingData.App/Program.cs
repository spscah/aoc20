using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DockingData.App
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<string> instructions = File.ReadAllLines(@"program.txt");

            Debug.Assert(PartOne(instructions) == 16003257187056);            
            Debug.Assert(PartTwo(instructions) == 3219837697833);

            Console.WriteLine("there's lovely.");
        }

        static long PartOne(IList<string> instructions)
        {
            IDictionary<int, long> data = new Dictionary<int, long>();
            long andMask = 0;
            long orMask = 0;

            string pattern = @"\[(\d+)\] = (\w+)$";
            Regex re = new Regex(pattern, RegexOptions.IgnoreCase);
            Match m;
            foreach(var instruction in instructions) {
                if(instruction.StartsWith("mask"))
                {
                    andMask = System.Convert.ToInt64(instruction.Substring(7).Replace('X', '1'), 2);
                    orMask = System.Convert.ToInt64(instruction.Substring(7).Replace('X', '0'),2);
                } else {
                   m = re.Match(instruction);
                   if(m.Success) {
                        int loc = int.Parse(m.Groups[1].Value);
                        long value = Convert.ToInt64(m.Groups[2].Value);
                        value |= orMask;
                        value &= andMask;
                        data[loc] = value;

                        if(!data.ContainsKey(loc))
                            data.Add(loc,value);
                        else
                            data[loc] = value;
                   }        
                }
            } 
            return data.Select(kvp => kvp.Value).Sum();
        }

        static long PartTwo(IList<string> instructions)
        {
            IDictionary<long, long> data = new Dictionary<long, long>();
            string mask = "";
            string pattern = @"\[(\d+)\] = (\w+)$";
            Regex re = new Regex(pattern, RegexOptions.IgnoreCase);
            Match m;
            foreach(var instruction in instructions) {
                if(instruction.StartsWith("mask"))
                {
                    mask = instruction.Substring(7);
                } else { 
                    m = re.Match(instruction);
                    if(m.Success) {
                        string location = Convert.ToString(long.Parse(m.Groups[1].Value), 2);
                        long value = Convert.ToInt64(m.Groups[2].Value);


                        location = location.PadLeft(36,'0');
                        StringBuilder sb = new StringBuilder();
                        for(int i=0; i < 36; ++i) {
                            if(mask[i] == '0')
                                sb.Append(location[i]);
                            else    
                                sb.Append(mask[i]);                            
                        }

                        foreach(long address in GenerateAlternatives(sb.ToString())) {
                            if(data.ContainsKey(address))
                                data[address] = value;
                            else   
                                data.Add(address, value);
                        }
                    }
                }
            }
            return data.Select(kvp => kvp.Value).Sum();
        }

        static IEnumerable<long> GenerateAlternatives(string maskedLocation) {
            int loc = maskedLocation.IndexOf('X');
            if(loc == -1)
                yield return Convert.ToInt64(maskedLocation, 2);
            else {
                StringBuilder sb1 = new StringBuilder(maskedLocation);
                sb1.Remove(loc,1);
                sb1.Insert(loc, '0');
                foreach(long x in GenerateAlternatives(sb1.ToString())) yield return x;
                sb1.Remove(loc,1);
                sb1.Insert(loc, '1');
                foreach(long x in GenerateAlternatives(sb1.ToString())) yield return x;
            }
        }

        static IList<int> Exes(string pattern)
        {
            IList<int> rv = new List<int>();
            int loc = pattern.IndexOf('X', 0);
            while(loc >= 0)
            {
                rv.Add(pattern.Length-1-loc);                
                loc = pattern.IndexOf('X', loc+1);
            }
            
            return rv.OrderBy(x => x).ToList();
        }
    }
}
