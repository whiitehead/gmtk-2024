
using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Hazard : MonoBehaviour
{
    private Collider2D _thisCollider;
    private void Awake()
    {
        _thisCollider = GetComponent<Collider2D>();
        if (_thisCollider == null)
        {
            Debug.LogError("Cannot find collider! " + this.gameObject.name);
        }

        _thisCollider.isTrigger = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Collision!" + other.gameObject.name);
    }
}
