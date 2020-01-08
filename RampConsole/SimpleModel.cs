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

        private readonly DoubleLayerTileMap<SimpleTile> map;

        public int Height { get; }
        public int Width { get; }

        public Point PlayerLocation { get; private set; }
        public HashSet<Point> FoodLocations { get; private set; } = new HashSet<Point>();

        public IReadOnlyTileMap<SimpleTile> Map => this.map;

        public int Score { get; set; }

        public SimpleModel(int w, int h) {
            this.Width = w;
            this.Height = h;
            this.map = new DoubleLayerTileMap<SimpleTile>(w, h);
            this.Reset();
        }

        private void GenerateRandomFoods(int count = 10) {
            this.FoodLocations.Clear();
            while (count-- > 0) {
                this.FoodLocations.Add(this.GetRandomPoint());
            }
        }

        private Point GetRandomPoint() {
            int x = rnd.Next(this.Width);
            int y = rnd.Next(this.Height);
            return new Point(x, y);
        }

        public IEnumerable<Point> Update(params GameKey[] input) {
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

        public IEnumerable<Point> ProcessNoInputFrame() {
            return Array.Empty<Point>();
        }

        public IEnumerable<Point> ProcessPlayerMove(Direction direction) {
            if (!this.fs.Tick()) {
                return Array.Empty<Point>();
            }

            var list = new List<Point>();
            this.map.Foreground[this.PlayerLocation] = null;
            list.Add(this.PlayerLocation);

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
                return Array.Empty<Point>();
            }

            this.ProcessFoodLogic();

            this.map.Foreground[this.PlayerLocation] = SimpleTile.Player;
            list.Add(this.PlayerLocation);
            
            return list;
        }

        public bool TestFood() => this.map.Foreground[this.PlayerLocation] == SimpleTile.Food;

        public void Reset() {
            this.lastDirection = Direction.Right;
            this.GenerateRandomFoods();
            this.PlayerLocation = Point.Zero;
            this.Score = 0;
            this.dead = false;

            this.map.Background.Fill(SimpleTile.Blank);

            foreach (var foodLocation in this.FoodLocations) {
                this.map.Foreground[foodLocation] = SimpleTile.Food;
            }
        }

        public bool IsWin() { return this.FoodLocations.Count == 0; }

        public bool IsDead() { return this.dead; }

        private void ProcessFoodLogic() {
            if (this.TestFood()) {
                this.map.Foreground[this.PlayerLocation] = SimpleTile.Blank;
                this.AddScore();
                this.FoodLocations.Remove(this.PlayerLocation);
            }
        }

        public void AddScore() {
            this.Score += 1;
        }
    }
}