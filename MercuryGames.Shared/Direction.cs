using System;
using System.Collections.Generic;
using System.Text;

namespace MercuryGames.Shared {
    public enum Direction : byte {
        Up,
        Down,
        Left,
        Right
    }

    public static class DirectionHelper {
        public static Direction? ToDirection(this GameKey key) {
            switch (key) {
                case GameKey.Up:
                    return Direction.Up;
                case GameKey.Down:
                    return Direction.Down;
                case GameKey.Left:
                    return Direction.Left;
                case GameKey.Right:
                    return Direction.Right;
                default:
                    return null;
                    //throw new ArgumentException("key is not a valid direction");
            }
        }

        public static bool IsOpposite(Direction a, Direction b) {
            return a == Direction.Up && b == Direction.Down
                   || a == Direction.Left && b == Direction.Right
                   || a == Direction.Down && b == Direction.Up
                   || a == Direction.Right && b == Direction.Left;
        }
    }
}
