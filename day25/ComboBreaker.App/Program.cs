using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ComboBreaker.App
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<ulong> publickeys = new List<ulong> { 5764801, 17807724}; 
            Debug.Assert(PartOne(publickeys) == 14897079);

            publickeys = new List<ulong> { 12092626, 4707356};
            Debug.Assert(PartOne(publickeys) == 18329280);
            Console.WriteLine("Hello World!");
        }

        private static ulong PartOne(IList<ulong> publickeys)
        {
            ulong card = publickeys[0];
            ulong door = publickeys[1];
            ulong generator = 20201227L;
            ulong value = 1; 
            ulong subject = 7;
            ulong res = 1;
            while(value != card) {
                value = (value * subject) % generator;
                res = (res * door) % generator;
            }            
            return res;
        }
    }
}
