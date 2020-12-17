using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ConwayCubes.App
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<string> cycle0 = new List<string> { ".#.", "..#", "###"};
            // test
            Cubes c = new Cubes(cycle0, 6);
            c.Go(false);
            Debug.Assert(c.NumberActive == 112);
            c.Go(true);
            Debug.Assert(c.NumberActive == 848);

            // puzzle input
            cycle0 = new List<string> { ".#######","#######.","###.###.","#....###",".#..##..","#.#.###.","###..###",".#.#.##."};
            c = new Cubes(cycle0, 6);
            c.Go(false);
            Debug.Assert(c.NumberActive == 395);
            c.Go(true);
            Debug.Assert(c.NumberActive == 2296);
            Console.WriteLine("works for me");
        }
    } 



    class Cubes {
        readonly int _initialSize;
        readonly int _cycles;
        IList<bool> _state; 
        IList<string> _initialState;

        int XDim => 2*_cycles+_initialSize;
        int YDim => XDim;
        int ZDim => 2*_cycles+1;

        int WDim => 2*_cycles+1;

        public int NumberActive => _state.Count(c => c == true);

        public Cubes(IList<string> initialState, int c)
        {
            _cycles = c;
            _initialSize = initialState[0].Length;
            _initialState = initialState;
        }

        void Reset()
        {
            _state = CleanSlate();
            int w = 0;
            int z = 0;
            for(int y=0; y < _initialState.Count; ++y)
                for(int x=0; x < _initialState[0].Length; ++x)
                    if(_initialState[y][x] == '#')
                        _state[Transform(x+_cycles,y+_cycles,z+_cycles,w+_cycles)] = true;

        }
        public int Go(bool useW)
        {
            Reset();
            for(int i = 0; i < _cycles; ++i)
                Step(i+1, useW);
            return NumberActive;
        }

        IList<bool> CleanSlate() 
        {
            return Enumerable.Range(0,XDim*YDim*ZDim*WDim).Select(r => false).ToList();
        }
        void Step(int offset, bool useW)
        {
            IList<bool> newState = CleanSlate();
            for(int x = _cycles - offset; x  < XDim - _cycles + offset; ++x) {
                for(int y = _cycles - offset; y < YDim- _cycles + offset; ++y) {
                    for(int z = _cycles - offset; z < ZDim- _cycles + offset; ++z) {
                        for(int w = _cycles - offset; w < WDim- _cycles + offset; ++w) {
                            if(!useW && w != _cycles)
                                continue;
                        
                            int i = Transform(x,y,z,w);
                            int count = Neighbours(x,y,z,w).Where(n => _state[n]).Count();
                            if(_state[i]) 
                                newState[i] = (count == 2 || count == 3);
                            else    
                                newState[i] = count == 3; 
                        }
                    }
                }
            }
            _state = newState;
        }

        IEnumerable<int> Neighbours(int x, int y, int z, int w)
        {
            int origin = Transform(x,y,z,w);
            foreach(int ox in new List<int> { -1, 0, 1}) {
                if(x + ox < 0)
                    continue;
                if(x + ox >= XDim)
                    continue;
                foreach(int oy in new List<int> { -1, 0, 1}) {
                    if(y + oy < 0)
                        continue;
                    if(y + oy >= YDim)
                        continue;
                    foreach(int oz in new List<int> { -1, 0, 1}) {
                        if(z + oz < 0)
                            continue;
                        if(z + oz >= ZDim)
                            continue;
                        foreach(int ow in new List<int> { -1, 0, 1}) {
                            if(w + ow < 0)
                                continue;
                            if(w + ow >= WDim)
                                continue;
                            if(ox == 0 && oy == 0 && oz == 0 && ow == 0) 
                                continue;
                            int rv = Transform(x+ox, y+oy, z+oz, w+ow);
                            yield return rv;                        
                        }
                    }
                }
            }                        
        }

        int Transform(int x, int y, int z, int w) {
            return WTransform(w) + ZTransform(z) + YTransform(y) + x;
        }

        int YTransform(int y) {
            return y * XDim;
        }

        int ZTransform(int z) {
            return z * XDim * YDim;
        }

        int WTransform(int w) {
            return w * XDim * YDim * ZDim;
        }

    }
}