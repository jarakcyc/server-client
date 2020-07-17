using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachToObject : MonoBehaviour
{
    public float offset = 3;
    public Transform target;

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        Vector3 new_position = target.position;
        new_position.y += offset;
        new_position.z = transform.position.z;
        transform.position = new_position;
    }
}
