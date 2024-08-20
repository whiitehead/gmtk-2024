using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class FaceController : MonoBehaviour
{
    public Vector2 MaxEyeMovement = new Vector2(0.4f, 0.3f);
    public Transform EyeBallTransform;
    public float EyePartialLookRadius;
    
    private SimpleAnimator[] _simpleAnimators;
    private float _timeUntilBlink = 0;
    
    void Start()
    {
        _simpleAnimators = GetComponentsInChildren<SimpleAnimator>();
    }

    void Update()
    {
        _timeUntilBlink -= Time.deltaTime;
        
        if (_timeUntilBlink <= 0)
        {
            Blink();
            _timeUntilBlink = Random.Range(0.5f, 10.0f);
        }
        
        LookAtMouse();
    }

    void Blink()
    {
        foreach (var animator in _simpleAnimators)
        {
            animator.Play();
        }
    }

    void LookAtMouse()
    {
        Vector2 lookDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        var magnitude = lookDirection.magnitude;
        lookDirection = Quaternion.Inverse(transform.rotation) * lookDirection; // corrects for the guys rotation.
        lookDirection.Normalize();
        lookDirection *= Mathf.Min(magnitude / EyePartialLookRadius, 1.0f); // So he won't look way off to the side when mouse is in front.
        EyeBallTransform.localPosition = new Vector3(lookDirection.x * MaxEyeMovement.x, lookDirection.y * MaxEyeMovement.y, 0);
    }
}
