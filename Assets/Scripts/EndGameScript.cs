using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EndGameScript : MonoBehaviour
{
    public GameObject player;
    public GameObject text;


    private void OnTriggerEnter2D(Collider2D other)
    {

        text.SetActive(true);    


    }

}
