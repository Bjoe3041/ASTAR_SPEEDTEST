using System;
using System.Text.Json;

namespace ASTAR_SPEEDTEST
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Launching map editor...");

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
            runBtn.Click += RunAStar;
            Controls.Add(runBtn);

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
                Rectangle cellRect = new Rectangle(p.y * CellSize + (int)(size/2), p.x * CellSize + (int)(size/2), (int)(CellSize*size), (int)(CellSize*size));
                g.FillRectangle(brush, cellRect);
            }
        }



        private void RunAStar(object? sender, EventArgs e)
        {
            DrawPath(new Position(0, 0), new Position(58, 58));

            Console.WriteLine("basic a* - started");
        }

        private void DrawPath(Position start, Position stop)
        {
            bool[,] tempGrid = grid;


            Standard_ASTAR startNode = new Standard_ASTAR(start, start, stop, tempGrid);
            Standard_ASTAR stopNode = new Standard_ASTAR(stop, stop, stop, tempGrid);

            ASTAR_Generic<Standard_ASTAR> astarEngine = new ASTAR_Generic<Standard_ASTAR>();
            List<Standard_ASTAR> pathList = astarEngine.GetPath(startNode, stopNode);

            Graphics g = this.CreateGraphics();


            for (int i = 0; i < pathList.Count - 1; i++)
            {
                Standard_ASTAR node = pathList[i];
                Standard_ASTAR node2 = pathList[i + 1];

                DrawSquare(g, node.c, 0.5f, Color.Magenta);
            }

            Console.WriteLine(pathList.Count);
            g.Dispose();
        }


    }
}
