namespace ASTAR_SPEEDTEST
{

    public class ASTAR_Generic<T> where T : INode
    {
        public List<T> GetPath(INode origin, INode target)
        {
            PriorityQueue<T> openSet = new PriorityQueue<T>();
            HashSet<string> closedSet = new HashSet<string>();
            Dictionary<T, T> cameFrom = new Dictionary<T, T>();

            int attempts = 7000;

            openSet.Enqueue((T)origin, origin.Heuristic());
            closedSet.Add(origin.GetIdentifier());

            T latest = (T)origin;

            while (attempts > 0)
            {
                //Debug.Log("openSet " + openSet.Count);
                //Debug.Log("closedSet " + closedSet.Count);
                //Debug.Log("cameFrom " + cameFrom.Count);

                attempts--;

                if (openSet.Count == 0)
                {
                    return TracePath(cameFrom, latest, new List<T>());
                }

                T current = openSet.Dequeue();

                latest = current;

                if (current.Validate(target))
                {
                    //Debug.Log("success");
                    //Console.WriteLine("Success");
                    List<T> returnList = new List<T>();

                    return TracePath(cameFrom, current, returnList);
                }

                foreach (T newNode in current.GetNeighbours())
                {
                    if (closedSet.Contains(newNode.GetIdentifier()))
                    {
                        //Console.WriteLine("Dublicate");

                        //Debug.Log("dublicate");
                        continue;
                    }

                    if (newNode.CheckIfObstructed())
                    {
                        //Console.WriteLine("Obstructed");

                        // Debug.Log("obstructed");
                        closedSet.Add(newNode.GetIdentifier());
                        continue;
                    }

                    cameFrom.Add(newNode, current);
                    openSet.Enqueue(newNode, newNode.Heuristic());
                    closedSet.Add(newNode.GetIdentifier());

                }
            }

            Console.WriteLine("Not found...");

            return new List<T>();
        }

        public int GetPath_NodeCount(INode origin, INode target)
        {
            PriorityQueue<T> openSet = new PriorityQueue<T>();
            HashSet<string> closedSet = new HashSet<string>();
            Dictionary<T, T> cameFrom = new Dictionary<T, T>();

            int nodeCount = 0;

            int attempts = 7000;

            openSet.Enqueue((T)origin, origin.Heuristic());
            closedSet.Add(origin.GetIdentifier());

            T latest = (T)origin;

            while (attempts > 0)
            {
                //Debug.Log("openSet " + openSet.Count);
                //Debug.Log("closedSet " + closedSet.Count);
                //Debug.Log("cameFrom " + cameFrom.Count);

                nodeCount = closedSet.Count;

                attempts--;

                if (openSet.Count == 0)
                {
                    return nodeCount;
                }

                T current = openSet.Dequeue();

                latest = current;

                if (current.Validate(target))
                {
                    //Debug.Log("success");
                    //Console.WriteLine("Success");
                    List<T> returnList = new List<T>();

                    return nodeCount;
                }

                foreach (T newNode in current.GetNeighbours())
                {
                    if (closedSet.Contains(newNode.GetIdentifier()))
                    {
                        //Console.WriteLine("Dublicate");

                        //Debug.Log("dublicate");
                        continue;
                    }

                    if (newNode.CheckIfObstructed())
                    {
                        //Console.WriteLine("Obstructed");

                        // Debug.Log("obstructed");
                        closedSet.Add(newNode.GetIdentifier());
                        continue;
                    }

                    cameFrom.Add(newNode, current);
                    openSet.Enqueue(newNode, newNode.Heuristic());
                    closedSet.Add(newNode.GetIdentifier());

                }
            }

            Console.WriteLine("Not found...");

            return nodeCount;
        }


        List<T> TracePath(Dictionary<T, T> traceDict, T i, List<T> returnList)
        {
            HashSet<T> visited = new HashSet<T>();

            while (traceDict.ContainsKey(i))
            {
                if (visited.Contains(i))
                    return returnList;

                visited.Add(i);

                returnList.Add(i);
                i = traceDict[i];
            }

            return returnList;
        }
    }

    public interface INode
    {
        public bool Validate(INode t);
        public List<INode> GetNeighbours();
        public float Heuristic();
        public bool CheckIfObstructed();
        string GetIdentifier();
    }

    public class Position
    {
        public int x, y;

        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Position operator +(Position a, Position b)
        {
            return new Position(a.x + b.x, a.y + b.y);
        }

        public override string? ToString()
        {
            return "[" + x + ", " + y + "]";
        }
    }

    public class PriorityQueue<T>
    {
        private List<(T item, float priority)> heap = new List<(T, float)>();

        public int Count => heap.Count;

        public void Enqueue(T item, float priority)
        {
            heap.Add((item, priority));
            int childIndex = heap.Count - 1;
            int parentIndex = (childIndex - 1) / 2;

            while (childIndex > 0 && heap[childIndex].priority < heap[parentIndex].priority)
            {
                (heap[parentIndex], heap[childIndex]) = (heap[childIndex], heap[parentIndex]);
                childIndex = parentIndex;
                parentIndex = (childIndex - 1) / 2;
            }
        }

        public T Dequeue()
        {
            if (heap.Count == 0) throw new InvalidOperationException("Priority queue is empty.");
            T result = heap[0].item;

            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);

            int parent = 0;
            while (true)
            {
                int left = 2 * parent + 1, right = 2 * parent + 2, smallest = parent;
                if (left < heap.Count && heap[left].priority < heap[smallest].priority) smallest = left;
                if (right < heap.Count && heap[right].priority < heap[smallest].priority) smallest = right;
                if (smallest == parent) break;

                (heap[parent], heap[smallest]) = (heap[smallest], heap[parent]);
                parent = smallest;
            }

            return result;
        }
    }
}
