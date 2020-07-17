using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet packet) {
        packet.WriteLength();
        Client.tcp.SendData(packet);
    }

    private static void SendUDPData(Packet packet) {
        packet.WriteLength();
        Client.udp.SendData(packet);
    }

    #region Packets

    public static void WelcomeReceived() {
        using (Packet packet = new Packet((int)ClientPackets.welcomeReceived)) {
            packet.Write(Client.myId);
            packet.Write(UIManager.instance.usernameField.text);

            SendTCPData(packet);
        }
    }

    public static void SessionPrepared() {
        using (Packet packet = new Packet((int)ClientPackets.sessionPrepared)) {
            packet.Write("Session prepared.");
            SendTCPData(packet);
        }
    }

    public static void UDPTestReceived() {
        using (Packet packet = new Packet((int)ClientPackets.udpTestReceived)) {
            packet.Write("Received a UDP packet.");

            SendUDPData(packet);
        }
    }

    public static void PlayerMovement(bool[] inputs) {
        using (Packet packet = new Packet((int)ClientPackets.playerMovement)) {
            packet.Write(inputs.Length);
            foreach (bool input in inputs) {
                packet.Write(input);
            }
            packet.Write(GameManager.players[Client.myId].transform.rotation);

            SendUDPData(packet);
        }
    }

    public static void Finish() {
        using (Packet packet = new Packet((int)ClientPackets.finish)) {
            packet.Write("Finished");

            SendTCPData(packet);
        }
    }

    public static void Die() {
        using (Packet packet = new Packet((int)ClientPackets.die)) {
            packet.Write("Die");

            SendTCPData(packet);
        }
    }

    public static void Shoot() {
        using (Packet packet = new Packet((int)ClientPackets.shoot)) {
            packet.Write("Shot");

            SendTCPData(packet);
        }
    }

    #endregion
}
