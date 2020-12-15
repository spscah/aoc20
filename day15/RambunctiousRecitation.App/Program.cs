using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RambunctiousRecitation.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.Assert(Game(new List<int> { 0,3,6}, 2020) == 436);
            Debug.Assert(Game(new List<int> { 1,3,2}, 2020) == 1);
            Debug.Assert(Game(new List<int> { 2,1,3}, 2020) == 10);
            Debug.Assert(Game(new List<int> { 1,2,3}, 2020) == 27);
            Debug.Assert(Game(new List<int> { 2,3,1}, 2020) == 78);
            Debug.Assert(Game(new List<int> { 3,2,1}, 2020) == 438);
            Debug.Assert(Game(new List<int> { 3,1,2}, 2020) == 1836);

            Debug.Assert(Game(new List<int>{ 2,0,1,7,4,14,18}, 2020) == 496);

            Debug.Assert(Game(new List<int> { 0,3,6}, 30000000) == 175594);
            Debug.Assert(Game(new List<int> { 1,3,2}, 30000000) == 2578);
            Debug.Assert(Game(new List<int> { 2,1,3}, 30000000) == 3544142);
            Debug.Assert(Game(new List<int> { 1,2,3}, 30000000) == 261214);
            Debug.Assert(Game(new List<int> { 2,3,1}, 30000000) == 6895259);
            Debug.Assert(Game(new List<int> { 3,2,1}, 30000000) == 18);
            Debug.Assert(Game(new List<int> { 3,1,2}, 30000000) == 362);

            Debug.Assert(Game(new List<int>{ 2,0,1,7,4,14,18}, 30000000) == 883);
            Console.WriteLine("perfect");
        }

        static int Game(List<int> sequence, int turns)
        {
            IDictionary<int, int> lastPlayed = sequence.ToDictionary(s => s, s=> sequence.IndexOf(s)+1);
            int currentTurn = sequence.Count;
            int nextNumber = 0;
            while(currentTurn < turns-1)
            {
                ++currentTurn;
                if(!lastPlayed.ContainsKey(nextNumber)) {
                    lastPlayed.Add(nextNumber, currentTurn);
                    nextNumber = 0;
                } else { 
                    int thisTurn = nextNumber;
                    nextNumber = currentTurn - lastPlayed[thisTurn];
                    lastPlayed[thisTurn] = currentTurn;                    
                }
            }


            return nextNumber;
        }
    }
}
