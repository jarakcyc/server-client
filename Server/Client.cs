using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Data;
using System.Numerics;

namespace Server {
    class Client {
        public static int kBufferSize = 4096;

        public int id;
        public int sessionId;
        public Player player;
        public TCP tcp;
        public UDP udp;

        public Client(int _id) {
            id = _id;
            sessionId = 0; // no session
            tcp = new TCP(_id);
            udp = new UDP(_id);
        }

        public class TCP {
            public TcpClient socket;

            private readonly int id;

            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            public TCP(int _id) {
                id = _id;
            }

            public void Connect(TcpClient _socket) {
                socket = _socket;
                socket.ReceiveBufferSize = kBufferSize;
                socket.SendBufferSize = kBufferSize;

                stream = socket.GetStream();

                receivedData = new Packet();
                receiveBuffer = new byte[kBufferSize];

                stream.BeginRead(receiveBuffer, 0, kBufferSize, ReceiveCallback, null);

                Console.WriteLine("Send welcome...");
                ServerSend.Welcome(id, "Welcome to the server, my friend!");
            }

            public void SendData(Packet packet) {
                try {
                    if (socket != null) {
                        stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    }
                } catch (Exception ex) {
                    Console.WriteLine($"Error while sending data to {id} via TCP: {ex}");
                }
            }

            private void ReceiveCallback(IAsyncResult result) {
                try {
                    int bytes_cnt = stream.EndRead(result);
                    if (bytes_cnt <= 0) {
                        Server.clients[id].Disconnect();
                        return;
                    }

                    byte[] data = new byte[bytes_cnt];
                    Array.Copy(receiveBuffer, data, bytes_cnt);

                    receivedData.Reset(HandleData(data));

                    stream.BeginRead(receiveBuffer, 0, kBufferSize, ReceiveCallback, null);
                } catch (Exception ex) {
                    Console.WriteLine($"error receiving TCP data: {ex}");
                    Server.clients[id].Disconnect();
                }
            }

            private bool HandleData(byte[] data) {
                int packetLength = 0;

                receivedData.SetBytes(data);

                if (receivedData.UnreadLength() >= 4) {
                    packetLength = receivedData.ReadInt();
                    if (packetLength <= 0) {
                        return true;
                    }
                }

                while (packetLength > 0 && packetLength <= receivedData.UnreadLength()) {
                    byte[] packetBytes = receivedData.ReadBytes(packetLength);
                    ThreadManager.ExecuteOnMainThread(() => {
                        using (Packet packet = new Packet(packetBytes)) {
                            int packetId = packet.ReadInt();
                            Server.packetHandlers[packetId](id, packet);
                        }
                    });

                    packetLength = 0;
                    if (receivedData.UnreadLength() >= 4) {
                        packetLength = receivedData.ReadInt();
                        if (packetLength <= 0) {
                            return true;
                        }
                    }
                }

                if (packetLength <= 1) {
                    return true;
                }

                return false;
            }

            public void Disconnect() {
                socket.Close();
                stream = null;
                receivedData = null;
                receiveBuffer = null;
                socket = null;
            }
        }

        public class UDP {
            public IPEndPoint endPoint;

            private int id;

            public UDP(int _id) {
                id = _id;
            }

            public void Connect(IPEndPoint _endPoint) {
                endPoint = _endPoint;
                ServerSend.UDPTest(id);
            }

            public void SendData(Packet packet) {
                Server.SendUDPData(endPoint, packet);
            }

            public void HandleData(Packet packetData) {
                int packetLength = packetData.ReadInt();
                byte[] packetBytes = packetData.ReadBytes(packetLength);

                ThreadManager.ExecuteOnMainThread(() => {
                    using (Packet packet = new Packet(packetBytes)) {
                        int packetId = packet.ReadInt();
                        Server.packetHandlers[packetId](id, packet);
                    }
                });
            }

            public void Disconnect() {
                endPoint = null;
            }
        }

        public void SendIntoGame(string playerName) {
            player = new Player(id, playerName, Constants.START_POSITION, sessionId);

            foreach (int clientId in SessionManager.sessions[sessionId].clientIds) {
                if (Server.clients[clientId].player != null) {
                    ServerSend.SpawnPlayer(clientId, player);
                }
            }
            foreach (int clientId in SessionManager.sessions[sessionId].clientIds) {
                if (clientId != id) {
                    if (Server.clients[clientId].player != null) {
                        ServerSend.SpawnPlayer(id, Server.clients[clientId].player);
                    }
                }
            }
        }

        private void Disconnect() {
            Console.WriteLine($"{tcp.socket.Client.RemoteEndPoint} has disconnected.");

            if (sessionId != 0) {
                SessionManager.sessions[sessionId].PlayerLeaveSession(id);
            }

            player = null;

            tcp.Disconnect();
            udp.Disconnect();
        }
    }
}
