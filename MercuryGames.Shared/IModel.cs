using System;
using System.Collections.Generic;
using System.Text;

namespace MercuryGames.Shared {
    public interface IModel <T> {
        IEnumerable<TileUpdateInfo<T>> Update(params GameKey[] input);

        IEnumerable<TileUpdateInfo<T>> GetFullPlayerInfo();

        bool IsWin();

        bool IsDead();

        void Reset();

        int Score { get; }

        int Width { get; }

        int Height { get; }

        T[,] Map { get; }
    }
}