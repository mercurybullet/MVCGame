using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MercuryGames.Shared;
using MercuryGames.Snake;

namespace RampConsole {
    interface IConnection {
        void Open();
    }

    interface IWindow {
        void Open();
    }

    class Something : IConnection, IWindow {
        public void Open() {
            Console.WriteLine("I'm opening");
        }

        void IWindow.Open() {
            Console.WriteLine("I'm opening a window");
        }
    }

    class Program {
        public static void WriteAt(Point p, string s) {
            int h = Console.WindowHeight;

            //Console.WriteLine("您的当前位置是{0}", p);
            Console.SetCursorPosition(p.X, h - p.Y - 1);
            Console.Write(s);
        }

        static void Main() {
            IModel<SnakeTile> model = new SnakeModel(20, 15);
            SimpleConsoleView<SnakeTile> simpleConsoleView = new SimpleConsoleView<SnakeTile>();
            ColorfulConsoleView<SnakeTile> colorful = new ColorfulConsoleView<SnakeTile>();

            simpleConsoleView.SetTextureMap(new Dictionary<SnakeTile, string>
            {
                {SnakeTile.Grass, "w"},
                {SnakeTile.Blank, " "},
                {SnakeTile.Snake, "@"},
                {SnakeTile.Food, "*"}
            });

            colorful.SetTextureMap(new Dictionary<SnakeTile, string>
            {
                {SnakeTile.Grass, "w"},
                {SnakeTile.Blank, " "},
                {SnakeTile.Snake, "@"},
                {SnakeTile.Food, "*"}
            });

            colorful.SetTextureMap(new Dictionary<SnakeTile, ConsoleColor>
            {
                {SnakeTile.Grass, ConsoleColor.Green},
                {SnakeTile.Snake, ConsoleColor.White},
                {SnakeTile.Food, ConsoleColor.Red},
                {SnakeTile.Blank, ConsoleColor.Black}
            });

            IView<SnakeTile> view;

            Console.WriteLine("你想要：A.黑白电视 B.彩色电视：");
            string input = Console.ReadLine();
            if (input.StartsWith("B")) {
                view = colorful;
            }
            else {
                view = simpleConsoleView;
            }

            Controller<SnakeTile> controller = new Controller<SnakeTile>(model, view);
            controller.GameMain();
        }


        static void Main1111(string[] args) {
            start:
            Point p = new Point(0, 0);
            var mines = new HashSet<Point>() {new Point(3, 3), new Point(4, 5), new Point(10, 6)};

            foreach (var mine in mines) {
                Console.SetCursorPosition(mine.X, mine.Y);
                WriteAt(mine, "*");
            }

            while (true) {
                WriteAt(p, "@");

                ConsoleKeyInfo input = Console.ReadKey(true); //当传入的参数为true时，不会在控制台回显
                switch (input.Key) {
                case ConsoleKey.RightArrow:
                    if (p.X < Console.WindowWidth - 1) p = p.RightPosition();
                    break;
                case ConsoleKey.LeftArrow:
                    if (p.X > 0) p = p.LeftPosition();
                    break;
                case ConsoleKey.UpArrow:
                    if (p.Y < Console.WindowHeight - 1) p = p.UpPosition();
                    break;
                case ConsoleKey.DownArrow:
                    if (p.Y > 0) p = p.DownPosition();
                    break;
                case ConsoleKey.A:
                    Console.WriteLine("您按下了A");
                    break;
                case ConsoleKey.B:
                    Console.WriteLine("您按下了B");
                    break;
                default:
                    continue;
                }

                if (mines.Contains(p)) {
                    Console.WriteLine("你踩到了地雷！按esc退出");
                    if (Console.ReadKey().Key == ConsoleKey.Escape) {
                        Console.WriteLine("游戏结束");
                        return;
                    }
                    else {
                        Console.Clear();
                        goto start;
                    }
                }
            }
        }
    }
}