using System;
using System.Collections.Generic;
using System.Text;

namespace Server {
    class SessionManager {
        public static int maxSessions = 100;
        public static Dictionary<int, Session> sessions = new Dictionary<int, Session>();

        private static Queue<int> waiterIds = new Queue<int>();

        public static void NewWaiter(int id) {
            waiterIds.Enqueue(id);
        }

        public static void Update() {
            if (waiterIds.Count >= Session.size) {
                // Make new session
                int[] currentGroup = new int[Session.size];
                for (int i = 0; i < Session.size; ++i) {
                    currentGroup[i] = waiterIds.Peek();
                    waiterIds.Dequeue();
                }

                for (int i = 1; i <= maxSessions; ++i) {
                    if (sessions[i].clientIds == null) {
                        sessions[i].MakeNew(currentGroup);
                        return;
                    }
                }

                Console.WriteLine("No free session.");
            }
        }

        public static void Initialize() {
            for (int i = 1; i <= maxSessions; ++i) {
                sessions.Add(i, new Session(i));
            }
        }
    }
}
