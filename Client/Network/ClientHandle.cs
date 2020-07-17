using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet packet) {
        string message = packet.ReadString();
        int myId = packet.ReadInt();

        Debug.Log($"Message from the server: {message}");
        Client.myId = myId;

        ClientSend.WelcomeReceived();

        Client.udp.Connect(((IPEndPoint)Client.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void UDPTest(Packet packet) {
        string msg = packet.ReadString();

        Debug.Log($"Received packet via UDP. Message: {msg}");
        ClientSend.UDPTestReceived();
    }

    public static void PrepareSession(Packet packet) {
        string msg = packet.ReadString();
        Debug.Log(msg);
        GameManager.instance.PrepareSession();
    }

    public static void SpawnPlayer(Packet packet) {
        int id = packet.ReadInt();
        string username = packet.ReadString();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();

        GameManager.instance.SpawnPlayer(id, username, position, rotation);
    }

    public static void PlayerPosition(Packet packet) {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();

        if (GameManager.players.ContainsKey(id)) {
            GameManager.players[id].transform.position = position;
        } else {
            Debug.Log($"GameManager doesn't know about player {id}");
        }
    }

    public static void PlayerRotation(Packet packet) {
        int id = packet.ReadInt();
        Quaternion rotation = packet.ReadQuaternion();

        //GameManager.players[id].transform.rotation = rotation;
    }

    public static void Win(Packet packet) {
        Debug.Log("Win");
        PlayerController.Win();
    }

    public static void Lose(Packet packet) {
        Debug.Log("Lose");
        PlayerController.Lose();
    }

    public static void Respawn(Packet packet) {
        Debug.Log("Respawn");
        PlayerController.instance.GetComponent<HP>().SetHPToMax();
    }

    public static void PlayerShot(Packet packet) {
        int playerId = packet.ReadInt();
        GameManager.players[playerId].Shoot();
    }
}
