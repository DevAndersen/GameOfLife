using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeLib
{
    public class LifeGrid
    {
        public delegate void UpdateArg(LifeGrid grid);
        public event UpdateArg OnUpdate;

        public int Width { get; set; }
        public int Height { get; set; }

        private bool[,] cells;
        private Random rand;

        public LifeGrid(int width, int height, int? seed = null)
        {
            Width = width;
            Height = height;
            cells = new bool[width, height];
            if (seed == null)
                rand = new Random();
            else
                rand = new Random(seed.Value);
        }

        public void Update()
        {
            NextTick();
            OnUpdate?.Invoke(this);
        }

        private void NextTick()
        {
            bool[,] newCells = new bool[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    bool alive = cells[x, y];
                    int aliveAround = 0;

                    if (x < Width - 1 && cells[x + 1, y]) aliveAround++;
                    if (x > 0 && cells[x - 1, y]) aliveAround++;
                    if (y < Height - 1 && cells[x, y + 1]) aliveAround++;
                    if (y > 0 && cells[x, y - 1]) aliveAround++;

                    if (x < Width - 1 && y < Height - 1 && cells[x + 1, y + 1]) aliveAround++;
                    if (x < Width - 1 && y > 0 && cells[x + 1, y - 1]) aliveAround++;
                    if (x > 0 && y < Height - 1 && cells[x - 1, y + 1]) aliveAround++;
                    if (x > 0 && y > 0 && cells[x - 1, y - 1]) aliveAround++;

                    if (alive)
                    {
                        if (aliveAround < 2)
                            newCells[x, y] = false;
                        else if (aliveAround > 3)
                            newCells[x, y] = false;
                        else
                            newCells[x, y] = true;
                    }
                    else
                    {
                        if (aliveAround == 3)
                            newCells[x, y] = true;
                    }
                }
            }
            cells = newCells;
        }

        public bool GetCell(int x, int y)
        {
            return cells[x, y];
        }

        public void SetCell(int x, int y, bool isAlive)
        {
            cells[x, y] = isAlive;
        }
    }
}