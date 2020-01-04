using System;
using System.Collections.Generic;
using System.Text;

namespace MercuryGames.Shared {
    public class TileMap<T> {
        public int Height { get; }
        public int Width { get; }

        private T[,] map;

        public TileMap(int w, int h) {
            this.Width = w;
            this.Height = h;
            this.map = new T[w, h];
        }

        public T this[Point index] {
            get {
                return this.map[index.X, index.Y];
            }
            set {
                this.map[index.X, index.Y] = value;
            }
        }

        public T this[int a, int b] {
            get {
                return this.map[a, b];
            }
            set {
                this.map[a, b] = value;
            }
        }

        public void Fill(T type) {
            for (int i = 0; i < this.Width; i++) {
                for (int j = 0; j < this.Height; j++) {
                    this.map[i, j] = type;
                }
            }
        }
    }
}