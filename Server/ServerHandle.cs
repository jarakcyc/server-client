using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Server {
    class ServerHandle {
        public static void WelcomeReceived(int fromClient, Packet packet) {
            int clientIdCheck = packet.ReadInt();
            string username = packet.ReadString();

            Console.WriteLine($"{Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint} connected sucsessfully and is now player {fromClient}.");
            if (fromClient != clientIdCheck) {
                Console.WriteLine($"Player {username} (ID: {fromClient}) has assumed wrong client ID: {clientIdCheck}");
            }

            // Send player into game
            //Server.clients[fromClient].SendIntoGame(username);

            SessionManager.NewWaiter(fromClient);
            SessionManager.Update();
        }

        public static void SessionPrepared(int fromClient, Packet packet) {
            Server.clients[fromClient].SendIntoGame($"{fromClient}");
        }

        public static void Finish(int fromClient, Packet packet) {
            int sessionId = Server.clients[fromClient].sessionId;
            SessionManager.sessions[sessionId].PlayerFinish(fromClient);
        }

        public static void Die(int fromClient, Packet packet) {
            Server.clients[fromClient].player.position = Constants.START_POSITION;
            ServerSend.Respawn(fromClient);
        }

        public static void Shoot(int fromClient, Packet packet) {
            ServerSend.PlayerShot(Server.clients[fromClient].player);
        }

        public static void UDPTestReceived(int fromClient, Packet packet) {
            string msg = packet.ReadString();

            Console.WriteLine($"Received packet via UDP. Message: {msg}");
        }

        public static void PlayerMovement(int fromClient, Packet packet) {
            bool[] inputs = new bool[packet.ReadInt()];
            for (int i = 0; i < inputs.Length; ++i) {
                inputs[i] = packet.ReadBool();
            }
            Quaternion rotation = packet.ReadQuaternion();

            Server.clients[fromClient].player.SetInput(inputs, rotation);
        }
    }
}
