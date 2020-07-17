using System;
using System.Collections.Generic;
using System.Text;

namespace Server {
    class Session {
        public static int size = 1;

        public int id;
        public int[] clientIds = null;
        public bool[] finished = null;
        public int winner = 0;
        public int left = 0;

        public Session(int _id) {
            id = _id;
        }

        public void MakeNew(int[] _clientIds) {
            Console.WriteLine($"Make new session with id {id}");
            clientIds = new int[_clientIds.Length];
            finished = new bool[_clientIds.Length];
            winner = 0;
            left = 0;

            for (int i = 0; i < clientIds.Length; ++i) {
                clientIds[i] = _clientIds[i];
                finished[i] = false;
                Server.clients[clientIds[i]].sessionId = id;
            }

            for (int i = 0; i < clientIds.Length; ++i) {
                ServerSend.PrepareSession(clientIds[i]);
            }
        }

        public void PlayerFinish(int playerId) {
            Console.WriteLine($"player {playerId} finished");
            if (winner == 0) {
                winner = playerId;
                ServerSend.Win(playerId);
            } else {
                ServerSend.Lose(playerId);
            }

            for (int i = 0; i < clientIds.Length; ++i) {
                if (playerId == clientIds[i]) {
                    finished[i] = true;
                    Server.clients[playerId].player.finished = true;
                }
            }
        }

        public void PlayerLeaveSession(int playerId) {
            Console.WriteLine($"Player {playerId} leave session with id {id}");
            for (int i = 0; i < clientIds.Length; ++i) {
                if (clientIds[i] == playerId) {
                    left++;
                    break;
                }
            }

            if (left == clientIds.Length) {
                CloseSession();
            }
        }

        private void CloseSession() {
            Console.WriteLine($"Close session with id {id}");
            clientIds = null;
        }
    }
}
