using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MercuryGames.Shared {
    public interface IReadOnlyTileMap <T> {
        int Height { get; }
        int Width { get; }
        T this[Point position] { get; }
        T this[int x, int y] { get; }
    }

    public interface ITileMap <T>: IReadOnlyTileMap<T> {
        new T this[Point position] { get; set; }
        new T this[int x, int y] { get; set; }

        void Fill(T type);
    }

    public class DoubleLayerTileMap <T> : IReadOnlyTileMap<T> where T:struct {
        public TileMap<T> Background { get; }
        public TileMap<T?> Foreground { get; }

        public int Height { get; }
        public int Width { get; }

        public DoubleLayerTileMap(int w, int h) {
            this.Height = h;
            this.Width = w;
            this.Background = new TileMap<T>(w, h);
            this.Foreground = new TileMap<T?>(w, h);
        }

        public DoubleLayerTileMap(TileMap<T> bg, TileMap<T?> fg) {
            if (bg.Width != fg.Width || bg.Height != fg.Height) {
                throw new ArgumentException("background and foreground differ in size.");
            }

            this.Background = bg;
            this.Foreground = fg;
        }

        public T this[Point position] {
            get {
                T? f = this.Foreground[position];
                if (f == null) {
                    return this.Background[position];
                }
                else {
                    return f.Value;//因为前面的f带问号，所以这里返回f不行，需要返回f的值
                }
            }
        }

        public T this[int x, int y] {
            get {
                T? f = this.Foreground[x, y];
                if (f == null) {
                    return this.Background[x, y];
                }
                else {
                    return f.Value;
                }
            }
        }
    }

    public class TileMap<T> : ITileMap<T> {
        public int Height { get; }
        public int Width { get; }

        private T[,] map;

        public TileMap(int w, int h) {
            this.Width = w;
            this.Height = h;
            this.map = new T[w, h];
        }

        public T this[Point position] {
            get {
                return this.map[position.X, position.Y];
            }
            set {
                this.map[position.X, position.Y] = value;
            }
        }

        public T this[int x, int y] {
            get {
                return this.map[x, y];
            }
            set {
                this.map[x, y] = value;
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