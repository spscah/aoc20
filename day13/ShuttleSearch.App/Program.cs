using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ShuttleSearch.App
{
    class Program
    {
        static void Main(string[] args)
        {
            int earliest = 939;
            string shuttleString = "7,13,x,x,59,x,31,19";

            Debug.Assert(PartOne(earliest, shuttleString) == 295);

            earliest = 1011416;
            shuttleString = "41,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,37,x,x,x,x,x,911,x,x,x,x,x,x,x,x,x,x,x,x,13,17,x,x,x,x,x,x,x,x,23,x,x,x,x,x,29,x,827,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,19";
            Debug.Assert(PartOne(earliest, shuttleString) == 4135);

            // Given Tests            
            Debug.Assert(ChineseRemainderTheorem("17,x,13,19") == 3417);
            Debug.Assert(ChineseRemainderTheorem("67,7,59,61") == 754018);
            Debug.Assert(ChineseRemainderTheorem("67,x,7,59,61") == 779210);
            Debug.Assert(ChineseRemainderTheorem("67,7,x,59,61") == 1261476);
            Debug.Assert(ChineseRemainderTheorem("1789,37,47,1889") == 1202161486);

            // part two 
            Debug.Assert(ChineseRemainderTheorem(shuttleString) == 640856202464541); 
            Console.WriteLine("Nirvana");
        }

        // https://en.wikipedia.org/wiki/Chinese_remainder_theorem 
        static long ChineseRemainderTheorem(string shuttleString) {
            int count = 0;
            IList<Tuple<long,long>> pairs = new List<Tuple<long,long>>();
            foreach(string sh in shuttleString.Split(',')) {
                if(sh[0] >= '0' && sh[0] <= '9') {
                    long n = long.Parse(sh);
                    pairs.Add(new Tuple<long, long>(n, n-count < 0 ? 2 * n - count : n-count ) );
                }
                ++count;
            }

            // iterated use of Bézout's identity and modular inverses https://en.wikipedia.org/wiki/Chinese_remainder_theorem#Existence_(direct_construction)
            long bigN = pairs.Select(p => p.Item1).Aggregate((total, next) => total * next);
            long sum = 0;
            foreach(var pair in pairs)
            {  
                long bigNi = bigN/pair.Item1;
                sum +=  pair.Item2 * ModularInverse(bigNi, pair.Item1) * bigNi;
            }
            while(sum < 0) 
                sum += bigN;
            sum %= bigN;
            return sum; 
        }


        // https://en.wikipedia.org/wiki/Extended_Euclidean_algorithm#Modular_integers
        // return the result such that a*r % b == 1
        static long ModularInverse(long a, long m)
        {
            Tuple<long,long,long> res = EGCD(a, m);
            if(res.Item1 != 1)
                throw new Exception("Whoops");
            return res.Item2 % m;
        }

        // triple(gcd,x,y) s.t. ax + by = gcd  
        static Tuple<long,long,long> EGCD(long a, long b) {
            if (a == 0)
                return new  Tuple<long,long,long> (b, 0, 1);
            else {
                Tuple<long,long,long> res = EGCD(b % a, a);
                return new Tuple<long,long,long> (res.Item1, res.Item3 - (b / a) * res.Item2, res.Item2);
            }
        }

        static int PartOne(int earliest, string shuttleString)
        {
            IList<int> shuttles = shuttleString.Split(',').Where(s => s[0] >= '0' && s[0] <= '9').Select(s => int.Parse(s)).ToList();
            IList<int> waits = shuttles.Select(s => earliest%s == 0 ? 0 : s-(earliest%s)).ToList();
            int index = waits.IndexOf(waits.Min());
            return waits[index] * shuttles[index];
        }
    }
}