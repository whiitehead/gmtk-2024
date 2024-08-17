using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Tweakables")]
    [SerializeField] private float _limbGrowthRate = 0.05f;
    private const int _maxLimbs = 4; //TODO: Make this serialized again once pooling works

    [Header("References")]
    [SerializeField] private GameObject _limbPrefab;
    [SerializeField] private Transform _limbsParent;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Transform _directionArrow; //TODO: replace with eyes
    
    private Dictionary<KeyCode, Limb> _limbMap = new Dictionary<KeyCode, Limb>();
    public static readonly KeyCode[] LIMB_KEYS = {KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F};

    private bool _isExtending = false;
    private bool _isRetracting = false;
    
    private void Awake()
    {
        for(int i = 0; i < _maxLimbs; i++)
        {
            GameObject limbGameObject = Instantiate(_limbPrefab);
            limbGameObject.transform.SetParent(_limbsParent);

            Limb newLimb = limbGameObject.GetComponent<Limb>();

            _limbMap.Add(LIMB_KEYS[i], newLimb);
            limbGameObject.SetActive(false);
        }
    }

    private void Update()
    {
        PointArrowToMouse();

        HandleButtonInputs();
    }

    private void PointArrowToMouse()
    {
        _directionArrow.rotation = GameUtils.GetAngleToMouse(_directionArrow);
    }

    private void HandleButtonInputs()
    {
        foreach (KeyCode limbKey in LIMB_KEYS)
        {
            Limb currentLimb = _limbMap[limbKey];

            if (Input.GetKeyDown(limbKey))
            {
                if(currentLimb.IsExtended)
                {
                    currentLimb.StartRetracting();
                }
                else if(currentLimb.IsRetracted)
                {
                    currentLimb.StartExtending(this.transform);
                }
            }

            if(currentLimb.IsExtending)
            {
                currentLimb.ExtendLimb(_limbGrowthRate);
            }

            if(Input.GetKeyUp(limbKey))
            {
                if(currentLimb.IsExtending)
                {
                    currentLimb.StopExtending();
                }
            }
        }
    }
}
