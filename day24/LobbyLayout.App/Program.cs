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

            Debug.Assert(LayItOut(tiles) == 10);
            Debug.Assert(LayItOut(tiles, true) == 2208);

            tiles = File.ReadAllLines(@"tiles.txt");
            Debug.Assert(LayItOut(tiles) == 312);
            Console.WriteLine(LayItOut(tiles, true));

            Console.WriteLine("Hello World!");
        }

        static int LayItOut(IList<string> tiles, bool parttwo = false)
        {
            IList<Tile> listOfTiles = new List<Tile>();
            foreach(string route in tiles){
                Tile t = Tile.TileMaker(route);
                int i = listOfTiles.IndexOf(t);
                if(i < 0)
                    listOfTiles.Add(t);
                else    
                    listOfTiles[i].Flip();
            }
            if(!parttwo)
                return listOfTiles.Where(t => !t.IsWhite).Count();

            int rv = 0;
            for(int turn = 0; turn < 100; ++turn) {
                int minX = listOfTiles.Select(t => t.X).Min()-2;
                int maxX = listOfTiles.Select(t => t.X).Max()+2;
                int minY = listOfTiles.Select(t => t.Y).Min()-2;
                int maxY = listOfTiles.Select(t => t.Y).Max()+2;
                for(int x = minX; x <= maxX; ++x) 
                    for(int y = minY; y <= maxY; ++y)
                        if( Math.Abs(x%2) == Math.Abs(y%2)) 
                            Tile.EnsureExistence(x, y, listOfTiles);
                
                IList<Tile> toFlip = listOfTiles.Where(tf => tf.NeedsToFlip()).ToList(); //create the list                     
                foreach(Tile tf in toFlip) // then flip it, *must* be after the identification phase 
                    tf.Flip();
                rv = listOfTiles.Where(t => !t.IsWhite).Count();
                // Console.WriteLine("Day {0}: {1}", turn+1, rv);
            }
            return rv;
        }
    }

    class Tile : IEquatable<Tile> {

        readonly int _x;
        readonly int _y;
        bool _isWhite;
        readonly IList<Tile> _neighbours;
        public int X => _x;
        public int Y => _y;

        public bool IsWhite { get { return _isWhite; } }

        public bool NeedsNeighbours { get { return _neighbours.Count == 0; } }


        Tile(int x, int y, bool isW)
        {
            _x = x;
            _y = y;
            _isWhite = isW;
            _neighbours = new List<Tile>();
        }

        Tile(Tile t, int ox, int oy) : this(t._x + ox, t._y + oy, true) { }

        public static void EnsureExistence(int x, int y, IList<Tile> tiles) {
            Tile c = new Tile(x,y, true);
            int i = tiles.IndexOf(c);
            if(i < 0)
                tiles.Add(c);
            else {
                c = tiles[i];
                if(!c.NeedsNeighbours)
                    return;
            } 
            foreach(Tile t in new List<Tile> {  
                    new Tile(c, -1, 1), // nw i
                    new Tile(c, 1, 1), // ne 
                    new Tile(c, -1, -1), // sw 
                    new Tile(c, 1, -1), // se 
                    new Tile(c, -2, 0), // w 
                    new Tile(c, 2, 0)}) { // e 
                if(!tiles.Contains(t)) {
                    tiles.Add(t);
                    c._neighbours.Add(t);    
                } else { 
                    c._neighbours.Add(tiles[tiles.IndexOf(t)]); // if it's already there, use it. 
                }
            }
        }

        public static Tile TileMaker(string s) {
            int x, y;
            Calculate(s, out x, out y);
            return new Tile(x, y, false);
        }

        static void Calculate(string s, out int x, out int y) {
            if(string.IsNullOrEmpty(s)) {
                x = 0; 
                y = 0;
                return;
            }
            int ox = 0; int oy = 0;
            int offset = 0;
            if(s[0] == 'n' || s[0] == 's') {
                offset = 2;
                oy = s[0] == 'n' ? 1 : -1;
                ox = s[1] == 'e' ? 1 : -1;
            } else {
                offset = 1;
                ox = s[0] == 'w' ? -2 : 2; 

            }
            Calculate(s.Substring(offset), out x, out y);
            x += ox; 
            y += oy;
        }

        public bool NeedsToFlip() { 
            int blackNeighbours = _neighbours.Where(n => !n.IsWhite).Count();
            if(IsWhite)
                return blackNeighbours == 2;
            else    
                return blackNeighbours != 1 && blackNeighbours != 2;

        }

        public void Flip() {
            _isWhite = !_isWhite;
        }

        public bool Equals(Tile other) {
            return _x == other._x && _y == other._y;
        }

        public override string ToString()
        {
            return string.Format("({0},{1}): {2} [{3}/{4}]", _x, _y, IsWhite ? "white" : "black", _neighbours.Where(n => !n.IsWhite).Count(), _neighbours.Count);
        }
    }
}
