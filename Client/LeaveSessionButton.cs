using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LeaveSessionButton : MonoBehaviour
{
    Button myButton;
    void Start() {
        myButton = GetComponent<Button>();
        if (myButton) myButton.onClick.AddListener(LeaveSession);
    }

    void LeaveSession() {
        GameManager.LeaveSession();
    }
}
