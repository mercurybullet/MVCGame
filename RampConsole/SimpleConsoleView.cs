using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MercuryGames.Shared;

namespace RampConsole {
    /// <summary>
    /// 简单的控制台View
    /// </summary>
    /// <typeparam name="T">逻辑层面上图块的类型</typeparam>
    /// <typeparam name="U">显示层面上图块的类型</typeparam>
    class SimpleConsoleView <T> : IView<T, string> {
        protected IDictionary<T, string> textureMap;

        public void SetTextureMap(IDictionary<T, string> textureMap) { this.textureMap = textureMap; }

        public void Clean() { Console.Clear(); }

        public virtual void UpdateTile(Point p, T type) {
            int h = Console.WindowHeight;

            //Console.WriteLine("您的当前位置是{0}", p);
            Console.SetCursorPosition(p.X, h - p.Y - 1);
            Console.Write(this.textureMap[type]);
        }

        public void WriteAt(Point p, string s) {
            int h = Console.WindowHeight;

            //Console.WriteLine("您的当前位置是{0}", p);
            Console.SetCursorPosition(p.X, h - p.Y - 1);
            Console.Write(s);
        }

        public void PrintDieMessage() { Console.WriteLine("YOU DIED"); }

        public void PrintScore(int score) { Console.Title = $"当前分数：{score}"; }
    }
}