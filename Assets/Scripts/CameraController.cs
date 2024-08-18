using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    public GameObject Guy;
    public float Zoom = 10.0f;
    
    private Vector2 _target;
    private CircleCollider2D _guyCollider;
    
    private void Start()
    {
        _guyCollider = Guy.GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        Vector2 min = _guyCollider.bounds.min;
        Vector2 max = _guyCollider.bounds.max;
            
        foreach (var boxCollider in Guy.GetComponentsInChildren<BoxCollider2D>())
        {
            min = Vector2.Min(min, boxCollider.bounds.min);
            max = Vector2.Max(max, boxCollider.bounds.max);
        }
        
        _target = Vector2.Lerp(min, max, 0.5f);
        
        transform.position = new Vector3(_target.x, _target.y, -Zoom);
    }
}
