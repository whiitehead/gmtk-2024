using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventScript : MonoBehaviour
{
    public GameObject player;
    public AudioSource audio;
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        audio.Play();  


    }

}
