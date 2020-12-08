using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Handheld.Assembler.App
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<string> instructions = File.ReadAllLines(@"day08.txt");
            CodeRunner cr = new CodeRunner();
            foreach(string i in instructions)
            {
                IList<string> parts = i.Split(' ').ToList();
                Operator op;
                int jump;

                if(Enum.TryParse(parts[0], out op) && int.TryParse(parts[1], out jump))
                {
                    cr.AddAnInstruction(new Instruction(op, jump));
                }
            }
            Debug.Assert(cr.FirstCycle() == 1317);    
            Debug.Assert(cr.FindCorruption() == 1033);

            Console.WriteLine("If you get here, all is good.");
        }
    }
}
