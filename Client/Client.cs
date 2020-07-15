using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System;
using UnityEngine.XR;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;

public class Client : MonoBehaviour
{
    public static Client instance;
    public static int kBufferSize = 4096;

    public string ip = "192.168.0.105"; // server ip
    public int port = 26950;
    public int myId = 0;
    public TCP tcp;
    public UDP udp;

    private bool isConnected = false;
    private delegate void PacketHandler(Packet packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    private void Awake() {
        if (instance == null) {
            Debug.Log($"New instance of client. Server ip: {ip}");
            instance = this;
        } else if (instance != this) {
            Debug.Log("instance already exists, destroying object");
            Destroy(this);
        }
    }

    private void Start() {
        tcp = new TCP();
        udp = new UDP();
    }

    private void OnApplicationQuit() {
        Disconnect();
    }

    public void ConnectToServer() {
        InitializeClientData();
        isConnected = true;
        tcp.Connect();
    }

    public class TCP {
        public TcpClient socket;

        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        public void Connect() {
            socket = new TcpClient {
                ReceiveBufferSize = kBufferSize,
                SendBufferSize = kBufferSize
            };

            receiveBuffer = new byte[kBufferSize];

            Debug.Log($"Instance try to connect to server with ip: {instance.ip}");
            socket.BeginConnect("192.168.0.105", instance.port, ConnectCallback, socket);
        }

        private void ConnectCallback(IAsyncResult result) {
            socket.EndConnect(result);

            if (!socket.Connected) {
                return;
            }

            stream = socket.GetStream();

            receivedData = new Packet();

            stream.BeginRead(receiveBuffer, 0, kBufferSize, ReceiveCallback, null);
        }

        public void SendData(Packet packet) {
            try {
                if (socket != null) {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            } catch (Exception ex) {
                Debug.Log($"Error while sending TCP data to server: {ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult result) {
            try {
                int bytes_cnt = stream.EndRead(result);
                if (bytes_cnt <= 0) {
                    instance.Disconnect();
                    return;
                }

                byte[] data = new byte[bytes_cnt];
                Array.Copy(receiveBuffer, data, bytes_cnt);

                // handle data
                receivedData.Reset(HandleData(data));

                stream.BeginRead(receiveBuffer, 0, kBufferSize, ReceiveCallback, null);
            }
            catch (Exception ex) {
                Console.WriteLine($"error receiving TCP data: {ex}");
                Disconnect();
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
                        packetHandlers[packetId](packet);
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

        private void Disconnect() {
            instance.Disconnect();

            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }

    public class UDP {
        public UdpClient socket;
        public IPEndPoint endPoint;

        public UDP() {
            endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
        }

        public void Connect(int localPort) {
            socket = new UdpClient(localPort);

            socket.Connect(endPoint);
            socket.BeginReceive(ReceiveCallback, null);

            using (Packet packet = new Packet()) {
                SendData(packet);
            }
        }

        public void SendData(Packet packet) {
            try {
                packet.InsertInt(instance.myId);
                if (socket != null) {
                    socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
                }
            } catch (Exception ex) {
                Debug.Log($"Error while sending UDP data to server: {ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult result) {
            try {
                byte[] data = socket.EndReceive(result, ref endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                if (data.Length < 4) {
                    instance.Disconnect();
                    return;
                }

                HandleData(data);
            } catch (Exception ex) {
                Disconnect();
            }
        }

        private void HandleData(byte[] data) {
            using (Packet packet = new Packet(data)) {
                int packetLength = packet.ReadInt();
                data = packet.ReadBytes(packetLength);
            }

            ThreadManager.ExecuteOnMainThread(() => {
                using (Packet packet = new Packet(data)) {
                    int packetId = packet.ReadInt();
                    packetHandlers[packetId](packet);
                }
            });
        }

        private void Disconnect() {
            instance.Disconnect();

            endPoint = null;
            socket = null;
        }
    }

    private void InitializeClientData() {
        packetHandlers = new Dictionary<int, PacketHandler>() {
            { (int)ServerPackets.welcome, ClientHandle.Welcome },
            { (int)ServerPackets.udpTest, ClientHandle.UDPTest },
            { (int)ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer },
            { (int)ServerPackets.playerPosition, ClientHandle.PlayerPosition },
            { (int)ServerPackets.playerRotation, ClientHandle.PlayerRotation }
        };
        Debug.Log("Initialized packets");
    }

    private void Disconnect() {
        if (isConnected) {
            isConnected = false;
            tcp.socket.Close();
            udp.socket.Close();

            Debug.Log("Disconnected from server.");
        }
    }
}
