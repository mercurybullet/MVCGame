using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MercuryGames.Shared;

namespace RampConsole {
    public class Model {
        public struct TileUpdateInfo {
            public Point P;
            public TileType Type;
        }

        private Random rnd = new Random();

        public int Height { get; }
        public int Width { get; }

        public Point PlayerLocation { get; private set; }
        public HashSet<Point> Mines { get; private set; } = new HashSet<Point>();

        public TileType[,] Map { get; }

        public Model(int w, int h) {
            this.Width = w;
            this.Height = h;
            this.Map = new TileType[w, h];
            this.Reset();
        }

        public TileType GetTile(Point p) { return this.Map[p.X, p.Y]; }

        public void SetTile(Point p, TileType value) { this.Map[p.X, p.Y] = value; }

        private void GenerateRandomMines(int count = 10) {
            this.Mines.Clear();
            while (count-- > 0) {
                this.Mines.Add(this.GetRandomPoint());
            }
        }

        private Point GetRandomPoint() {
            int x = rnd.Next(this.Width);
            int y = rnd.Next(this.Height);
            return new Point(x, y);
        }

        public IEnumerable<TileUpdateInfo> ProcessUserInput(GameKey key) {
            var list = new List<TileUpdateInfo>();
            list.Add(new TileUpdateInfo {P = this.PlayerLocation, Type = this.GetTile(this.PlayerLocation)});

            switch (key) {
            case GameKey.Right:
                if (this.PlayerLocation.X < this.Width - 1) this.PlayerLocation = this.PlayerLocation.RightPosition();
                break;
            case GameKey.Left:
                if (this.PlayerLocation.X > 0) this.PlayerLocation = this.PlayerLocation.LeftPosition();
                break;
            case GameKey.Up:
                if (this.PlayerLocation.Y < this.Height - 1) this.PlayerLocation = this.PlayerLocation.UpPosition();
                break;
            case GameKey.Down:
                if (this.PlayerLocation.Y > 0) this.PlayerLocation = this.PlayerLocation.DownPosition();
                break;
            default:
                return Array.Empty<TileUpdateInfo>();
            }

            list.Add(new TileUpdateInfo {P = this.PlayerLocation, Type = TileType.Player});

            return list;
        }

        public bool TestBomb() => this.GetTile(this.PlayerLocation) == TileType.Mine;

        public void Reset() {
            this.GenerateRandomMines();
            this.PlayerLocation = Point.Zero;
            for (int i = 0; i < this.Width; i++) {
                for (int j = 0; j < this.Height; j++) {
                    this.Map[i, j] = TileType.Grass;
                }
            }

            foreach (var mine in this.Mines) {
                this.SetTile(mine, TileType.Mine);
            }
        }
    }
}