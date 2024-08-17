using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Tweakables")]
    [SerializeField] private float _limbGrowthRate = 0.05f;
    [SerializeField] private float _limbRetractRate = 1f;
    private const int _maxLimbs = 4; //TODO: Make this serialized again once pooling works

    [Header("References")]
    [SerializeField] private GameObject _limbPrefab;
    [SerializeField] private Transform _limbsParent;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Transform _directionArrow; //TODO: replace with eyes
    
    private Dictionary<KeyCode, Limb> _limbMap = new Dictionary<KeyCode, Limb>();
    public static readonly KeyCode[] LIMB_KEYS = {KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F};

    private bool _isExtending = false;
    private bool _retractButtonPressed = false;
    
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


/// <summary>
/// Press and hold keyboard keys to grow individual limbs
/// Press keys again to continue growing limbs
/// Press Mouse Left Click and keyboard keys to instantly retract individual limbs 
/// </summary>
    private void HandleButtonInputs()
    {
        if(Input.GetMouseButtonDown(0))
        {
            _retractButtonPressed = true;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            _retractButtonPressed = false;
        }


        foreach (KeyCode limbKey in LIMB_KEYS)
        {
            Limb currentLimb = _limbMap[limbKey];

            if (Input.GetKeyDown(limbKey))
            {
                if(_retractButtonPressed)
                {
                    currentLimb.StartRetracting();
                }
                else if(currentLimb.IsExtended || currentLimb.IsRetracted)
                {
                    currentLimb.StartExtending(this.transform);
                }

                currentLimb.ToggleHighlight(true);
            }
            else if(Input.GetKeyUp(limbKey))
            {
                if(currentLimb.IsExtending)
                {
                    currentLimb.StopExtending();
                }

                currentLimb.ToggleHighlight(false);
            }

            if(currentLimb.IsExtending)
            {
                currentLimb.AdjustLimbLength(_limbGrowthRate);
            }
            else if(currentLimb.IsRetracting)
            {
                currentLimb.AdjustLimbLength(-1 * _limbRetractRate);
            }            
        }
    }
}
