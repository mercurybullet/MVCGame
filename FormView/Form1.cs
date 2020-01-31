using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormView
{
    using MercuryGames.Shared;
    using MercuryGames.Snake;
    public partial class Form1 : Form, IView<SnakeTile, Image>
    {
        public Form1()
        {
            InitializeComponent();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {

            if (keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Left || keyData == Keys.Right)
            {
                return false;
            }
            else
            {
                return base.ProcessDialogKey(keyData);
            }
        }

        private PictureBox[,] pbArray;
        private static int tileLength = 32;
        protected IDictionary<SnakeTile, Image> textureMap;

        private int width;
        private int height;

        public void InitView(int w, int h)
        {
            this.width = w;
            this.height = h;
            this.pbArray = new PictureBox[w, h];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    PictureBox pb = new PictureBox();
                    this.pbArray[i, j] = pb;

                    pb.Left = i * Form1.tileLength;
                    pb.Top = j * Form1.tileLength;

                    pb.Height = Form1.tileLength;
                    pb.Width = Form1.tileLength;

                    pb.Parent = this;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        public void PrintScore(int score)
        {
            this.Text = $"当前分数：{score}";
        }

        public void Clean()
        {
            foreach (PictureBox pb in this.pbArray)
            {
                pb.Image = this.textureMap[SnakeTile.Blank];
            }
        }

        public void UpdateTile(Point p, SnakeTile type)
        {
            this.UpdateTile(p.X, p.Y, type);
        }

        public void UpdateTile(int x, int y, SnakeTile type)
        {
            PictureBox pb = this.pbArray[x, this.height - y - 1];
            pb.Image = this.textureMap[type];
        }

        public void DrawTileMap(IReadOnlyTileMap<SnakeTile> map)
        {
            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Height; j++)
                {
                    this.UpdateTile(i, j, map[i, j]);
                }
            }
        }

        public void SetTextureMap(IDictionary<SnakeTile, Image> map)
        {
            this.textureMap = map;
        }
    }
}
