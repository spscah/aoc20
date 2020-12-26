using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace LobbyLayout.App
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<string> tiles = new List<string> { 
                "sesenwnenenewseeswwswswwnenewsewsw",
                "neeenesenwnwwswnenewnwwsewnenwseswesw",
                "seswneswswsenwwnwse",
                "nwnwneseeswswnenewneswwnewseswneseene",
                "swweswneswnenwsewnwneneseenw",
                "eesenwseswswnenwswnwnwsewwnwsene",
                "sewnenenenesenwsewnenwwwse",
                "wenwwweseeeweswwwnwwe",
                "wsweesenenewnwwnwsenewsenwwsesesenwne",
                "neeswseenwwswnwswswnw",
                "nenwswwsewswnenenewsenwsenwnesesenew",
                "enewnwewneswsewnwswenweswnenwsenwsw",
                "sweneswneswneneenwnewenewwneswswnese",
                "swwesenesewenwneswnwwneseswwne",
                "enesenwswwswneneswsenwnewswseenwsese",
                "wnwnesenesenenwwnenwsewesewsesesew",
                "nenewswnwewswnenesenwnesewesw",
                "eneswnwswnwsenenwnwnwwseeswneewsenese",
                "neswnwewnwnwseenwseesewsenwsweewe",
                "wseweeenwnesenwwwswnew"
            };

            Debug.Assert(CubicGrid(tiles) == 10);
            Debug.Assert(CubicGrid(tiles, true) == 2208);

            tiles = File.ReadAllLines(@"tiles.txt");
            Debug.Assert(CubicGrid(tiles) == 312);
            Debug.Assert(CubicGrid(tiles, true) == 3733); 

            Console.WriteLine("That'll do pig, that'll do.");
        }

        // cube co-ordinates: 3-axes: nw-se (Ll), ne-sw (Rr), e-w (+1,-1) in those orders  
        // but each nw can be replaced by a ne & w (Re) and each se by a sw & e (re), so we can use a 2d representation of a 3d location 
        static int CubicGrid(IList<string> tiles, bool parttwo = false) {
            tiles = tiles.Select(t => t.Replace("nw", "Rw").Replace("ne", "R").Replace("se", "re").Replace("sw", "r")).ToList();
            int dim = tiles.Select(t => t.Length).Max() + (parttwo ? 101 : 0); // 101 to allow for 100 turns, plus one for the neighbours 
            int sz = dim * 2 + 1;
            bool[,] cubicgrid = new bool[sz,sz];

            foreach(string tile in tiles.Where(t => !string.IsNullOrEmpty(t))) {
                int ne = dim;
                int e = dim;
                foreach(char c in tile) {
                    switch(c) {
                        case('R'): ++ne; break; 
                        case('r'): --ne; break; 
                        case('e'): ++e; break;
                        case('w'): --e; break;
                    }
                }
                cubicgrid[ne,e] = !cubicgrid[ne,e]; 
            }

            if(parttwo) {
            
                for(int turn = 0; turn < 100; ++turn) {
                    IList<Tuple<int,int>> flips = new List<Tuple<int,int>>();
                    for(int ne = 1; ne < sz-1; ++ne) // [1,sz-1) to allow for the neighbours 
                    for(int  e = 1;  e < sz-1; ++e) {
                        int flippedneighbours = 0;
                        for(int one = -1; one <= 1; ++one) for(int oe = -1; oe <= 1; ++oe) if(one != oe && cubicgrid[ne+one, e+oe]) ++flippedneighbours; 
                        if((cubicgrid[ne,e] && (flippedneighbours == 0 || flippedneighbours > 2))   
                           || (!cubicgrid[ne,e] && flippedneighbours == 2))
                            flips.Add(new Tuple<int, int>(ne,e));

                    }
                    foreach(Tuple<int,int> f in flips)
                        cubicgrid[f.Item1, f.Item2] = !cubicgrid[f.Item1, f.Item2];
                }
            }
            
            return cubicgrid.Cast<bool>().Where(c => c == true).Count();
        }

    }
}
