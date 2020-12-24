using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CrabCups.App
{
    class Program
    {
        static void Main(string[] args)
        {
            string i = "389125467"; // tmy est data
            Debug.Assert(PlayCups(i,false,10, false) == "92658374");
            Debug.Assert(PlayCups(i,false, debug: false) == "67384529");
            Debug.Assert(PlayCups(i,true, debug: false) == "149245887792");

            i = "916438275"; // puzzle input 
            Debug.Assert(PlayCups(i,false, debug: false) == "39564287");
            Debug.Assert(PlayCups(i,true, debug: false) == "404431096944");
            
            Console.WriteLine("Hello World!");
        }

        static string PlayCups(string incoming, bool partTwo, int v = 100, bool debug = true)
        {
            Node<int> circle = null; 
            Node<int> endoffront = null;
            int numberOfCups = partTwo ? 1000000 : 9;
            IList<Node<int>> lookup = Enumerable.Range(0,numberOfCups+1).Select(i => (Node<int>)null).ToList();
            foreach(int n in incoming.Select(d => d-'0')) { 
                Node<int> node = new Node<int>(n);
                lookup[n] = node;
                if(circle == null)
                    circle = node;
                else 
                    endoffront.Next = node;
                endoffront = node;
            }



            if(partTwo) {
                Node<int> frontofend = circle;
                // given a million nodes, this needs to be ideally linear but definitely non-recursive 
                for(int i = numberOfCups; i > incoming.Length; --i) {
                    Node<int> end = new Node<int>(i) { Next = frontofend} ;
                    lookup[i] = end;
                    frontofend = end;
                }
                endoffront.Next = frontofend;
                v = 10000000;
            }
            else
            // complete the circle 
                endoffront.Next = circle;

            Node<int> destination;
            Node<int> triple;
            for(int turn = 0; turn < v; ++turn) {
                if(debug) Console.WriteLine("turn {1}{2}cups: {0}", circle.ToString(circle.Value), turn+1, Environment.NewLine);
                // snip triple
                triple = circle.Next;
                circle.Next = triple.Next.Next.Next;
                if(debug) Console.WriteLine("pick up: {0}", triple.ToString(triple.Value));

                // locate destination - largest number not in the triple
                int target = circle.Value-1; 
                if(target == 0) target = numberOfCups;
                for(int i = 0; i < 3; ++i) {
                    if(triple.Value != target && triple.Next.Value != target & triple.Next.Next.Value != target)
                        break;
                    --target;
                    if(target == 0) target = numberOfCups;
                } 
                destination = lookup[target]; // circle.ReturnT(target);
                if(debug) Console.WriteLine("destination: {0}{1}", destination, Environment.NewLine);
                
                // place triple back down - could/should embed this in a loop?  
                triple.Next.Next.Next = destination.Next;
                destination.Next = triple;
                circle = circle.Next;
                triple = null;
            }
            circle = lookup[1]; // circle.ReturnT(1);
            if(partTwo) {
                return ((long)(circle.Next.Next.Value) * (long)(circle.Next.Value)).ToString(); 
            } 
            else
                return circle.ToString(circle.Value, string.Empty).Substring(1);
        }

    }




    class Node<T> where T : IComparable<T> { 
        readonly T _value;
        Node<T> _next; 

        public Node<T> Next { 
            get { return _next; }
            set { _next = value; }
        }

        public T Value { get { return _value; } }

        public Node(T value) {
            _value = value;
            _next = null;
        }

        public Node<T> ReturnT(T target) { // turns out that this is horrible with a million nodes 
            Node<T> pointer = this;
            while(pointer.Value.CompareTo(target) != 0)
                pointer = pointer.Next;
            return pointer;
        }

        public string ToString(T endpoint, string delimiter = " -> ") {
            return string.Format("{0}{1}", ToString(), (_next == null || _next._value.CompareTo(endpoint) == 0) ? string.Empty : delimiter + _next.ToString(endpoint, delimiter));
        }

        public override string ToString()
        {
            return _value.ToString();
        }

    }
}
