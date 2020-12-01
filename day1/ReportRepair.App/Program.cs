using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ReportRepair.App
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<int> reports = File.ReadAllLines(@"report.txt").Select(x => Convert.ToInt32(x)).ToList();
            int result = Find2020(reports);
            Console.WriteLine(result);
            int result3 = Find2020Triple(reports);
            Console.WriteLine(result3);
        }

        static int Find2020Triple(IList<int> reports)
        {
            for(int i = 0; i < reports.Count; ++i)
            {
                for(int j = i+1; j < reports.Count; ++j)
                {
                    if(reports.Contains(2020-reports[i]-reports[j]))
                        return reports[i] * reports[j] * (2020-reports[i]-reports[j]);
                }
            }
            throw new Exception("triple not found");
        }

        static int Find2020(IList<int> reports)
        {
            foreach(int i in reports)
            {
                if(reports.Contains(2020-i))
                    return i * (2020-i);
            }
            throw new Exception("pair not found");
        }
    }
}
