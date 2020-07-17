using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BulletScript : MonoBehaviour {
	public float speed = 10;
	public float life_time = 1;
	public int bullet_damage = 40;

	private float timer = 0f;
	private Transform bullet_transform;

	private string owner_tag;
	private Vector3 direction = new Vector3(0f, 1f, 0f);

	void Start () 
	{
		bullet_transform = GetComponent<Transform> ();
	}

	void Update () 
	{
		timer += Time.deltaTime;
		if (timer >= life_time)
		{
			Destroy(gameObject);
		}

		bullet_transform.position += direction * speed * Time.deltaTime;
	}

	public void setOwner(string new_owner_tag) 
	{
		owner_tag = new_owner_tag;
	}

	public void setDirection(Vector3 new_direction)
	{
		direction = new_direction;
	}

	public string getOwnerTag()
	{
		return owner_tag;
	}

	private void OnTriggerEnter2D(Collider2D coll)
    {
		if (coll.gameObject.CompareTag(owner_tag))
        {
			return;
        }
		HP hp = coll.gameObject.GetComponent<HP>();
		if (hp == null)
        {
			return;
        }
		hp.TakeDamage(bullet_damage);
    }
}
