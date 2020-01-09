using System;
using System.Collections.Generic;
using MercuryGames.Shared;
using System.Linq;

namespace MercuryGames.Snake {
    public class SnakeModel : IModel<SnakeTile> {
        private Random rnd = new Random();

        private bool dead = false;

        private Queue<Point> snakeBodyLocations = new Queue<Point>();

        private Direction lastDirection = Direction.Right;

        private FrameSkipper frameSkipper = new FrameSkipper(5);

        public int Height { get; }
        public int Width { get; }

        private Point headLocation;
        public HashSet<Point> FoodLocations { get; private set; } = new HashSet<Point>();

        // TODO: 改成双层地图，一层放地形，一层放player和食物

        IReadOnlyTileMap<SnakeTile> IModel<SnakeTile>.Map => this.Map;

        public DoubleLayerTileMap<SnakeTile> Map { get; }

        public int Score { get; set; }

        public SnakeModel(int w, int h) {
            this.Width = w;
            this.Height = h;
            this.Map = new DoubleLayerTileMap<SnakeTile>(w, h);
            this.Reset();
        }

        public IEnumerable<Point> GetFullPlayerInfo() {
            var list = new List<Point>();
            foreach (Point body in this.snakeBodyLocations) {
                list.Add(body);
            }

            return list;
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
                this.frameSkipper.FramePerUpdate = this.frameSkipper.FramePerUpdate == 5 ? 3 : 5;
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

        private IEnumerable<Point> ProcessPlayerMove(Direction direction) {
            if (!this.frameSkipper.Tick()) {
                return Array.Empty<Point>();
            }

            var list = new List<Point>();
            Point previousTailLocation = this.snakeBodyLocations.Peek();

            switch (this.lastDirection) {
            case Direction.Right:
                if (this.headLocation.X < this.Width - 1) this.headLocation = this.headLocation.RightPosition();
                else this.dead = true;
                break;
            case Direction.Left:
                if (this.headLocation.X > 0) this.headLocation = this.headLocation.LeftPosition();
                else this.dead = true;
                break;
            case Direction.Up:
                if (this.headLocation.Y < this.Height - 1) this.headLocation = this.headLocation.UpPosition();
                else this.dead = true;
                break;
            case Direction.Down:
                if (this.headLocation.Y > 0) this.headLocation = this.headLocation.DownPosition();
                else this.dead = true;
                break;
            default:
                return Array.Empty<Point>();
            }

            bool eat = this.ProcessFoodLogic();
            
            if (this.IsEatingSelf(this.headLocation)) {
                this.dead = true;
            }

            list.Add(this.headLocation);
            this.Map.Foreground[this.headLocation] = SnakeTile.Snake;

            if (!eat) {
                this.snakeBodyLocations.Dequeue(); // 去掉尾巴
                list.Add(previousTailLocation);
                this.Map.Foreground[previousTailLocation] = null;
            }

            this.snakeBodyLocations.Enqueue(this.headLocation); // 新的头

            return list;
        }

        public void Reset() {
            this.lastDirection = Direction.Right;

            this.GenerateRandomMines();
            this.snakeBodyLocations.Clear();
            this.headLocation = new Point(this.Width / 2, this.Height / 2); //头生成在屏幕最中间

            this.snakeBodyLocations.Enqueue(this.headLocation.LeftPosition().LeftPosition());
            this.snakeBodyLocations.Enqueue(this.headLocation.LeftPosition());
            this.snakeBodyLocations.Enqueue(this.headLocation);

            foreach (Point bodyLocation in this.snakeBodyLocations) {
                this.Map.Foreground[bodyLocation] = SnakeTile.Snake;
            }

            this.Score = 0;
            this.dead = false;

            this.Map.Background.Fill(SnakeTile.Grass);

            foreach (Point foodLocation in this.FoodLocations) {
                this.Map.Foreground[foodLocation] = SnakeTile.Food;
            }
        }

        public bool IsWin() {
            return this.FoodLocations.Count == 0;
        }

        public bool IsDead() {
            return this.dead;
        }

        private void GenerateRandomMines(int count = 10) {
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

        private bool TestFood() => this.Map.Foreground[this.headLocation] == SnakeTile.Food;

        private bool ProcessFoodLogic() {
            if (this.TestFood()) {
                this.Map.Foreground[this.headLocation] = null;
                this.AddScore();
                this.FoodLocations.Remove(this.headLocation);

                return true;
            }

            return false;
        }

        private bool IsEatingSelf(Point newHeadLocation) {
            // 如果蛇头的位置重合了
            return this.snakeBodyLocations.Contains(newHeadLocation);
        }

        public void AddScore() {
            this.Score += 1;
        }
    }
}