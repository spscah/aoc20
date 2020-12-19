using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MonsterMessages.App
{
    class Program
    {
        static void Main(string[] args)
        { 
            IList<string> rules = new List<string> { 
                "0: 4 1 5",
                "1: 2 3 | 3 2",
                "2: 4 4 | 5 5",
                "3: 4 5 | 5 4",
                "4: \"a\"",
                "5: \"b\"", 
                "",
                "ababbb",
                "bababa",
                "abbbab",
                "aaabbb",
                "aaaabbb"
                };

            Debug.Assert(CountMatchingRules(rules, false) == 2);
            rules = File.ReadAllLines(@"rules.txt");
            Debug.Assert(CountMatchingRules(rules, false) == 171);

            // part two
            rules = File.ReadAllLines(@"rules-example.txt");
            Debug.Assert(CountMatchingRules(rules, false) == 3);
            Debug.Assert(CountMatchingRules(rules, true) == 12);

            rules = File.ReadAllLines(@"rules.txt");
            Debug.Assert(CountMatchingRules(rules, true) == 369 );
            Console.WriteLine("10!");
        }

        static int CountMatchingRules(IList<string> rules, bool part2)
        {
            string pattern = @"(\d+): (.*)$";
            Regex re = new Regex(pattern, RegexOptions.IgnoreCase);
            Match m;

            IList<Node> nodes = new List<Node>(); 
            foreach(string rule in rules) {
                if(string.IsNullOrEmpty(rule)) break;
                nodes.Add(null);
            }
            // hack
            int extra = 43-nodes.Count;
            for(; extra > 0; --extra)
                nodes.Add(null);

            bool ruleUpdating = true;
            int valid = 0;
            foreach(string rule in rules) {
                if(ruleUpdating) {
                    if(string.IsNullOrEmpty(rule)) {
                        foreach(Node n in nodes.Where(n => n != null))
                            n.Update(nodes);
                        ruleUpdating = false;
                        continue;
                    }

                    m = re.Match(rule);
                    if (m.Success)
                    {
                        int index = int.Parse(m.Groups[1].Value);
                        Node newNode = null;
                        if(index == 0 && part2) {
                            newNode = new ZeroNode(m.Groups[2].Value);
                        } else if(index == 8 && part2) {
                            newNode = new LoopNode1(m.Groups[2].Value); 
                        } else if(index == 11 && part2) {
                            newNode = new LoopNode2(m.Groups[2].Value); 
                        } else if(m.Groups[2].Value.Contains('|')) {
                            newNode = new AnyNode(m.Groups[2].Value);
                        } else if(m.Groups[2].Value.StartsWith('"')) {
                                newNode = new LeafNode(m.Groups[2].Value);
                        } else {
                            newNode = new AllNode(m.Groups[2].Value);
                        }                        
                        nodes[index] = newNode;
                    }
                } else {
                    string copy = rule;
                    if(nodes[0].Matches(ref copy) && copy.Length == 0)
                        ++valid;
                }

            }
            return valid;
        }        
    }

    abstract class Node {
        public string Text;
        public Node(string t) { 
            Text = t;
        }

        public abstract bool Matches(ref string s);
        public abstract void Update(IList<Node> nodes);

        public override string ToString()
        {
            return Text;
        }
    }


    class LeafNode : Node
    {
        public readonly char Value; 

        public LeafNode(string t) : base(t) 
        {
            Value = t[1];
        }
        public override bool Matches(ref string s)
        {
            if(s.Length == 0)
                return false;
            if(s[0] == Value) {
                s = s.Substring(1);
                return true;
            }
            return false;
        }

        public override void Update(IList<Node> nodes)
        {

        }

        public override string ToString()
        {
            return string.Format("Leaf: " + base.ToString());
        }

    }

    class AllNode : Node
    {
        public Node _a;
        public Node _b;
        public Node _c;

        public AllNode(string t) : base(t)
        {

        }

        public override bool Matches(ref string s)
        {
            string t = s;
            if(!_a.Matches(ref t)) 
                return false;
            if(_b != null) {
                if(!_b.Matches(ref t))
                    return false;
                if(_c != null) {
                    if(!_c.Matches(ref t))
                        return false;
                }
            }
            s = t;
            return true;
        }

        public override void Update(IList<Node> nodes)
        {
            IList<string> kids = Text.Split(' ');
            _a = nodes[int.Parse(kids[0])];
            _a.Update(nodes);
            if(kids.Count > 1) { 
                _b = nodes[int.Parse(kids[1])];
                _b.Update(nodes);
            }
            if(kids.Count > 2) {
                _c = nodes[int.Parse(kids[2])];
                _c.Update(nodes);
            }
            
        }

        public override string ToString()
        {
            return string.Format("All: " + base.ToString());
        }

    }

    class ZeroNode : AllNode {
        public ZeroNode(string t) : base(t) {
            

        }

        public override string ToString()
        {
            return string.Format("Zero: " + base.ToString());
        }

        public override void Update(IList<Node> nodes)
        {
            base.Update(nodes);
        }

        public override bool Matches(ref string s){ 

            string t = s;
            while(_a.Matches(ref t)) {
                string t2 = t;
                if(_b.Matches(ref t2)) {
                    s = t2;
                    return true;
                }
            }
            s = t;
            return false;
        }


    }

    class LoopNode1 : AllNode
    {
        public LoopNode1(string t) : base(t) {
            _b = null;
        }
        public override bool Matches(ref string s)
        {
            string t = s;
            if(base.Matches(ref t)) {
                s = t;
                return true; // fudge for the context of needing to parse 11 (the LoopNode2) before 8 (the LoopNode1) 
/*                
                string t2 = t;
                if(this.Matches(ref t2)) {
                    s = t2;
                    return true;
                } else { 
                    s = t2;
                    return true;
                }
                */
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("Loop: " + base.ToString());
        }

        public override void Update(IList<Node> nodes)
        {
            IList<string> kids = Text.Split(' ');
            _a = nodes[int.Parse(kids[0])];
            _a.Update(nodes);
        }
    }

    class LoopNode2 : AllNode { 
        public LoopNode2(string t) : base(t) {
        }
        public override bool Matches(ref string s)
        {
            string t = s;
            if(_a.Matches(ref t)) {
                string t2 = t;
                if(this.Matches(ref t2)) {
                    string t3 = t2;
                    if(_b.Matches(ref t3)) {
                        s = t3;
                        return true;
                    } 
                } else { 
                    if(_b.Matches(ref t2)) {
                        s = t2;
                        return true;
                    } 

                }
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("Loop: " + base.ToString());
        }

        public override void Update(IList<Node> nodes)
        {
            IList<string> kids = Text.Split(' ');
            _a = nodes[int.Parse(kids[0])];
            _a.Update(nodes);
            _b = nodes[int.Parse(kids[1])];
            _b.Update(nodes);
        }

    }

    class AnyNode : Node
    {
        public AllNode _a;
        public AllNode _b;

        public AnyNode(string t) : base(t) { }
        public override bool Matches(ref string s)
        {
            string t = s;
            if(_a.Matches(ref t)) {
                s = t;    
                return true;
            }
            t = s;
            if(_b.Matches(ref t)) {
                s = t;
                return true;
            }

            return false;
        }

        public override void Update(IList<Node> nodes)
        {
            IList<string> children = Text.Split(" | ").ToList();
            _a = new AllNode(children[0]);
            _a.Update(nodes);
            _b = new AllNode(children[1]);
            _b.Update(nodes);
        }

        public override string ToString()
        {
            return string.Format("Any: " + base.ToString());
        }

    }

}
