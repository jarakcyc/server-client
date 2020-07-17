using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace Server {
    class Server {
        public static int maxPlayers;
        public static int port;

        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public delegate void PacketHandler(int fromClient, Packet packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        private static TcpListener tcpListener;
        private static UdpClient udpListener;

        public static void Start(int _maxPlayers, int _port) {
            maxPlayers = _maxPlayers;
            port = _port;

            Console.WriteLine("Starting accept clients...");

            InitializeServerData();
            SessionManager.Initialize();

            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(TCPConnectionCallback, null);

            udpListener = new UdpClient(port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            Console.WriteLine($"Server started on port {port}");
        }

        private static void TCPConnectionCallback(IAsyncResult result) {
            TcpClient client = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(TCPConnectionCallback, null);

            Console.WriteLine($"Incoming connection from {client.Client.RemoteEndPoint}");

            for (int i = 1; i <= maxPlayers; ++i) {
                if (clients[i].tcp.socket == null) {
                    Console.WriteLine($"Connect on place: {i}");
                    clients[i].tcp.Connect(client);
                    return;
                }
            }

            Console.WriteLine($"{client.Client.RemoteEndPoint} failed to connect. Server is full!");
        }

        private static void UDPReceiveCallback(IAsyncResult result) {
            try {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpListener.EndReceive(result, ref clientEndPoint); // clientEndPoint will be set up here
                udpListener.BeginReceive(UDPReceiveCallback, null);

                if (data.Length < 4) {
                    return;
                }

                using (Packet packet = new Packet(data)) {
                    int clientId = packet.ReadInt();

                    if (clientId == 0) {
                        return;
                    }

                    if (clients[clientId].udp.endPoint == null) {
                        clients[clientId].udp.Connect(clientEndPoint);
                        return;
                    }

                    if (clients[clientId].udp.endPoint.ToString() == clientEndPoint.ToString()) {
                        clients[clientId].udp.HandleData(packet);
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine($"Error receiving UDP data: {ex}");
            }
        }

        public static void SendUDPData(IPEndPoint clientEndPoint, Packet packet) {
            try {
                if (clientEndPoint != null) {
                    udpListener.BeginSend(packet.ToArray(), packet.Length(), clientEndPoint, null, null);
                }
            } catch (Exception ex) {
                Console.WriteLine($"Error while sending UDP data: {ex}");
            }
        }

        private static void InitializeServerData() {
            for (int i = 1; i <= maxPlayers; ++i) {
                clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>() {
                { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                { (int)ClientPackets.udpTestReceived, ServerHandle.UDPTestReceived },
                { (int)ClientPackets.playerMovement, ServerHandle.PlayerMovement },
                { (int)ClientPackets.sessionPrepared, ServerHandle.SessionPrepared },
                { (int)ClientPackets.finish, ServerHandle.Finish },
                { (int)ClientPackets.die, ServerHandle.Die },
                { (int)ClientPackets.shoot, ServerHandle.Shoot }
            };
            Console.WriteLine("Initialized packets.");
        }
    }
}
