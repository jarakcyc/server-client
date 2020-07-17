using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;

    public GameObject bullet;

    public void Shoot() {
        GameObject new_bullet = Instantiate(bullet, transform.position, Quaternion.identity);
        BulletScript bullet_script = new_bullet.GetComponent<BulletScript>();
        bullet_script.setOwner("enemy");
        bullet_script.setDirection(new Vector3(0, 1, 0));
    }
}
