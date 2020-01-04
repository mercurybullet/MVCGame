using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MercuryGames.Shared {
    public interface IView <T> {
        void PrintScore(int score);

        void Clean();

        void UpdateTile(Point p, T type);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">逻辑层面上图块的类型</typeparam>
    /// <typeparam name="U">显示层面上图块的类型</typeparam>
    public interface IView <T, U> : IView<T> {
        void SetTextureMap(IDictionary<T, U> map);
    }
}