using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MercuryGames.Shared;

namespace RampConsole {
    class View {
        private static Dictionary<TileType, string> tileMap = new Dictionary<TileType, string>
        {
            {TileType.Grass, "w"},
            {TileType.Player, "@"},
            {TileType.food, "*"},
            {TileType.Blank, " "}
        };

        public void Clean() {
            Console.Clear();
        }

        public void UpdateTile(Point p, TileType type) {
            int h = Console.WindowHeight;

            //Console.WriteLine("您的当前位置是{0}", p);
            Console.SetCursorPosition(p.X, h - p.Y - 1);
            Console.Write(tileMap[type]);
        }

        public void WriteAt(Point p, string s) {
            int h = Console.WindowHeight;

            //Console.WriteLine("您的当前位置是{0}", p);
            Console.SetCursorPosition(p.X, h - p.Y - 1);
            Console.Write(s);
        }

        public void PrintDieMessage() {
            Console.WriteLine("YOU DIED");
        }

        public void PrintScore(int score) {

            Console.Title=$"当前分数：{score}";
        }

    }
}
