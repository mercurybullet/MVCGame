using System;
using System.Collections.Generic;
using System.Text;

namespace MercuryGames.Shared {
    public class FrameSkipper {
        public int FramePerUpdate { get; set; }

        private int current;

        public FrameSkipper(int count) {
            this.FramePerUpdate = count;
            this.current = 1;
        }

        public bool Tick() {
            if (this.current < this.FramePerUpdate) {
                this.current += 1;
                return false;
            }

            this.current = 1;
            return true;
        }
    }
}
