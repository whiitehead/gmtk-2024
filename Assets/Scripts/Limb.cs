using System;
using UnityEngine;


public enum LimbState
{
    RETRACTED,
    EXTENDING,
    EXTENDED,
    RETRACTING,
}

public class Limb : MonoBehaviour
{
    [Header("Tweakables")]
    [SerializeField] private Color _highlightColor = Color.yellow;

    [Header("UI Elements")]
    [SerializeField] private Transform _limbExtension;
    [SerializeField] private SpriteRenderer _limbSprite;

    private Color _originalColor;
    private LimbState _limbState = LimbState.RETRACTED;
    private float _limbSpriteBaseYPosition;
    private float _limbSpriteBaseXScale;

    public bool IsRetracted => _limbState == LimbState.RETRACTED;
    public bool IsExtending => _limbState == LimbState.EXTENDING;
    public bool IsExtended => _limbState == LimbState.EXTENDED;
    public bool IsRetracting => _limbState == LimbState.RETRACTING;

    public int GrowingUnits = 0;

    [HideInInspector] public PlayerController Player;

    private void Awake()
    {
        StopRetracting();
        // _originalColor = _limbSprite.color;
        _limbSpriteBaseYPosition = _limbSprite.transform.localPosition.y;
        _limbSpriteBaseXScale = _limbSprite.transform.localScale.y;

    }

    public void ToggleHighlight(bool enabled)
    {
        // _limbSprite.color = enabled ? _highlightColor : _originalColor;
    }

    private const float LIMB_THRESHOLD = 10.24f; // I think this is cancelled out at some point when the local transform is calculated and the math mess below could be simplified.
    public void AdjustLimbLength(float delta)
    {
        float newLength = _limbExtension.localScale.y + delta;

        if(newLength <= 0)
        {
            newLength = 0;
            StopRetracting();
        }

        if (newLength < LIMB_THRESHOLD)
        {
            _limbSprite.transform.localPosition = new Vector3(_limbSprite.transform.localPosition.x, _limbSpriteBaseYPosition + newLength - 1, 0);
            _limbSprite.transform.localScale = new Vector3(_limbSpriteBaseXScale, _limbSprite.transform.localScale.y, 1);
        }
        else
        {
            _limbSprite.transform.localPosition = new Vector3(_limbSprite.transform.localPosition.x, _limbSpriteBaseYPosition + (newLength + LIMB_THRESHOLD) / 2 - 1, 0);
            _limbSprite.transform.localScale = new Vector3(newLength * _limbSpriteBaseXScale / LIMB_THRESHOLD, _limbSprite.transform.localScale.y, 1);
        }
        _limbExtension.localScale = new Vector3(_limbExtension.localScale.x, newLength, _limbExtension.localScale.z);
    }

    public void StartExtending(Transform directionArrow)
    {
        if(IsRetracted)
        {
            gameObject.transform.rotation = GameUtils.GetAngleToMouse(directionArrow);
            gameObject.SetActive(true);
        }
        
        _limbState = LimbState.EXTENDING;
    }

    public void StopExtending()
    {
        _limbState = LimbState.EXTENDED;
    }

    public void StartRetracting()
    {
        if(_limbState != LimbState.RETRACTED)
        {
            _limbState = LimbState.RETRACTING;
        }
    }

    public void StopRetracting()
    {
        _limbState = LimbState.RETRACTED;
        this.gameObject.SetActive(false); //This will return the Limb to the ObjectPool
    }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other == null)
    //     {
    //         return;
    //     }
    //
    //     Hazard touchedHazard = other.gameObject.GetComponent<Hazard>();
    //     if (touchedHazard != null)
    //     {
    //         Player.MarkLimbAsHurt(this);
    //     }
    // }
}
