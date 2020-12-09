using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EncodingError.App
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<long> numbers = File.ReadAllLines(@"encoding.txt").Select(n => long.Parse(n)).ToList();
            int preamble = 25;
            IList<long> targets = Enumerable.Range(0, numbers.Count-preamble).Select(i => numbers[i+preamble]).ToList();
            for(int target = preamble; target < numbers.Count; ++target) {
                for(int lower = target-preamble; lower < target-1; ++lower) {
                    for(int upper = lower + 1; upper < target; ++upper) {
                        if(numbers[lower] + numbers[upper] == numbers[target] && targets.Contains(numbers[target]))
                            targets.Remove(numbers[target]);
                    }
                }
            }

            long targetS = targets[0];
            Console.WriteLine("part 1: {0}", targetS);
            IList<long> stretchy = new List<long>();
            foreach(long n in numbers)
            {
                long s = stretchy.Sum();
                if(s == targetS)
                    break;
                while(stretchy.Sum()+n > targetS)
                    stretchy.RemoveAt(0);
                stretchy.Add(n);
            }         

            Console.WriteLine("part 2: {0}", (stretchy.Min() + stretchy.Max()));
        }
    }
}
