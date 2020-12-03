using System;
using System.Collections.Generic;
using System.IO;

namespace TobogganTrajectory.App
{
    class Program
    {
        static void Main(string[] args)
        {

            IList<string> map = File.ReadAllLines(@"map.txt"); 
            //  new List<string> { "..##.......","#...#...#..",".#....#..#.","..#.#...#.#",".#...##..#.","..#.##.....",".#.#.#....#",".#........#","#.##...#...","#...##....#",".#..#...#.#"};
            int xcoord = 0;
            int count = 0;

            foreach(string row in map)
            {
                if(row[xcoord] == '#')
                    ++count;
                xcoord = (xcoord + 3) % row.Length;
            }
            Console.WriteLine("part 1: {0}", count);


            IList<Tuple<int,int>> offsets = new List<Tuple<int,int>> { 
                new Tuple<int, int>(1,1),
                new Tuple<int, int>(3,1),
                new Tuple<int, int>(5,1),
                new Tuple<int, int>(7,1),
                new Tuple<int, int>(1,2)
                };

            int product = 1;
            foreach(Tuple<int,int> t in offsets)
            {
                int ycoord = 0;
                xcoord = 0;
                count = 0;
                while(ycoord < map.Count)
                {
                    if(map[ycoord][xcoord] == '#')
                        ++count;
                    xcoord = (xcoord + t.Item1) % map[0].Length;
                    ycoord += t.Item2;
                }
                product *= count;
            }
            
            Console.WriteLine("part 2: {0}", product);


        }
    }
}
