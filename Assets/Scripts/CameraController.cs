using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public struct Bounds2D
{
    public Vector2 Min;
    public Vector2 Max;
    public Vector2 Center => Vector2.Lerp(Min, Max, 0.5f);
    public Vector2 Dimensions => Max - Min;
}

public class CameraController : MonoBehaviour
{
    public GameObject Guy;
    [Tooltip("Height of the viewport in world space unit")]
    public float DefaultSize = 15.0f;
    [Tooltip("Height of viewport for the Intro")]
    public float IntroSize = 5.0f;
    [Tooltip("Viewport height change in units per second")]
    public float ZoomSpeed = 5.0f;
    [Tooltip("Units per second")]
    public float CameraSpeed = 10.0f;
    [Tooltip("Max ratio of the screen the guy can horizontally or vertically fill before the camera grows larger")]
    public float ZoomThreshold = 0.5f;
    public Bounds2D Bounds {
        get
        {
            var verticalExtent = _camera.orthographicSize;
            var sizeDelta = new Vector2(verticalExtent * _camera.aspect, verticalExtent);
        
            return new Bounds2D
            {
                Min = (Vector2)transform.position - sizeDelta,
                Max = (Vector2)transform.position + sizeDelta
            };
        }
    }

    private Vector2 _target;
    private float _targetSize;
    private Camera _camera;

    private float _defaultSize;

    private void Start()
    {
        _defaultSize = IntroSize;
        _targetSize = IntroSize;
        _camera = GetComponent<Camera>();
        _camera.orthographicSize = IntroSize;
    }

    public void EndIntro()
    {
        _defaultSize = DefaultSize;
        _targetSize = DefaultSize;
    }

    private void LateUpdate()
    {
        var guyBounds = new Bounds2D
        {
            Min = Vector2.positiveInfinity,
            Max = Vector2.negativeInfinity
        };
            
        foreach (var childCollider in Guy.GetComponentsInChildren<Collider2D>())
        {
            guyBounds.Min = Vector2.Min(guyBounds.Min, childCollider.bounds.min);
            guyBounds.Max = Vector2.Max(guyBounds.Max, childCollider.bounds.max);
        }
        
        _target = guyBounds.Center;

        var cameraBounds = Bounds;
        var zoomThreshold = cameraBounds.Dimensions * ZoomThreshold;
        
        if (zoomThreshold.x < guyBounds.Dimensions.x || zoomThreshold.y < guyBounds.Dimensions.y)
        {
            _targetSize = Mathf.Max(guyBounds.Dimensions.y / ZoomThreshold, guyBounds.Dimensions.x / ZoomThreshold / _camera.aspect);
        }
        else
        {
            _targetSize = _defaultSize;
        }
        
        MoveTowardsTarget();
    }

    private void MoveTowardsTarget()
    {
        var maxZoomDelta = ZoomSpeed * Time.deltaTime;
        _camera.orthographicSize += Mathf.Clamp(_targetSize - _camera.orthographicSize, -maxZoomDelta, maxZoomDelta);
        
        var cameraDelta = Vector2.ClampMagnitude(_target - (Vector2)transform.position, CameraSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x + cameraDelta.x, transform.position.y + cameraDelta.y, -10);
    }
}
