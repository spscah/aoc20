using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomCustoms.App
{
    class Program
    {
        public static void Main (string[] args) {
            IList<string> responses = File.ReadAllLines(@"questions-answered.txt").ToList(); // needs the ToList() so we can add the extra new line 
            responses.Add(string.Empty);
            IList<int> answers = Enumerable.Range(1,26).Select(i => 0).ToList(); // create a list of 26 items 
            int total =  0;
            int total2 = 0;
            int groupcounter= 0;
            foreach(string answer in responses) {
                if(string.IsNullOrEmpty(answer)) {
                    total += answers.Where(a => a > 0).Count();
                    total2 += answers.Where(a=> a == groupcounter).Count();
                    groupcounter= 0;
                    answers.Clear();
                    for(int i = 0; i < 26; ++i) {
                        answers.Add(0);
                    }
                }
                else {
                    ++groupcounter;
                    foreach(char a in answer) {
                        answers[a-'a'] += 1;
                    }
                }
            }
            
            Console.WriteLine(total); // 7120
            Console.WriteLine(total2); // 3570
        }
    }
}
