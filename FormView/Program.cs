using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormView
{
    using System.Drawing;
    using MercuryGames.Snake;

    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var model = new SnakeModel(10, 10);
            Form1 view = new Form1();
            view.InitView(10, 10);
            view.SetTextureMap(new Dictionary<SnakeTile, Image>()
            {
                { SnakeTile.Blank, Image.FromFile(@"C:\Users\xiwyu\Pictures\blank.bmp")},
                { SnakeTile.Snake, Image.FromFile(@"C:\Users\xiwyu\Pictures\snake.bmp")},
                { SnakeTile.Food, Image.FromFile(@"C:\Users\xiwyu\Pictures\food.bmp")},
                { SnakeTile.Grass, Image.FromFile(@"C:\Users\xiwyu\Pictures\grass.bmp")}
            });
            view.KeyPreview = true;

            var controller = new WinFormController<SnakeTile>(model, view);
            view.KeyDown += controller.HandelUserInput;
            controller.InitView();
            controller.BindSelfToTimer(view.Timer1);

            Application.Run(view);

        }
    }
}
