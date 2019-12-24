using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MercuryGames.Shared;

namespace RampConsole {
    class Controller {
        private Model model;
        private View view;

        #region Game status

        private GameKey lastDirection = GameKey.Up;
        private bool gamePaused = false;
        #endregion

        private Task inputHandelTask;

        public Controller(Model m, View v) {
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
                this.ProcessGameFrame();
                Thread.Sleep(1000);
            }
        }

        public void ProcessGameFrame() {
            if (this.gamePaused) {
                return;
            }

            var update = this.model.ProcessUserInput(this.lastDirection);

            this.UpdatePlayerToView(update);

            if (this.model.TestBomb()) {
                this.view.PrintDieMessage();
                this.HandelGameOverInput();
            }
        }

        public void HandelUserInput() {
            ConsoleKeyInfo input = Console.ReadKey(true); //当传入的参数为true时，不会在控制台回显
            switch (input.Key) {
            case ConsoleKey.RightArrow:
                this.lastDirection = GameKey.Right;
                break;
            case ConsoleKey.LeftArrow:
                this.lastDirection = GameKey.Left;
                break;
            case ConsoleKey.UpArrow:
                this.lastDirection = GameKey.Up;
                break;
            case ConsoleKey.DownArrow:
                this.lastDirection = GameKey.Down;
                break;
            case ConsoleKey.A:
                this.gamePaused = !this.gamePaused;
                break;
            case ConsoleKey.B:
                this.view.Clean();
                this.model.Reset();
                this.InitView();
                    break;
            }
        }

        public void HandelGameOverInput() {
            if (Console.ReadKey().Key == ConsoleKey.Escape) {
                Environment.Exit(0);
                return;
            }
            else {
                this.view.Clean();
                this.model.Reset();
                this.InitView();
            }
        }


        //画出地雷
        public void InitView() {
            for (int i = 0; i < this.model.Width; i++) {
                for (int j = 0; j < this.model.Height; j++) {
                    this.view.UpdateTile(new Point(i, j), this.model.Map[i, j]);
                }
            }
        }

        public void UpdatePlayerToView(IEnumerable<Model.TileUpdateInfo> update) {
            foreach (var info in update) {
                this.view.UpdateTile(info.P, info.Type);
            }
        }
    }
}