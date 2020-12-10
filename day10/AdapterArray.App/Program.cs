using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AdapterArray.App
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<int> jolts = File.ReadAllLines(@"jolts.txt").Select(j => int.Parse(j)).OrderBy(x => x).ToList();
            jolts.Add(jolts.Last()+3);
            int n = jolts.Count;
            int m = jolts.Last();
            Debug.Assert((4 * n * m - 3 * n * n - m * m) / 4 == 2475);
            
            IList<long> routes = new List<long>();
            for(int i = 0; i < jolts.Count; ++i) {
                routes.Add(jolts[i] <= 3 ? 1 : 0); 
                for(int j = -3; j < 0; ++j) 
                    if(i+j >= 0 && jolts[i+j] + 3 >= jolts[i])
                        routes[i] += routes[i+j];
            }
            Debug.Assert(routes.Last() == 442136281481216);
            Console.WriteLine("This is a good thing to see.");
        }
    }
}
