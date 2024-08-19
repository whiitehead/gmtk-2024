using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventScript : MonoBehaviour
{
    public GameObject player;
    public GameObject screen;


    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("other");

        screen.SetActive(false);    


    }

}
