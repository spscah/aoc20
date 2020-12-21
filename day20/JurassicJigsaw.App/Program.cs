using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace JurassicJigsaw.App
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<Grid> grids = Grid.GridMaker("tiles-example.txt");
            IList<Grid> thereBeMonsters = null; 
            Debug.Assert(PartOne(grids, out thereBeMonsters) == 20899048083289);
            Console.WriteLine(PartTwo(thereBeMonsters)); 

            grids = Grid.GridMaker("tiles.txt");
            Debug.Assert(PartOne(grids, out thereBeMonsters) == 21599955909991);

            Console.WriteLine(PartTwo(thereBeMonsters)); 

            Console.WriteLine("tier zero");
        }

        static int PartTwo(IList<Grid> monsters)
        {
            int sideLength = (int)Math.Sqrt(monsters.Count);
            IList<IList<string>> woBorders = monsters.Select(m => m.WithoutBorders()).ToList();
            IList<string> image = new List<string>();
            int dimension = sideLength * woBorders[0][0].Length;
            for(int i = 0; i < sideLength; ++i) { // move down the x major blocks 
                for(int j = 0; j < dimension/sideLength; ++j) { // move across the y 
                    StringBuilder sb = new StringBuilder();
                    for(int k = 0; k < sideLength; ++k)  // move down the x minor block
                        sb.Append(woBorders[i*sideLength + k][j]);
                    image.Add(sb.ToString());
                }
            }

            Grid original = new Grid(0,image);
            IList<Grid> images = new List<Grid> { original};
            foreach(Transform t in Enum.GetValues(typeof(Transform)).Cast<Transform>().Where(t => t != Transform.Original))
                images.Add(new Grid(original, t));

            int rv = 0;
            foreach(Grid g in images) {
                rv = g.AreThereMonsters();
                if( rv > 0 ) return rv;               
            }
            return rv;
        }

        static long PartOne(IList<Grid> grids, out IList<Grid> forExport) { 
            // initialise a stack of lists of grids 
            Stack<IList<Grid>> stack = new Stack<IList<Grid>>();
            foreach(Grid g in grids)
                stack.Push(new List<Grid>{g});

            int sideLength = (int)Math.Sqrt(grids.Count/8); // hacky magic number - todo: use enum count 
            // dfs - pop the front list, go through the list and replace with all those not in the list 
            IList<Grid> successful = null; 
            while(stack.Count > 0)
            {
                IList<Grid> next = stack.Pop();
                // where are we placing the next one? 
                // to the right of which? 
                Grid afterMe = next.Count % sideLength == 0 ? null : next[next.Count-1];
                // below which?
                Grid belowMe = next.Count < sideLength ? null : next[next.Count-sideLength];

                foreach(Grid vc in grids.Where(g => (afterMe == null || afterMe.ValidCandidate(g, true)) && (belowMe == null || belowMe.ValidCandidate(g, false))).ToList()) { 
                    IList<Grid> candidate = next.Select(g => g).ToList();
                    candidate.Add(vc);
                    if(candidate.Count == sideLength * sideLength) {
                        successful = candidate;
                        break;
                    }
                    stack.Push(candidate);        
                }
                if(successful != null)
                    break;
                
            }
            if(successful != null) {
                forExport = successful;
                return (successful[0].Id * successful[sideLength-1].Id * successful[sideLength*(sideLength-1)].Id * successful[sideLength*sideLength-1].Id);
            }
            throw new Exception("That was not expected");
        }

    }

    enum Transform { Original, H, V, R1, R2, R3, H1, H3}
    class Grid : IComparable<Grid> {

        IList<string> _initialState;
        readonly int _dimension;
        readonly long _id;
        readonly Transform _transform;
        IList<string> _edges;

        public long Id { get { return _id; } }

        internal Grid(long id, IList<string> initial) {
            _id = id;
            _initialState = initial;
            _transform = Transform.Original;
            _dimension = initial.Count;
            string right = "";
            string left = "";
            for(int i = 0; i < _dimension; ++i) {
                left += _initialState[i][0];
                right += _initialState[i][_dimension-1];
            }
            _edges = new List<string> { 
                _initialState[0],
                right,
                _initialState[_dimension-1],
                left
            };            

        }

        internal Grid(Grid original, Transform t) : this(original._id, original._initialState)
        {
            _transform = t;
            TransformEdges(_transform);
        } 


        public IList<string> WithoutBorders() {
            IList<string> transformed = ActuallyTransform(_transform, _initialState, _dimension);
            IList<string> rv = new List<string>();
            for(int i = 1; i < transformed.Count-1; ++i)
                rv.Add(transformed[i].Substring(1, transformed[i].Length-2));
            return rv;
        }        

        public static IList<Grid> GridMaker(string filename) {
            IList<Grid> rv = new List<Grid>();
            IList<string> input = File.ReadAllLines(filename);
            for(int i = 0; i < input.Count;) {
                long id = long.Parse(input[i].Substring(5,4)); // slightly hacky - assumes 4 digit code 
                ++i;
                int limit = i + input[i].Length;  
                IList<string> state = new List<string>();
                for(; i < limit; ++i)
                    state.Add(input[i]);
                ++i; // blank line
                Grid grid = new Grid(id, state);
                rv.Add(grid);
                foreach(Transform t in Enum.GetValues(typeof(Transform)).Cast<Transform>().Where(t => t != Transform.Original))
                    rv.Add(new Grid(grid, t));
            }    

            return rv;
        }

        public static string Reverse( string s )
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse( charArray );
            return new string( charArray );
        }

        public static IList<string> ActuallyTransform(Transform t, IList<string> incoming, int dim) {
            switch(t) {
                case(Transform.Original):
                    return incoming;
                case(Transform.H):
                    IList<string> rv = new List<string>();
                    for(int i = 1; i <= dim; ++i)
                        rv.Add(incoming[dim-i]);
                    return rv;
                case(Transform.V):
                    return incoming.Select(s => Reverse(s)).ToList();
                case(Transform.R1):
                    return Enumerable.Range(0,dim).Select(i => Reverse(string.Join("", incoming.Select(s => s[i]).ToList()))).ToList();
                case(Transform.R2):
                    return ActuallyTransform(Transform.R1, ActuallyTransform(Transform.R1, incoming, dim), dim);
                case(Transform.R3):
                    return ActuallyTransform(Transform.R1, ActuallyTransform(Transform.R2, incoming, dim), dim);
                case(Transform.H1):
                    return ActuallyTransform(Transform.R1, ActuallyTransform(Transform.H, incoming, dim), dim);
                case(Transform.H3):
                    return ActuallyTransform(Transform.R3, ActuallyTransform(Transform.H, incoming, dim), dim);
            }
            throw new Exception("shouldn't get here");
        }

        public void TransformEdges(Transform t) {
            switch(t) {
                case(Transform.Original):
                    break;
                case(Transform.H):
                    _edges = new List<string> { _edges[2], Reverse(_edges[1]), _edges[0], Reverse(_edges[3])};
                    break;
                case(Transform.V):
                    _edges = new List<string> { Reverse(_edges[0]), _edges[3], Reverse(_edges[2]), _edges[1]};
                    break;
                case(Transform.R1):
                    _edges = new List<string> { Reverse(_edges[3]), _edges[0], Reverse(_edges[1]), _edges[2]}; 
                    break;
                case(Transform.R2):
                    TransformEdges(Transform.R1);
                    TransformEdges(Transform.R1);
                    break;
                case(Transform.R3):
                    TransformEdges(Transform.R1);
                    TransformEdges(Transform.R1);
                    TransformEdges(Transform.R1);
                    break;
                case(Transform.H1):
                    TransformEdges(Transform.H);
                    TransformEdges(Transform.R1);
                    break;
                case(Transform.H3):
                    TransformEdges(Transform.H);
                    TransformEdges(Transform.R1);
                    TransformEdges(Transform.R1);
                    TransformEdges(Transform.R1);
                    break;
            }

        }

        public bool ValidCandidate(Grid other, bool placeRight) {
            if(_id == other._id)
                return false;
            if(placeRight) // are we placing to the right 
                return string.Equals(_edges[1], other._edges[3]);
            else // we're placing below if not to the right 
                return string.Equals(_edges[2], other._edges[0]);
        }

        public override string ToString()
        {
            return string.Format("[{0}:{1}] {2},{3},{4},{5}", _id, _transform, _edges[0], _edges[1], _edges[2], _edges[3]);
        }

        public int CompareTo(Grid other)
        {
            if(_id == other._id) 
                return _transform.CompareTo(other._transform);
            return _id.CompareTo(other._id);
        }

        public int AreThereMonsters() {
            IList<string> pattern = new List<string> { 
                "                  # ",
                "#    ##    ##    ###",
                " #  #  #  #  #  #   "
            };
            int patternWidth = pattern[0].Length;
            IList<string> transformed = ActuallyTransform(_transform, _initialState, _dimension);
            int counter = 0;
            for(int x = 0; x < transformed[0].Length-patternWidth; ++x) {
                for(int y = 0; y < transformed.Count-pattern.Count; ++y) {
                    bool found = true;
                    for(int px = 0; px < pattern[0].Length; ++px) {
                        for(int py = 0; py < pattern.Count; ++py) {
                            if(pattern[py][px] == '#' && transformed[y+py][x+px] != '#')
                                found = false;
                        }
                    }
                    if(found) ++counter;
                }
            }
            if(counter == 0) 
                return 0;
            return transformed.Select(t => t.Count(c => c == '#')).Sum() - (pattern.Select(t => t.Count(c => c == '#')).Sum() * counter);
        }
    }
    
}
