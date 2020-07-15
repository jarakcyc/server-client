﻿using System;
using System.Threading;

namespace Server
{
    class Program
    {
        public static bool isRunning = false;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello!");

            isRunning = true;

            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            Server.Start(50, 26950);
        }

        private static void MainThread() {
            Console.WriteLine($"Main thread started. Running at {Constants.TICKS_PER_SEC} ticks per second.");

            DateTime nextLoop = DateTime.Now;
            while (isRunning) {
                while (nextLoop < DateTime.Now) {
                    GameLogic.Update();

                    nextLoop = nextLoop.AddMilliseconds(Constants.MS_PER_TICK);

                    if (nextLoop > DateTime.Now) {
                        Thread.Sleep(nextLoop - DateTime.Now);
                    }
                }
            }
        }
    }
}
