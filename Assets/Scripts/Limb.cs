using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limb : MonoBehaviour
{
    [SerializeField] private Transform _limbExtension;
    //[SerializeField] private FixedJoint2D _fixedJoint;

    private bool _preparedToExtend = false;

    private void Awake()
    {
        RetractLimb();
    }

    public void ConnectToBody(Rigidbody2D mainBody)
    {
        //_fixedJoint.connectedBody = mainBody;
    }

    public void PrepareToExtend(Transform directionArrow)
    {
        if(_preparedToExtend)
        {
            return;
        }

        _preparedToExtend = true;

        gameObject.transform.rotation = GameUtils.GetAngleToMouse(directionArrow);
        gameObject.SetActive(true);
    }

    public void ExtendLimb(float delta)
    {
        float newLength = _limbExtension.localScale.y + delta;
        _limbExtension.localScale = new Vector3(_limbExtension.localScale.x, newLength, _limbExtension.localScale.z);
    }

    public void RetractLimb()
    {
        _limbExtension.localScale = new Vector3(_limbExtension.localScale.x, 0f, _limbExtension.localScale.z);
        _preparedToExtend = false;
    }
}
