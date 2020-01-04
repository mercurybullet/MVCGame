using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MercuryGames.Shared;

namespace RampConsole {
    class ColorfulConsoleView<T> : SimpleConsoleView<T>, IView<T,ConsoleColor> {
        private IDictionary<T, ConsoleColor> tileColorMap;

        //    new Dictionary<TileType, ConsoleColor>
        //{
        //    {TileType.Grass, ConsoleColor.Green},
        //    {TileType.Player, ConsoleColor.White},
        //    {TileType.Food, ConsoleColor.Red},
        //    {TileType.Blank, ConsoleColor.Black}
        //};

        public override void UpdateTile(Point p, T type) {
            int h = Console.WindowHeight;

            //Console.WriteLine("您的当前位置是{0}", p);
            Console.SetCursorPosition(p.X, h - p.Y - 1);
            Console.BackgroundColor = tileColorMap[type];
            Console.Write(this.textureMap[type]);
            Console.ResetColor();
        }

        public void SetTextureMap(IDictionary<T, ConsoleColor> map) { this.tileColorMap = map; }
    }
}
