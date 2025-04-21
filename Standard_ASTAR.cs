using System.Diagnostics;
using System.Numerics;

namespace ASTAR_SPEEDTEST
{
    internal class Standard_ASTAR : INode
    {
        public float accumulatedWeight = 0;

        public Position c;
        Position o;
        Position t;
        bool[,] grid;

        public Standard_ASTAR(Position c, Position o, Position t, bool[,] grid)
        {
            this.c = c;
            this.o = o;
            this.t = t;
            this.grid = grid;
        }

        public Standard_ASTAR(Position c, Position o, Position t, float accumulatedWeight, bool[,] grid) : this(c, o, t, grid)
        {
            this.c = c;
            this.o = o;
            this.t = t;
            this.accumulatedWeight = accumulatedWeight + 1;
            this.grid = grid;

            //Debug.DrawRay((Vector2)c, Vector2.up * 0.3f, Color.blue);
        }

        public List<INode> GetNeighbours()
        {
            List<INode> neighbours = new List<INode>();
            List<Position> offsets = new List<Position>() 
            { 
                new Position (0, -1),
                new Position (0, 1),
                new Position (-1, 0),
                new Position (1, 0),
            };

            for (int i = 0; i < offsets.Count; i++)
            {
                Position newPos = c + offsets[i];
                if (newPos.x < 0 || newPos.y < 0 || newPos.x >= grid.GetLength(0) || newPos.y >= grid.GetLength(1))
                {

                }
                else
                {
                    neighbours.Add(new Standard_ASTAR(newPos, o, t, accumulatedWeight, grid));
                }
            }

            /*neighbours.Add(new Standard_ASTAR(c + new Position(0, 1), o, t, accumulatedWeight, grid));
            neighbours.Add(new Standard_ASTAR(c + new Position(0, -1), o, t, accumulatedWeight, grid));
            neighbours.Add(new Standard_ASTAR(c + new Position(1, 0), o, t, accumulatedWeight, grid));
            neighbours.Add(new Standard_ASTAR(c + new Position(-1, 0), o, t, accumulatedWeight, grid));*/

            return neighbours;
        }

        public string GetIdentifier()
        {
            return $"{c.x}_{c.y}";
        }

        public bool CheckIfObstructed()
        {
            if (c.x < 0 || c.y < 0 || c.x >= grid.GetLength(0) || c.y >= grid.GetLength(1))
                return false;

            return grid[c.x, c.y];

            /*Collider2D[] arr = Physics2D.OverlapBoxAll(c, Vector2.one, 0);
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].tag == "Obstacle")
                    return true;
            }

            return false;*/
        }

        public float Heuristic()
        {
            //float h = Vector2.Distance(c, o);
            float h = 0; //only accu
            //float g = Vector2.Distance(c, t);
            float g = MathF.Abs(c.x - t.x) + MathF.Abs(c.y - t.y); //manhattan

            //Debug.Log("Heu: " + (h + g + accumulatedWeight));
            //Debug.Log("Accu: " + (accumulatedWeight * 1));

            return h + g + accumulatedWeight * 1f;
        }

        public bool Validate(INode t)
        {
            if (t is Standard_ASTAR node)
            {
                return node.c.x == c.x && node.c.y == c.y;
            }
            return false;
        }
    }
}
