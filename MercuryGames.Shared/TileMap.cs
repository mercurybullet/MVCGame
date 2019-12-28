using System;
using System.Collections.Generic;
using System.Text;

namespace MercuryGames.Shared {
    public class TileMap {
        public int Height { get; }
        public int Width { get; }

        private TileType[,] map;

        public TileMap(int w, int h) {
            this.Width = w;
            this.Height = h;
            this.map = new TileType[w, h];
        }

        public TileType this[Point index] {
            get {
                return this.map[index.X, index.Y];
            }
            set {
                this.map[index.X, index.Y] = value;
            }
        }

        public TileType this[int a, int b] {
            get {
                return this.map[a, b];
            }
            set {
                this.map[a, b] = value;
            }
        }

        public void Fill(TileType type) {
            for (int i = 0; i < this.Width; i++) {
                for (int j = 0; j < this.Height; j++) {
                    this.map[i, j] = type;
                }
            }
        }
    }
}