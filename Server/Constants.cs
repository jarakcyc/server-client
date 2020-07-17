using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace Server {
    class Constants {
        public const int TICKS_PER_SEC = 60;
        public const int MS_PER_TICK = 1000 / TICKS_PER_SEC;
        public static Vector3 START_POSITION = new Vector3(0, -7, 0);
    }
}
