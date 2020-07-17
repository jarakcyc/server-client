using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerController : MonoBehaviour
{
    private void FixedUpdate() {
        SendInputToServer();
    }

    private void SendInputToServer() {
        bool[] inputs = new bool[] {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D)
        };

        ClientSend.PlayerMovement(inputs);
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        Debug.Log("Collision detected");
        if (collision.transform.tag == "Finish") {
            Debug.Log("Finish");
            ClientSend.Finish();
        }
    }
}
