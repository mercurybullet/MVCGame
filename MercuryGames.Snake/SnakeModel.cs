using System;
using System.Collections.Generic;
using MercuryGames.Shared;
using System.Linq;

namespace MercuryGames.Snake {
    public class SnakeModel : IModel<SnakeTile> {
        /// <summary>
        /// 随机数发生器
        /// </summary>
        private Random rnd = new Random();

        /// <summary>
        /// 我现在死了吗
        /// </summary>
        private bool dead = false;

        /// <summary>
        /// 蛇在上一个tick中的移动方向
        /// </summary>
        private Direction currentDirection = Direction.Right;

        /// <summary>
        /// 用来跳帧的
        /// </summary>
        private FrameSkipper frameSkipper = new FrameSkipper(5);

        public int Height { get; }
        public int Width { get; }

        private Point headLocation;

        /// <summary>
        /// 蛇身所在的每一个图块的坐标，反向存放（即蛇尾在队列头，蛇头在队列尾）
        /// </summary>
        private Queue<Point> snakeBodyLocations = new Queue<Point>();
        public HashSet<Point> FoodLocations { get; private set; } = new HashSet<Point>();

        /// <summary>
        /// 这个写法叫做【显式接口实现】
        /// 因为下面一行的那个Map是DoubleLayerTileMap类型，而接口IModel中要求Map的类型是IReadOnlyTileMap，所以不得不用显式接口实现
        /// </summary>
        IReadOnlyTileMap<SnakeTile> IModel<SnakeTile>.Map => this.Map;

        /// <summary>
        /// 地图
        /// </summary>
        public DoubleLayerTileMap<SnakeTile> Map { get; }

        public int Score { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public SnakeModel(int w, int h) {
            this.Width = w;
            this.Height = h;
            this.Map = new DoubleLayerTileMap<SnakeTile>(w, h);
            this.Reset();
        }

        /// <summary>
        /// 这个方法已经没用了，可以删掉
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Point> GetFullPlayerInfo() {
            var list = new List<Point>();
            foreach (Point body in this.snakeBodyLocations) {
                list.Add(body);
            }

            return list;
        }

        /// <summary>
        /// 更新一帧
        /// </summary>
        /// <param name="input">用户在这一帧内的输入。当前来说，如果玩家在一帧内按下了多个按钮，我们只处理第一个</param>
        /// <returns>地图上被改变了的点的坐标集合</returns>
        public IEnumerable<Point> Update(params GameKey[] input) {
            // 如果没有输入的话，那么蛇继续沿着之前的方向移动
            if (input.Length == 0) {
                return this.ProcessPlayerMove();
            }

            var key = input[0];

            switch (key) {
            case GameKey.A:
                break;
            case GameKey.B:
                // 更新间隔在5帧和3帧之间切换
                this.frameSkipper.FramePerUpdate = this.frameSkipper.FramePerUpdate == 5 ? 3 : 5;
                break;

            // 如果输入了方向键的话
            case GameKey.Up:
            case GameKey.Down:
            case GameKey.Left:
            case GameKey.Right:
                // ToDirection是扩展方法，返回的是带问号（Nullable）的Direction，也就是【Direction?】。需要用强制类型转换转成Direction
                var newDirection = (Direction) key.ToDirection();
                // 蛇只能转向，不能掉头
                if (!DirectionHelper.IsOpposite(this.currentDirection, newDirection)) {
                    this.currentDirection = newDirection;
                }

                return ProcessPlayerMove();
            default:
                break;
            }

            // 什么情况下会执行到这行呢？
            // 答案：当用户有输入，但是输入的不是方向键的情况下
            return ProcessPlayerMove();
        }

        /// <summary>
        /// 处理蛇移动的逻辑
        /// </summary>
        /// <returns>地图上被改变了的点的坐标集合</returns>
        private IEnumerable<Point> ProcessPlayerMove() {
            // 贪吃蛇移动的逻辑（.代表空地，@代表蛇，*代表食物，蛇在向右移动）：

            // 只有在没吃到东西的时候，蛇尾才会向前移动。
            // 移动前：...@@@@....
            // 移动后：....@@@@...

            // 如果吃到了东西，蛇尾不动，蛇头长出一格。
            // 移动前：...@@@@*...
            // 移动后：...@@@@@...

            // 如果frameSkipper说这一帧休眠的话，就返回空数组（长度为0）
            if (!this.frameSkipper.Tick()) {
                // return new Point[0];
                // 这两种写法在功能上是一样的，那么为什么要写成下面的写法呢？
                // 答案是new Point[0]会在内存中创建对象，也就是每一帧都会新创建一个长度为0的数组
                // 而Array.Empty<Point>()只会在内存中创建一次，之后就一直用那个长度为0的数组
                return Array.Empty<Point>();
            }

            // list里存储的是被改变的图块的坐标
            var list = new List<Point>();
            // snakeBodyLocations是一个Queue。Queue的Peek方法返回队首的元素。
            Point previousTailLocation = this.snakeBodyLocations.Peek();

            switch (this.currentDirection) {
            case Direction.Right:
                if (this.headLocation.X < this.Width - 1) this.headLocation = this.headLocation.RightPosition();
                else this.dead = true; // 撞墙死
                break;
            case Direction.Left:
                if (this.headLocation.X > 0) this.headLocation = this.headLocation.LeftPosition();
                else this.dead = true; // 撞墙死
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

            // 此时headLocation已经是蛇头的【新】位置了
            // ProcessFoodLogic的返回值表示蛇头有没有吃到东西
            bool eat = this.ProcessFoodLogic();
            
            if (this.IsEatingSelf(this.headLocation)) {
                this.dead = true;
            }

            // 新位置变成了蛇头
            list.Add(this.headLocation);
            this.Map.Foreground[this.headLocation] = SnakeTile.Snake;

            if (!eat) {
                // 只有在没吃到东西的情况下才要拿走原本的蛇尾。原因看这个函数开头的注释。
                // 去掉尾巴。Dequeue和Peek相似，也会返回队首的元素，但是Dequeue还会把队首的元素踢出去。在这里我们用不到它的返回值。
                this.snakeBodyLocations.Dequeue();
                // 之前的蛇尾的位置变成了空
                list.Add(previousTailLocation);
                this.Map.Foreground[previousTailLocation] = null;
            }

            this.snakeBodyLocations.Enqueue(this.headLocation); // 新的头加入队尾

            return list;
        }

        /// <summary>
        /// 重置游戏
        /// </summary>
        public void Reset() {
            this.currentDirection = Direction.Right; // 默认方向朝右

            this.GenerateRandomFoods(); 
            this.headLocation = new Point(this.Width / 2, this.Height / 2); //头生成在屏幕最中间

            // 重新生成蛇身，要注意队列是反着存的
            this.snakeBodyLocations.Clear();
            this.snakeBodyLocations.Enqueue(this.headLocation.LeftPosition().LeftPosition()); 
            this.snakeBodyLocations.Enqueue(this.headLocation.LeftPosition());
            this.snakeBodyLocations.Enqueue(this.headLocation);

            this.Map.Foreground.Fill(null); // 这行代码修复了【按esc重置游戏后，之前的蛇身还显示在屏幕上】的bug
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
            // 如果蛇头的位置和任意一节蛇身的位置重合了
            return this.snakeBodyLocations.Contains(newHeadLocation);
        }

        public void AddScore() {
            this.Score += 1;
        }
    }
}