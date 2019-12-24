using System;
using System.Linq;

namespace MercuryGames.Shared
{
    public struct Point: IEquatable<Point>
    {
        public bool Equals(Point p)
        {
            return this == p;
        }
        public override string ToString()
        {
            return string.Format("({0},{1})", this.X, this.Y);
            // return $"({this.X},{this.Y})";
        }
        public int X {
            get;
        }
        public int Y {
            get;
        }
        public Point(int a, int b)
        {
            this.X = a;
            this.Y = b;
        }

        public static Point operator +(Point a, Point b)
        {
            int x = a.X + b.X;
            int y = a.Y + b.Y;
            return new Point(x, y);
        }
        public static Point operator -(Point a, Point b)
        {
            int x = a.X - b.X;
            int y = a.Y - b.Y;
            return new Point(x, y);

        }

        public static bool operator ==(Point a, Point b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Point a, Point b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public static Point Zero {
            get;
        } = new Point(0, 0);//调用了Point构造方法，传了参数0,0

        public static Point PositiveX { get; } = new Point(1, 0);
        public static Point NegativeX {get;} = new Point(-1, 0);
        public static Point PositiveY {get;} = new Point(0, 1);
        public static Point NegativeY {get;} = new Point(0, -1);

        public Point UpPosition() => this + PositiveY;
        public Point DownPosition() => this + NegativeY;
        public Point RightPosition() => this + PositiveX;
        public Point LeftPosition() => this + NegativeX;

        public static Point Parse(string s)
        {
            int a, b;
            if (s[0] == '('&&s.Last()==')') {
                s = s.Substring(1, s.Length - 2);
                var array = s.Split(',');
                if(array.Length==2)
                {
                    a = int.Parse(array[0]);
                    b = int.Parse(array[1]);
                    return new Point(a, b);
                }
            }

            throw new FormatException();
        }
    }
}
