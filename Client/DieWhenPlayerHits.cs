using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieWhenPlayerHits : MonoBehaviour
{
    public int damage = 40;

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("player"))
        {
            coll.gameObject.GetComponent<HP>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
