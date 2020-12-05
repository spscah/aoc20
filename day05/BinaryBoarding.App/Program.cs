using System;
using System.Collections.Generic;
using System.IO;

namespace BinaryBoarding.App
{
    class Program
    {
        static void Main(string[] args)
        {
            const int BITS = 10;
            bool[] array = new bool[1 << BITS];
            ushort part1 = 0;
            foreach(string seat in File.ReadAllLines(@"seats.txt"))
            {
                ushort b = 0;
                for(int i = 0; i < BITS; ++i)
                {
                    b |= (ushort)((seat[i] == 'B' || seat[i] == 'R') ? (1 << (BITS-1-i)) : 0);
                }
                array[b] = true;
                if(b > part1)
                    part1 = b;
            }
            Console.WriteLine("Part 1: {0}", part1);
            ushort part2 = 0;
            for(ushort j = 1; j < part1-1; ++j)
                if(!array[j] && array[j-1] && array[j+1])
                    part2 = j;
            Console.WriteLine("Part 2: {0}", part2);
        }
    }
}
