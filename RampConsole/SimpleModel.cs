using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using MercuryGames.Shared;

namespace RampConsole {
    public enum SimpleTile {
        Blank,
        Player,
        Food
    }

    public class SimpleModel : IModel<SimpleTile> {
        private Random rnd = new Random();

        private bool dead = false;
        private Direction lastDirection = Direction.Right;

        private FrameSkipper fs = new FrameSkipper(5);

        private readonly TileMap<SimpleTile> map;

        public int Height { get; }
        public int Width { get; }

        public Point PlayerLocation { get; private set; }
        public HashSet<Point> Foods { get; private set; } = new HashSet<Point>();

        public IReadOnlyTileMap<SimpleTile> Map => this.map;

        public int Score { get; set; }

        public SimpleModel(int w, int h) {
            this.Width = w;
            this.Height = h;
            this.map = new TileMap<SimpleTile>(w, h);
            this.Reset();
        }

        public SimpleTile GetTile(Point p) { return this.map[p.X, p.Y]; }

        public void SetTile(Point p, SimpleTile value) { this.map[p.X, p.Y] = value; }

        private void GenerateRandomMines(int count = 10) {
            this.Foods.Clear();
            while (count-- > 0) {
                this.Foods.Add(this.GetRandomPoint());
            }
        }

        private Point GetRandomPoint() {
            int x = rnd.Next(this.Width);
            int y = rnd.Next(this.Height);
            return new Point(x, y);
        }

        public IEnumerable<TileUpdateInfo<SimpleTile>> GetFullPlayerInfo() {
            var list = new List<TileUpdateInfo<SimpleTile>>{new TileUpdateInfo<SimpleTile> {P = this.PlayerLocation, Type = SimpleTile.Player}};
            return list;
        }

        public IEnumerable<TileUpdateInfo<SimpleTile>> Update(params GameKey[] input) {
            if (input.Length == 0) {
                return this.ProcessPlayerMove(this.lastDirection);
            }

            var key = input[0];

            switch (key) {
            case GameKey.A:
                break;
            case GameKey.B:
                this.fs.FramePerUpdate = this.fs.FramePerUpdate == 5 ? 3 : 5;
                break;

            case GameKey.Up:
            case GameKey.Down:
            case GameKey.Left:
            case GameKey.Right:
                var direction = (Direction) key.ToDirection();
                if (!DirectionHelper.IsOpposite(this.lastDirection, direction)) {
                    this.lastDirection = direction;
                }

                return ProcessPlayerMove(direction);
            default:
                break;
            }

            return ProcessPlayerMove(this.lastDirection);
        }

        public IEnumerable<TileUpdateInfo<SimpleTile>> ProcessNoInputFrame() {
            return Array.Empty<TileUpdateInfo<SimpleTile>>();
        }

        public IEnumerable<TileUpdateInfo<SimpleTile>> ProcessPlayerMove(Direction direction) {
            if (!this.fs.Tick()) {
                return Array.Empty<TileUpdateInfo<SimpleTile>>();
            }

            var list = new List<TileUpdateInfo<SimpleTile>>();
            list.Add(new TileUpdateInfo<SimpleTile> {P = this.PlayerLocation, Type = this.GetTile(this.PlayerLocation)});

            switch (direction) {
            case Direction.Right:
                if (this.PlayerLocation.X < this.Width - 1) this.PlayerLocation = this.PlayerLocation.RightPosition();
                else this.dead = true;
                break;
            case Direction.Left:
                if (this.PlayerLocation.X > 0) this.PlayerLocation = this.PlayerLocation.LeftPosition();
                else this.dead = true;
                break;
            case Direction.Up:
                if (this.PlayerLocation.Y < this.Height - 1) this.PlayerLocation = this.PlayerLocation.UpPosition();
                else this.dead = true;
                break;
            case Direction.Down:
                if (this.PlayerLocation.Y > 0) this.PlayerLocation = this.PlayerLocation.DownPosition();
                else this.dead = true;
                break;
            default:
                return Array.Empty<TileUpdateInfo<SimpleTile>>();
            }

            list.Add(new TileUpdateInfo<SimpleTile> {P = this.PlayerLocation, Type = SimpleTile.Player});

            this.ProcessFoodLogic();

            return list;
        }

        public bool TestFood() => this.GetTile(this.PlayerLocation) == SimpleTile.Food;

        public void Reset() {
            this.lastDirection = Direction.Right;
            this.GenerateRandomMines();
            this.PlayerLocation = Point.Zero;
            this.Score = 0;
            this.dead = false;

            this.map.Fill(SimpleTile.Blank);

            foreach (var food in this.Foods) {
                this.SetTile(food, SimpleTile.Food);
            }
        }

        public bool IsWin() { return this.Foods.Count == 0; }

        public bool IsDead() { return this.dead; }

        private void ProcessFoodLogic() {
            if (this.TestFood()) {
                this.SetTile(this.PlayerLocation, SimpleTile.Blank);
                this.AddScore();
                this.Foods.Remove(this.PlayerLocation);
            }
        }

        public void AddScore() {
            this.Score += 1;
        }
    }
}