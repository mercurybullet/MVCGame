using System;
using System.Collections.Generic;
using MercuryGames.Shared;
using System.Linq;

namespace MercuryGames.Snake {
    public class SnakeModel : IModel<SnakeTile> {
        private Random rnd = new Random();

        private bool dead = false;

        private Queue<Point> snakeBodies = new Queue<Point>();

        private Direction lastDirection = Direction.Right;

        private FrameSkipper fs = new FrameSkipper(5);

        public int Height { get; }
        public int Width { get; }

        private Point PlayerLocation { get; set; }
        public HashSet<Point> Foods { get; private set; } = new HashSet<Point>();

        // TODO: 改成双层地图，一层放地形，一层放player和食物
        public SnakeTile[,] Map { get; }

        public int Score { get; set; }

        public SnakeModel(int w, int h) {
            this.Width = w;
            this.Height = h;
            this.Map = new SnakeTile[w, h];
            this.Reset();
        }

        public SnakeTile GetTile(Point p) { return this.Map[p.X, p.Y]; }

        public void SetTile(Point p, SnakeTile value) { this.Map[p.X, p.Y] = value; }


        public IEnumerable<TileUpdateInfo<SnakeTile>> GetFullPlayerInfo() {
            var list = new List<TileUpdateInfo<SnakeTile>>();
            foreach (Point body in this.snakeBodies) {
                list.Add(new TileUpdateInfo<SnakeTile> {P = body, Type = SnakeTile.Player});
            }

            return list;
        }

        public IEnumerable<TileUpdateInfo<SnakeTile>> Update(params GameKey[] input) {
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

        private IEnumerable<TileUpdateInfo<SnakeTile>> ProcessPlayerMove(Direction direction) {
            if (!this.fs.Tick()) {
                return Array.Empty<TileUpdateInfo<SnakeTile>>();
            }

            var list = new List<TileUpdateInfo<SnakeTile>>();
            Point tail = this.snakeBodies.Peek();


            switch (this.lastDirection) {
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
                return Array.Empty<TileUpdateInfo<SnakeTile>>();
            }

            bool eat = this.ProcessFoodLogic();

            if (eat) {
                list.Add(new TileUpdateInfo<SnakeTile> {P = this.PlayerLocation, Type = SnakeTile.Player});
            }
            else {
                this.snakeBodies.Dequeue(); // 去掉尾巴
                list.Add(new TileUpdateInfo<SnakeTile> {P = tail, Type = this.GetTile(tail)});
                list.Add(new TileUpdateInfo<SnakeTile> {P = this.PlayerLocation, Type = SnakeTile.Player});
            }

            this.snakeBodies.Enqueue(this.PlayerLocation); // 新的头

            if (this.IsEatingSelf()) {
                this.dead = true;
            }

            return list;
        }

        public void Reset() {
            this.lastDirection = Direction.Right;

            this.GenerateRandomMines();
            this.snakeBodies.Clear();
            this.PlayerLocation = new Point(this.Width / 2, this.Height / 2); //头生成在屏幕最中间

            this.snakeBodies.Enqueue(this.PlayerLocation.LeftPosition().LeftPosition());
            this.snakeBodies.Enqueue(this.PlayerLocation.LeftPosition());
            this.snakeBodies.Enqueue(this.PlayerLocation);

            this.Score = 0;
            this.dead = false;

            for (int i = 0; i < this.Width; i++) {
                for (int j = 0; j < this.Height; j++) {
                    this.Map[i, j] = SnakeTile.Grass;
                }
            }

            foreach (var food in this.Foods) {
                this.SetTile(food, SnakeTile.Food);
            }
        }

        public bool IsWin() { return this.Foods.Count == 0; }

        public bool IsDead() { return this.dead; }

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

        private bool TestFood() => this.GetTile(this.PlayerLocation) == SnakeTile.Food;

        private bool ProcessFoodLogic() {
            if (this.TestFood()) {
                this.SetTile(this.PlayerLocation, SnakeTile.Blank);
                this.AddScore();
                this.Foods.Remove(this.PlayerLocation);

                return true;
            }

            return false;
        }

        private bool IsEatingSelf() { return this.snakeBodies.Count(p => p == this.PlayerLocation) > 1; }

        public void AddScore() { this.Score += 1; }
    }
}