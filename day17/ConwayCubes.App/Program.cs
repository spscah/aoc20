using System;
using System.Collections.Generic;


namespace ConwayCubes.App
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<string> cycle0 = new List<string> { ".#.", "..#", "###"};
            
            Cubes c = new Cubes(cycle0, 6);
            Console.WriteLine(c.NumberActive);
            Console.WriteLine("works for me");
        }


    } 

    class Cubes {
        readonly int _initialSize;
        readonly int _cycles;
        IList<bool> _state; 

        int XDim => 2*_initialSize+_cycles;
        int YDim => XDim;
        int ZDim => 2*_initialSize+1;

        public int NumberActive => _state.Count(c => c == true);

        public Cubes(IList<string> initialState, int c)
        {
            _cycles = c;
            _initialSize = initialState[0].Length;
            _state = Enumerable.Range(0,XDim*YDim*ZDim).Select(r => false).ToList();
            int z = 0;
            for(int y=0; y < initialState.Count; ++y)
                for(int x=0; x < initialState[0].Length; ++x)
                    if(initialState[y][x] == '#')
                        _state[Transform(x,y,z)] = true;


        }

        int Transform(int x, int y, int z) {
            return ZTransform(z) + YTransform(y) + x;
        }

        int YTransform(int y) {
            return (y + _cycles) * XDim;
        }

        int ZTransform(int z) {
            return (z + _cycles) * XDim * YDim;
        }
    }
}