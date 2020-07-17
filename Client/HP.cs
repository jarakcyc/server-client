using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    public int max_hp;
    private int hp;
    public TMPro.TextMeshProUGUI text;

    private void Start()
    {
        hp = max_hp;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            if (gameObject.tag == "player") {
                PlayerController.Die();
            }
            else {
                Destroy(gameObject);
            }
        }
    }

    public void SetHPToMax() {
        hp = max_hp;
    }

    private void Update()
    {
        text.text = hp.ToString();
    }
}
