using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public float horizontal_speed = 6f;
    public float vertical_speed = 4;
    public GameObject bullet;

    public Joystick joystick;

    public static bool finished;
    public GameObject endGameMenu;

    private void Awake() {
        instance = this;
    }

    public void Shoot()
    {
        ClientSend.Shoot();
        GameObject new_bullet = Instantiate(bullet, transform.position, Quaternion.identity);
        BulletScript bullet_script = new_bullet.GetComponent<BulletScript>();
        bullet_script.setOwner("player");
        bullet_script.setDirection(new Vector3(0, 1, 0));
    }

    /*private float GetXRate()
    {
        return Input.GetAxis("Horizontal") + joystick.Horizontal;
    }

    private void Move(float x_rate)
    {
        Vector3 new_position = transform.position;
        new_position.x += x_rate * horizontal_speed * Time.deltaTime;
        new_position.y += vertical_speed * Time.deltaTime;
        transform.position = new_position;
    }*/

    private void FixedUpdate() {
        if (!finished) {
            SendInputToServer();
        }
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

    private void Update()
    {
        //float x_rate = GetXRate();
        //Move(x_rate);
        if (Input.GetKeyDown("space"))
        {
            Shoot();
        }
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("ground"))
        {
            Die();
        }

        if (coll.transform.tag == "Finish") {
            Finish();
        }
    }

    public static void Finish() {
        Debug.Log("Finish");

        finished = true;
        ClientSend.Finish();
    }

    public static void Win() {
        GameObject instance_endgame_menu = Instantiate(instance.endGameMenu, new Vector2(0, 0), Quaternion.identity) as GameObject;
        instance_endgame_menu.transform.Find("Canvas").transform.Find("Win").gameObject.SetActive(true);
    }

    public static void Lose() {
        GameObject instance_endgame_menu = Instantiate(instance.endGameMenu, new Vector2(0, 0), Quaternion.identity) as GameObject;
        instance_endgame_menu.transform.Find("Canvas").transform.Find("Lose").gameObject.SetActive(true);
    }

    public static void Die() {
        if (!finished) {
            ClientSend.Die();
        }
    }
}
