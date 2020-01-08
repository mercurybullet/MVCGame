using System;
using System.Collections.Generic;
using System.Text;

namespace MercuryGames.Shared {
    public interface IModel <T> {
        IEnumerable<Point> Update(params GameKey[] input);

        bool IsWin();

        bool IsDead();

        void Reset();

        int Score { get; }

        int Width { get; }

        int Height { get; }

        IReadOnlyTileMap<T> Map { get; }
    }
}