using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FormView
{
    using System.Threading;
    using System.Windows.Forms;
    using MercuryGames.Shared;
    using Timer = System.Windows.Forms.Timer;

    class WinFormController<T>
    {
        private IModel<T> model;
        private IView<T> view;

        private Nullable<GameKey> lastInputKey = null;
        private bool gamePaused = false;
        private bool gameGoingToReset = false;
        private int frameInterval = 100; // milliseconds
        private Timer timer;

        public WinFormController(IModel<T> m, IView<T> v)
        {
            this.model = m;
            this.view = v;
        }

        public void BindSelfToTimer(Timer timer)
        {
            this.timer = timer;
            timer.Enabled = true;
            timer.Interval = this.frameInterval;
            timer.Tick += this.ProcessGameFrame;
            timer.Start();
        }

        public void ProcessGameFrame(object o, EventArgs e)
        {
            if (this.gameGoingToReset)
            {
                this.Reset();
            }

            if (this.gamePaused)
            {
                return;
            }

            this.timer.Enabled = false;

            // lastInputKey一次性使用
            var key = this.lastInputKey;
            this.lastInputKey = null;

            IEnumerable<Point> update;
            update = key == null ? this.model.Update() : this.model.Update(key.Value);

            this.UpdateView(update);

            this.view.PrintScore(this.model.Score);

            if (this.model.IsWin())
            {
                //Console.WriteLine("player win.");
                DialogResult userChoice = MessageBox.Show("You Win", "Notice", MessageBoxButtons.OKCancel);
                this.HandelGameOverInput(userChoice);
            }

            if (this.model.IsDead())
            {
                DialogResult userChoice = MessageBox.Show("You DIED", "Notice", MessageBoxButtons.OKCancel);
                this.HandelGameOverInput(userChoice);
            }

            this.timer.Enabled = true;
        }

        public void HandelUserInput(object sender, KeyEventArgs e)
        {
            Keys key = e.KeyCode;
            switch (key)
            {// ConsoleKey是控制台专用的，咱们这个controller要怎么获得winform的键盘输入信息呢？
                case Keys.Right:
                    this.lastInputKey = GameKey.Right;
                    break;
                case Keys.Left:
                    this.lastInputKey = GameKey.Left;
                    break;
                case Keys.Up:
                    this.lastInputKey = GameKey.Up;
                    break;
                case Keys.Down:
                    this.lastInputKey = GameKey.Down;
                    break;
                case Keys.A:
                    this.lastInputKey = GameKey.A;
                    this.gamePaused = !this.gamePaused;
                    break;
                case Keys.Escape:
                    this.lastInputKey = GameKey.Start;
                    this.gameGoingToReset = true;
                    break;
                case Keys.B:
                    this.lastInputKey = GameKey.B;
                    break;
            }
        }

        public void HandelGameOverInput(DialogResult userChoice)
        {
            if (userChoice==DialogResult.Yes || userChoice == DialogResult.OK)
            {
                this.Reset();
            }
            else
            {
                Environment.Exit(0);
            }
        }

        private void Reset()
        {
            this.lastInputKey = null;
            this.gamePaused = false;
            this.gameGoingToReset = false;
            this.view.Clean();
            this.model.Reset();
            this.InitView();
        }

        //画出地雷
        public void InitView()
        {
            this.view.PrintScore(this.model.Score);
            this.view.DrawTileMap(this.model.Map);
        }

        public void UpdateView(IEnumerable<Point> update)
        {
            foreach (var p in update)
            {
                this.view.UpdateTile(p, this.model.Map[p]);
            }
        }
    }
}
