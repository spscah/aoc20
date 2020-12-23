using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CrabCombat.App
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<string> hands = new List<string> { "Player 1:","9","2","6","3","1","","Player 2:","5","8","4","7","10"};
            Debug.Assert(PartOne(hands) == 306);
            Debug.Assert(PartTwo(hands) == 291);

            hands = File.ReadAllLines(@"cards.txt");
            Debug.Assert(PartOne(hands) == 32489);
            Debug.Assert(PartTwo(hands) == 35676);

            Console.WriteLine("go. collect £200 as you pass.");
        }

        static int PartTwo(IList<string> hands) {
            IList<Queue<int>> queuesOfCards = new List<Queue<int>> { new Queue<int>(), new Queue<int>()}; // can't use the stack class as we add to both ends 

            int index = 0;
            foreach(string card in hands.Where(h => !h.StartsWith("Player"))) {
                if(string.IsNullOrEmpty(card)) {
                    index = 1;
                    continue;
                }
                queuesOfCards[index].Enqueue(int.Parse(card));
            }   

            int winner = PlayRecursiveCombat(queuesOfCards);
            int result = 0;
            while(queuesOfCards[winner].Count > 0)
                result += queuesOfCards[winner].Count * queuesOfCards[winner].Dequeue();
            
            return result;
        }

        static int HashGameState(IList<Queue<int>> queues) {
            unchecked 
            {
                int hash = 29;
                for(int i = 0; i < 2; ++i) {
                    hash = hash * 31 + i.GetHashCode();
                    foreach(int c in queues[i]) {
                        hash = hash * 31 + c.GetHashCode();
                    }
                }
                return hash;
            }
        }
        static int PlayRecursiveCombat(IList<Queue<int>> queuesOfCards)
        {
            for(int i = 0; i < 2; ++i)
                if(queuesOfCards[i].Count == 0)
                    return (i+1)%2;
            IList<int> previous = new List<int>{HashGameState(queuesOfCards)};
            IList<int> turns = new List<int> { -1, -1};
            int winner = -1;
            while(queuesOfCards[0].Count > 0 && queuesOfCards[1].Count > 0) {
                for(int i = 0; i < 2; ++i) 
                    turns[i] = queuesOfCards[i].Dequeue();
                
                if(queuesOfCards[0].Count >= turns[0] && queuesOfCards[1].Count >= turns[1]) {
                    IList<Queue<int>> playdown = Enumerable.Range(0,2).Select(i => new Queue<int>(queuesOfCards[i].Take(turns[i]).Select(t => t))).ToList();

                    winner = PlayRecursiveCombat(playdown);
                }
                else {
                    winner = turns[0] > turns[1] ? 0 : 1;
                }
                queuesOfCards[winner].Enqueue(turns[winner]);
                queuesOfCards[winner].Enqueue(turns[(winner + 1) %2]);
                int hash = HashGameState(queuesOfCards);
                if(previous.Contains(hash))
                    return 0;
                previous.Add(hash);
            }   
            if(winner == -1)
                winner = queuesOfCards[0].Count > 0 ? 0 : 1;
            return winner;
        }

        static long PartOne(IList<string> hands)
        {
            IList<Queue<long>> queuesOfCards = new List<Queue<long>> { new Queue<long>(), new Queue<long>()};

            int index = 0;
            foreach(string card in hands.Where(h => !h.StartsWith("Player"))) {
                if(string.IsNullOrEmpty(card)) {
                    index = 1;
                    continue;
                }
                queuesOfCards[index].Enqueue(long.Parse(card));
            }   
            IList<long> turn = new List<long> { 0,0 } ;        
            int winner = 0;
            while(queuesOfCards[0].Count > 0 && queuesOfCards[1].Count > 0) {
                for(int i = 0; i < 2; ++i)
                    turn[i] = queuesOfCards[i].Dequeue();
                winner = turn[0] > turn[1] ? 0 : 1;
                queuesOfCards[winner].Enqueue(turn[winner]);
                queuesOfCards[winner].Enqueue(turn[(winner + 1) %2]);
            }
            long result = 0;
            while(queuesOfCards[winner].Count > 0){
                result += queuesOfCards[winner].Count * queuesOfCards[winner].Dequeue();
            }
            return result;
        }
    }
}
