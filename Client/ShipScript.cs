using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ShipScript : MonoBehaviour
{
    enum direction
    {
        LEFT,
        RIGHT
    }

    private Transform ship_transform;
    private direction current_direction = direction.LEFT;
    private float start_time;

    public float speed;
    public float activation_time;

    private void Start() 
    {
        start_time = Time.time;
        ship_transform = GetComponent<Transform> ();
    }

    void Update()
    {
        if (Time.time - start_time > activation_time)
        {
            if (current_direction == direction.LEFT)
            {
                ship_transform.position += new Vector3(-1f, 0f, 0f) * speed * Time.deltaTime;
            }
            else if (current_direction == direction.RIGHT)
            {
                ship_transform.position += new Vector3(1f, 0f, 0f) * speed * Time.deltaTime;
            }
        }
    }

    public void changeDirection()
    {
        Vector3 current_scale = ship_transform.localScale;
        current_scale.x = -current_scale.x;
        ship_transform.localScale = current_scale;
        if (current_direction == direction.LEFT)
        {
            current_direction = direction.RIGHT;
        } else if (current_direction == direction.RIGHT)
        {
            current_direction = direction.LEFT;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "ground") 
        {
            changeDirection();
        }
    }
}
