using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SeatingSystem.App
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<List<bool?>> seats = File.ReadAllLines(@"seats.txt")
                .Select(s =>  s.Select(c => (c == 'L') ? false : (bool?)null).ToList())
                .ToList();
           
            while(true)
            {
                IList<List<bool?>> clone = seats.Select(r => r.Select(c => c).ToList()).ToList();
                for(int r = 0; r < seats.Count; ++r)
                    for(int c = 0; c < seats[0].Count; ++c)
                    {
                        if(seats[r][c].HasValue)
                        {
                            int n = Neighbours(seats, r, c);
                            if(seats[r][c].Value && n >= 4)
                                clone[r][c] = false;
                            if(!seats[r][c].Value && n == 0)
                                clone[r][c] = true;
                        }
                    }

                if(Match(seats, clone))
                    break;
                seats = clone;
            }

            int seated = seats.Sum(r => r.Count(r => r.HasValue && r.Value));
            Debug.Assert(seated == 2438);        

            seats = File.ReadAllLines(@"seats.txt")
                .Select(s =>  s.Select(c => (c == 'L') ? false : (bool?)null).ToList())
                .ToList();

            while(true)
            {
                IList<List<bool?>> clone = seats.Select(r => r.Select(c => c).ToList()).ToList();
                for(int r = 0; r < seats.Count; ++r)
                    for(int c = 0; c < seats[0].Count; ++c)
                    {
                        if(seats[r][c].HasValue)
                        {
                            int n = Visible(seats, r, c);
                            if(seats[r][c].Value && n >= 5)
                                clone[r][c] = false;
                            if(!seats[r][c].Value && n == 0)
                                clone[r][c] = true;
                        }
                    }
                if(Match(seats, clone))
                    break;
                seats = clone;
            }
            seated = seats.Sum(r => r.Count(r => r.HasValue && r.Value));
            Debug.Assert(seated == 2174);       
            Console.WriteLine("das ist gut");
        }

        static bool Match(IList<List<bool?>> seats, IList<List<bool?>> clone)
        {
            for(int r = 0; r < seats.Count; ++r)
                for(int c = 0; c < seats[0].Count; ++c)
                    if(seats[r][c].HasValue && seats[r][c].Value != clone[r][c].Value)
                        return false;
            return true;
        }


        static int Visible(IList<List<bool?>> seats, int row, int column)
        {
            int count = 0;
            // up, same, down
            for(int r = -1; r <= 1; ++r)
                // left same right
                for(int c = -1; c <= 1; ++c) {
                    // check for no movement at all 
                    if(r != 0 || c != 0) {
                        // find the furthest edge, to limit the weight
                        int nearest = new List<int> { row, column, seats.Count-row, seats[0].Count - column}.Max();
                        // try to move multiple steps in each of the 8 directions 
                        for(int weight = 1; weight < nearest; ++weight) {
                            // if the seat is on the grid  
                            if(row+(r*weight) >= 0 && row+(r*weight) < seats.Count && column+(c*weight) >= 0 && column+(c*weight) < seats[0].Count) {
                                // increment the count if the seat is true 
                                count += (seats[row+(r*weight)][column+(c*weight)].HasValue && seats[row+(r*weight)][column+(c*weight)].Value) ? 1 : 0;
                                // if a seat exists, break out of the weight loop - all further options are hidden 
                                if(seats[row+(r*weight)][column+(c*weight)].HasValue)
                                    break;
                            }
                        }
                    }
                }
            return count;
        }

        static int Neighbours(IList<List<bool?>> seats, int row, int column)
        {
            int count = 0;
            for(int r = -1; r <= 1; ++r)
                for(int c = -1; c <= 1; ++c)
                    if(row+r >= 0 && row+r < seats.Count && column+c >= 0 && column+c < seats[0].Count)
                        if(r != 0 || c != 0)
                            count += (seats[row+r][column+c].HasValue && seats[row+r][column+c].Value) ? 1 : 0;
            return count;
        }
    }
}
