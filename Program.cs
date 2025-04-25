using System;
using System.Diagnostics;
using System.Text.Json;
using System.Timers;

namespace ASTAR_SPEEDTEST
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Launching map editor...");

            Console.ForegroundColor = ConsoleColor.Blue;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new EditorForm());
        }
    }

    public class EditorForm : Form
    {
        const int GridSize = 60;
        const int CellSize = 20;
        bool[,] grid = new bool[GridSize, GridSize];

        private bool isDrawing = false;
        private bool drawMode = true;

        public Position start = new Position(0,0);
        public Position stop = new Position(58, 58);

        public bool shouldDraw = false;

        public EditorForm()
        {
            this.Text = "Map Editor";
            this.DoubleBuffered = true;
            this.ClientSize = new Size(GridSize * CellSize, GridSize * CellSize + 140);

            // Save button
            var saveBtn = new Button() { Text = "Save Map", Location = new Point(10, GridSize * CellSize + 5) };
            saveBtn.Click += SaveMap;
            Controls.Add(saveBtn);

            // Load button
            var loadBtn = new Button() { Text = "Load Map", Location = new Point(100, GridSize * CellSize + 5) };
            loadBtn.Click += LoadMap;
            Controls.Add(loadBtn);

            var runBtn = new Button() { Text = "Run A*", Location = new Point(190, GridSize * CellSize + 5) };
            runBtn.Click += RunAStar2;
            Controls.Add(runBtn);

            var drawToggle = new CheckBox() {Text = "Draw path", Location = new Point(280, GridSize * CellSize + 5) };

            drawToggle.CheckedChanged += (sender, e) =>
            {
                shouldDraw = drawToggle.Checked;
            };

            Controls.Add(drawToggle);

            // Events
            this.MouseDown += EditorForm_MouseDown;
            this.MouseMove += EditorForm_MouseMove;
            this.MouseUp += (s, e) => isDrawing = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            for (int y = 0; y < GridSize; y++)
            {
                for (int x = 0; x < GridSize; x++)
                {
                    Rectangle rect = new Rectangle(x * CellSize, y * CellSize, CellSize, CellSize);
                    g.FillRectangle(grid[y, x] ? Brushes.Black : Brushes.White, rect);
                    g.DrawRectangle(Pens.Gray, rect);
                }
            }
        }

        private void EditorForm_MouseDown(object? sender, MouseEventArgs e)
        {
            int x = e.X / CellSize;
            int y = e.Y / CellSize;

            if (x >= 0 && x < GridSize && y >= 0 && y < GridSize)
            {
                drawMode = !ModifierKeys.HasFlag(Keys.Shift);
                grid[y, x] = drawMode;
                isDrawing = true;
                Invalidate();
            }
        }

        private void EditorForm_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!isDrawing) return;

            int x = e.X / CellSize;
            int y = e.Y / CellSize;

            if (x >= 0 && x < GridSize && y >= 0 && y < GridSize)
            {
                grid[y, x] = drawMode;
                Invalidate();
            }
        }

        private void SaveMap(object? sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                FileName = "map.json"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                bool[][] arr = new bool[GridSize][];
                for (int y = 0; y < GridSize; y++)
                {
                    arr[y] = new bool[GridSize];
                    for (int x = 0; x < GridSize; x++)
                    {
                        arr[y][x] = grid[y, x];
                    }
                }

                string json = JsonSerializer.Serialize(arr);
                File.WriteAllText(dialog.FileName, json);
            }
        }

        private void LoadMap(object? sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string json = File.ReadAllText(dialog.FileName);
                bool[][]? arr = JsonSerializer.Deserialize<bool[][]>(json);

                if (arr != null)
                {
                    for (int y = 0; y < GridSize && y < arr.Length; y++)
                    {
                        for (int x = 0; x < GridSize && x < arr[y].Length; x++)
                        {
                            grid[y, x] = arr[y][x];
                        }
                    }
                    Invalidate();
                }
            }
        }


        private void DrawSquare(Graphics g, Position p, float size, Color color)
        {
            using (Brush brush = new SolidBrush(color))
            {
                int offset = (int) MathF.Abs(CellSize - (size * (float)CellSize)) / 2;

                Rectangle cellRect = new Rectangle(p.y * CellSize + offset, p.x * CellSize + offset, (int)(CellSize*size), (int)(CellSize*size));
                g.FillRectangle(brush, cellRect);
            }
        }



        /*private void RunAStar(object? sender, EventArgs e)
        {
            Console.WriteLine("basic a* - started");

            int length_sum = 0;

            int iterations = 20000;

            bool draw = shouldDraw;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < iterations; i++)
            {
                grid[start.x, stop.x] = !grid[start.x, stop.x]; //Toggle a tile in the top right corner, to prevent optimisation from the compiler
                var p = GetPath(start, stop);
                length_sum += p.Item1.Count;

                if(i == iterations - 1 && draw)
                {
                    DrawPath(p.Item1, p.Item2);
                }
            }

            stopwatch.Stop();
            var time_sum = stopwatch.ElapsedMilliseconds;

            Console.WriteLine("Iterations: " + iterations);
            Console.WriteLine("Sum: " + time_sum + " ms");
            Console.WriteLine("Path length: " + length_sum / iterations);
            Console.WriteLine("Individual time: " + (time_sum / (double)iterations).ToString("F2") + " ms");

            Console.WriteLine("Inspected " + GetPathNodeCount(start, stop) + " nodes.");
        }*/


        private void RunAStar2(object? sender, EventArgs e)
        {
            for (int _i = 0; _i < 6; _i++)
            {

            

            Console.WriteLine("\nModified A* - started\nMap: Two Paths\n");

            int length_sum = 0;
            int iterations = 5000;
            bool draw = shouldDraw;

            List<double> timings = new List<double>();
            double tickToMs = 1000.0 / Stopwatch.Frequency;

            Stopwatch overallStopwatch = new Stopwatch();
            overallStopwatch.Start();

            for (int i = 0; i < iterations; i++)
            {
                grid[start.x, stop.x] = !grid[start.x, stop.x]; // Toggle a tile in the top right corner

                Stopwatch sw = Stopwatch.StartNew();
                var p = GetPath(start, stop);
                sw.Stop();

                double elapsedMs = sw.ElapsedTicks * tickToMs;
                timings.Add(elapsedMs);

                length_sum += p.Item1.Count;

                if (i == iterations - 1 && draw)
                {
                    DrawPath(p.Item1, p.Item2);
                }
            }

            overallStopwatch.Stop();

            double totalTime = overallStopwatch.ElapsedMilliseconds;
            double averageTime = timings.Average();
            double stdDev = Math.Sqrt(timings.Sum(t => Math.Pow(t - averageTime, 2)) / timings.Count);

            Console.WriteLine("Iterations: " + iterations);
            Console.WriteLine("Total time: " + totalTime + " ms");
            Console.WriteLine("Average path length: " + (length_sum / iterations));
            Console.WriteLine("Average time: " + averageTime.ToString("F2") + " ms");
            Console.WriteLine("Standard deviation: " + stdDev.ToString("F2") + " ms");

            Console.WriteLine("Inspected " + GetPathNodeCount(start, stop) + " nodes.");
            }
        }


        private (List<Position>, bool) GetPath(Position start, Position stop)
        {
            bool[,] tempGrid = grid;

            Standard_ASTAR startNode = new Standard_ASTAR(start, start, stop, tempGrid);
            Standard_ASTAR stopNode = new Standard_ASTAR(stop, stop, stop, tempGrid);

            ASTAR_Generic<Standard_ASTAR> astarEngine = new ASTAR_Generic<Standard_ASTAR>();
            List<Standard_ASTAR> pathList = astarEngine.GetPath(startNode, stopNode);

            bool found = pathList.First().Validate(stopNode);

            if (found)
                return (pathList.Select(p => p.c).ToList(), true);
            else
                return (new List<Position>(), false);
        }


        private int GetPathNodeCount(Position start, Position stop)
        {
            bool[,] tempGrid = grid;

            Standard_ASTAR startNode = new Standard_ASTAR(start, start, stop, tempGrid);
            Standard_ASTAR stopNode = new Standard_ASTAR(stop, stop, stop, tempGrid);

            ASTAR_Generic<Standard_ASTAR> astarEngine = new ASTAR_Generic<Standard_ASTAR>();
            int c = astarEngine.GetPath_NodeCount(startNode, stopNode);

            return c;
        }


        private void DrawPath(List<Position> list, bool found)
        {
            Graphics g = this.CreateGraphics();

            for (int i = 0; i < list.Count - 1; i++)
            {
                if(found)
                    DrawSquare(g, list[i], 0.70f, Color.LimeGreen);
                else
                    DrawSquare(g, list[i], 0.30f, Color.Coral);
            }

            Console.WriteLine(list.Count + ": " + found);
            g.Dispose();
        }


    }
}
