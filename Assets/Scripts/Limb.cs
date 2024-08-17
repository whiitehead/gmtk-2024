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
    [SerializeField] private Transform _limbExtension;

    private LimbState _limbState = LimbState.RETRACTED;

    public bool IsRetracted => _limbState == LimbState.RETRACTED;
    public bool IsExtending => _limbState == LimbState.EXTENDING;
    public bool IsExtended => _limbState == LimbState.EXTENDED;
    public bool IsRetracting => _limbState == LimbState.RETRACTING;

    private void Awake()
    {
        StartRetracting();
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

    public void ExtendLimb(float delta)
    {
        float newLength = _limbExtension.localScale.y + delta;
        _limbExtension.localScale = new Vector3(_limbExtension.localScale.x, newLength, _limbExtension.localScale.z);
    }

    public void StopExtending()
    {
        _limbState = LimbState.EXTENDED;
    }

    public void StartRetracting()
    {
        _limbExtension.localScale = new Vector3(_limbExtension.localScale.x, 0f, _limbExtension.localScale.z);
        _limbState = LimbState.RETRACTED; //TODO: make the limb visual retract
    }

    public void StopRetracting()
    {
        _limbState = LimbState.RETRACTED;
    }
}
