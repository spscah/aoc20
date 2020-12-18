using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace OperationOrder.App
{
    class Program
    {
        static void Main(string[] args)
        {
            // part 1
            Debug.Assert(Evaluate("2 * 3 + (4 * 5)", false) == 26);
            Debug.Assert(Evaluate("5 + (8 * 3 + 9 + 3 * 4 * 3)", false) == 437);
            Debug.Assert(Evaluate("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", false) == 12240);
            Debug.Assert(Evaluate("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", false) == 13632);

            // tricky customers 
            Debug.Assert(Evaluate("9 + 3 * ((2 * 7) * 4 + 9 + 8 * (6 + 5) * 7) * 5 + 9", false) == 337269);
            Debug.Assert(Evaluate("(3 * 9) * 3 + 4 * 6 * (4 * 4 * 2 * 9 * 7 * 4) + 3", false) == 4112643);
            Debug.Assert(Evaluate("(3 * 3 + 3 + 4 + (3 + 9)) * (2 + 6 * 6 + (2 + 2)) + 4 * 6 + 4 * (2 + 2)", false) == 35056);

            IList<string> calculations = File.ReadAllLines(@"calculations.txt");
            Debug.Assert(calculations.Select(c => Evaluate(c, false)).Sum() == 21993583522852);

            // part 2
            Debug.Assert(Evaluate("1 + (2 * 3) + (4 * (5 + 6))", true) == 51);
            Debug.Assert(Evaluate("2 * 3 + (4 * 5)", true) == 46);
            Debug.Assert(Evaluate("5 + (8 * 3 + 9 + 3 * 4 * 3)", true) == 1445);
            Debug.Assert(Evaluate("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", true) == 669060);
            Debug.Assert(Evaluate("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", true) == 23340);

            Debug.Assert(calculations.Select(c => Evaluate(c, true)).Sum() == 122438593522757);

            Console.WriteLine("success");
        }

        static string RemoveDouble(string s, bool part2)
        {
            int open1 = s.IndexOf('(');
            if(open1 == -1)
                return s;
            int open2 = s.IndexOf('(', open1+1);
            if(open2 == -1)
                return s;
            int close1 = s.IndexOf(')');
            if(close1 < open2)
                return s;

            int count = 2;
            int i = open2+1;
            for(; i < s.Length; ++i) {
                if(s[i] == '(') ++count;
                if(s[i] == ')') --count;
                if(count == 0)
                    break;
            }
            string part = s.Substring(open1, i-open1+1);
            s = s.Replace(part, Evaluate(part.Substring(1, part.Length-2), part2).ToString());            
            return RemoveDouble(s, part2);
        }

        static long Evaluate(string s, bool part2) {
            s = RemoveDouble(s, part2);

            string lazy = @"(\(.*?\))";
            Regex re = new Regex(lazy, RegexOptions.IgnoreCase);
            Match m = re.Match(s);
            if (m.Success)
            {
                string part = m.Groups[1].Value;
                return Evaluate(s.Replace(part, Evaluate(part.Substring(1, part.Length-2), part2).ToString()), part2);
            }

            IList<string> tokens = s.Split(' ').ToList();
            if(part2) {
                int pointer = 1;
                while(pointer < tokens.Count) {
                    if(tokens[pointer] == "+") {
                        tokens[pointer-1] = (long.Parse(tokens[pointer-1]) + long.Parse(tokens[pointer+1])).ToString();
                        for(int i = 0; i < 2; ++i)
                            tokens.RemoveAt(pointer);
                    } 
                    else 
                        pointer += 2;
                }
                if(tokens.Count == 1)
                    return long.Parse(tokens[0]);
            }
            Queue<string> numbers = new Queue<string> (tokens);

            long value = long.Parse(numbers.Dequeue());            
            while(numbers.Count > 0) {
                if(numbers.Dequeue() == "+")
                    value += long.Parse(numbers.Dequeue());
                else
                    value *= long.Parse(numbers.Dequeue());
            }
            return value;
        }
    }
}
