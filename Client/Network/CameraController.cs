using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float offset = 3;
    private Transform target;

    private void Update() {
        GameObject player = GameObject.FindWithTag("player");
        if (player != null) {
            target = player.transform;
            Vector3 new_position = target.position;
            new_position.y += offset;
            new_position.z = transform.position.z;
            transform.position = new_position;
        }
    }
}
