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

    public bool IsRetracted => _limbState == LimbState.RETRACTED;
    public bool IsExtending => _limbState == LimbState.EXTENDING;
    public bool IsExtended => _limbState == LimbState.EXTENDED;
    public bool IsRetracting => _limbState == LimbState.RETRACTING;

    private void Awake()
    {
        StopRetracting();
        _originalColor = _limbSprite.color;
    }

    public void ToggleHighlight(bool enabled)
    {
        _limbSprite.color = enabled ? _highlightColor : _originalColor;
    }

    public void AdjustLimbLength(float delta)
    {
        float newLength = _limbExtension.localScale.y + delta;

        if(newLength <= 0)
        {
            newLength = 0;
            StopRetracting();
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
}
