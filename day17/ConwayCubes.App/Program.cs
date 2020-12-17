using System;
using System.Collections.Generic;
using System.Linq;

namespace ConwayCubes.App
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<string> cycle0 = new List<string> { ".#.", "..#", "###"};
            
            Cubes c = new Cubes(cycle0, 6);
            c.Go();
            Console.WriteLine(c.NumberActive);
            Console.WriteLine("works for me");
        }


    } 



    class Cubes {
        readonly int _initialSize;
        readonly int _cycles;
        IList<bool> _state; 

        int XDim => 2*_cycles+_initialSize;
        int YDim => XDim;
        int ZDim => 2*_cycles+1;

        public int NumberActive => _state.Count(c => c == true);

        public Cubes(IList<string> initialState, int c)
        {
            _cycles = c;
            _initialSize = initialState[0].Length;
            _state = CleanSlate();
            int z = 0;
            for(int y=0; y < initialState.Count; ++y)
                for(int x=0; x < initialState[0].Length; ++x)
                    if(initialState[y][x] == '#')
                        _state[Transform(x+c,y+c,z+c)] = true;
            Console.WriteLine(NumberActive);

        }

        public int Go()
        {
            for(int i = 0; i < _cycles; ++i)
                Step(i+1);
            return NumberActive;
        }

        IList<bool> CleanSlate() 
        {
            return Enumerable.Range(0,XDim*YDim*ZDim).Select(r => false).ToList();
        }
        void Step(int offset)
        {
            IList<bool> newState = CleanSlate();
            for(int x = _cycles - offset; x  < XDim - _cycles + offset; ++x) {
                for(int y = _cycles - offset; y < YDim- _cycles + offset; ++y) {
                    for(int z = _cycles - offset; z < ZDim- _cycles + offset; ++z) {
                        int i = Transform(x,y,z);
                        int count = Neighbours(x,y,z).Where(n => _state[n]).Count();
                        if(_state[i]) 
                            newState[i] = (count == 2 || count == 3);
                        else    
                            newState[i] = count == 3; 
                    }
                }
            }
            _state = newState;
            Console.WriteLine("{0}: {1}", offset, NumberActive);
        }



        IEnumerable<int> Neighbours(int x, int y, int z)
        {
            int origin = Transform(x,y,z);
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
                        if(ox == 0 && oy == 0 && oz == 0) 
                            continue;
                        int rv = Transform(x+ox, y+oy, z+oz);
                        yield return rv;                        
                    }
                }
            }                        
        }

        int Transform(int x, int y, int z) {
            return ZTransform(z) + YTransform(y) + x;
        }

        int YTransform(int y) {
            return y * XDim;
        }

        int ZTransform(int z) {
            return z * XDim * YDim;
        }
    }
}