using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShotController : MonoBehaviour, IPointerDownHandler
{
    GameObject player;

    public void OnPointerDown(PointerEventData eventData) 
    {
        player = GameObject.FindWithTag("player");
        if (player != null) 
        {
            player.GetComponent<PlayerController>().Shoot();
        }
    }
}
