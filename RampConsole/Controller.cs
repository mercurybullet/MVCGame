using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MercuryGames.Shared;

namespace RampConsole {
    class Controller <T> {
        private IModel<T> model;
        private IView<T> view;

        #region Game status

        private Nullable<GameKey> lastInputKey = null;
        private bool gamePaused = false;
        private bool gameGoingToReset = false;
        private int frameInterval = 100; // milliseconds

        #endregion

        private Task inputHandelTask;

        public Controller(IModel<T> m, IView<T> v) {
            this.model = m;
            this.view = v;
            this.inputHandelTask = new Task(() => {
                                                while (true) {
                                                    this.HandelUserInput();
                                                }
                                            });
        }

        public void GameMain() {
            this.InitView();
            this.inputHandelTask.Start();


            while (true) {
                if (this.gameGoingToReset) {
                    this.Reset();
                }

                this.ProcessGameFrame();

                Thread.Sleep(this.frameInterval);
            }
        }

        public void ProcessGameFrame() {
            if (this.gamePaused) {
                return;
            }

            // lastInputKey一次性使用
            var key = this.lastInputKey;
            this.lastInputKey = null;

            IEnumerable<TileUpdateInfo<T>> update;
            update = key == null ? this.model.Update() : this.model.Update(key.Value);

            this.UpdateView(update);

            this.view.PrintScore(this.model.Score);

            if (this.model.IsWin()) {
                Console.WriteLine("player win.");
                this.HandelGameOverInput();
            }

            if (this.model.IsDead()) {
                Console.WriteLine("YOU DIED.");
                this.HandelGameOverInput();
            }
        }

        public void HandelUserInput() {
            ConsoleKeyInfo input = Console.ReadKey(true); //当传入的参数为true时，不会在控制台回显
            switch (input.Key) {
            case ConsoleKey.RightArrow:
                this.lastInputKey = GameKey.Right;
                break;
            case ConsoleKey.LeftArrow:
                this.lastInputKey = GameKey.Left;
                break;
            case ConsoleKey.UpArrow:
                this.lastInputKey = GameKey.Up;
                break;
            case ConsoleKey.DownArrow:
                this.lastInputKey = GameKey.Down;
                break;
            case ConsoleKey.A:
                this.lastInputKey = GameKey.A;
                this.gamePaused = !this.gamePaused;
                break;
            case ConsoleKey.Escape:
                this.lastInputKey = GameKey.Start;
                this.gameGoingToReset = true;
                break;
            case ConsoleKey.B:
                this.lastInputKey = GameKey.B;
                break;
            }
        }

        public void HandelGameOverInput() {
            if (Console.ReadKey().Key == ConsoleKey.Escape) {
                Environment.Exit(0);
                return;
            }
            else {
                this.Reset();
            }
        }

        private void Reset() {
            this.lastInputKey = null;
            this.gamePaused = false;
            this.gameGoingToReset = false;
            this.view.Clean();
            this.model.Reset();
            this.InitView();
        }

        //画出地雷
        public void InitView() {
            this.view.PrintScore(this.model.Score);
            for (int i = 0; i < this.model.Width; i++) {
                for (int j = 0; j < this.model.Height; j++) {
                    this.view.UpdateTile(new Point(i, j), this.model.Map[i, j]);
                }
            }

            this.UpdateView(this.model.GetFullPlayerInfo());
        }

        public void UpdateView(IEnumerable<TileUpdateInfo<T>> update) {
            foreach (var info in update) {
                this.view.UpdateTile(info.P, info.Type);
            }
        }
    }
}