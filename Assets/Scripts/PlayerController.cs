using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [Header("Tweakables")]
    [SerializeField] private int _growingUnitsMaximum = 10000;
    [SerializeField] private float _limbGrowthRate = 0.05f;
    [SerializeField] private float _limbRetractRate = 1f;
    [SerializeField] private float _bodySizeMaximum = 0.25f;
    [SerializeField] private float _bodySizeMinimum = 0.1f;

    [Header("References")]
    [SerializeField] private GameObject _limbPrefab;
    [SerializeField] private Transform _limbsParent;
    [SerializeField] private Transform _bodyScale;
    
    private Dictionary<KeyCode, Limb> _limbMap = new Dictionary<KeyCode, Limb>();

    [HideInInspector] public KeyCode[] AllLimbKeys = null;

    private bool _forceRetractAllLimbs = false;
    private bool _retractButtonPressed = false;

    private int _growingUnitsCurrent = 0;

    private float _bodyScaleGrowthRate;
    
    private void Awake()
    {
        GenerateAllLimbKeys();

        for(int i = 0, count = AllLimbKeys.Length; i < count; i++)
        {
            if(_limbMap.ContainsKey(AllLimbKeys[i]))
            {
                continue;
            }

            GameObject limbGameObject = Instantiate(_limbPrefab);
            limbGameObject.transform.SetParent(_limbsParent, false);

            Limb newLimb = limbGameObject.GetComponent<Limb>();

            _limbMap.Add(AllLimbKeys[i], newLimb);
            limbGameObject.SetActive(false);
        }

        _bodyScaleGrowthRate = (_bodySizeMaximum - _bodySizeMinimum) / _growingUnitsMaximum;
        _growingUnitsCurrent = _growingUnitsMaximum;
    }

    private void GenerateAllLimbKeys()
    {
        if(AllLimbKeys != null && AllLimbKeys.Length != 0)
        {
            return;
        }

        Array allKeys = Enum.GetValues(typeof(KeyCode));
        List<KeyCode> tempList = new List<KeyCode>();

        foreach(KeyCode keyCode in allKeys)
        {
            // Ignore all Mouse and Joystick inputs
            if(keyCode >= KeyCode.Mouse0)
            {
                break;
            }

            // Ignore the Windows keys
            if(keyCode == KeyCode.LeftWindows || keyCode == KeyCode.RightWindows
             || keyCode == KeyCode.LeftMeta || keyCode == KeyCode.RightMeta)
            {
                break;
            }

            tempList.Add(keyCode);
        }
        AllLimbKeys = tempList.ToArray();
    }

    private void Update()
    {
        HandleButtonInputs();
    }

    private void ChangeBodyScale(float delta)
    {
        float oldScale = _bodyScale.transform.localScale.x;
        float newScale = Mathf.Clamp(oldScale + delta, _bodySizeMinimum, _bodySizeMaximum);
        _bodyScale.transform.localScale = new Vector3(newScale, newScale);
    }


/// <summary>
/// Press and hold keyboard keys to grow individual limbs
/// Press keys again to continue growing limbs
/// Press Mouse Left Click and keyboard keys to instantly retract individual limbs 
/// Press Mouse Right Click to force-retract all Limbs
/// </summary>
    private void HandleButtonInputs()
    {
        // Left Click
        if(Input.GetMouseButtonDown(0))
        {
            _retractButtonPressed = true;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            _retractButtonPressed = false;
        }

        // Right Click
        if(Input.GetMouseButtonDown(1))
        {
            _forceRetractAllLimbs = true;
        }
        else if(Input.GetMouseButtonUp(1))
        {
            _forceRetractAllLimbs = false;
        }

        // Note that this checks all 100+ keyboard KeyCodes every Update.
        // It's okay, computers are fast
        foreach (KeyCode limbKey in AllLimbKeys)
        {
            Limb currentLimb = _limbMap[limbKey];
            if(_forceRetractAllLimbs)
            {
                currentLimb.StartRetracting();
                currentLimb.AdjustLimbLength(-1 * _limbRetractRate);
                _growingUnitsCurrent = _growingUnitsMaximum;
                ChangeBodyScale(_bodySizeMaximum);
                continue;
            }

            if (Input.GetKeyDown(limbKey))
            {
                if(_retractButtonPressed)
                {
                    currentLimb.StartRetracting();
                }
                else if( _growingUnitsCurrent > 0 && (currentLimb.IsExtended || currentLimb.IsRetracted))
                {
                    currentLimb.StartExtending(this.transform);
                }

                currentLimb.ToggleHighlight(true);
            }
            else if(Input.GetKeyUp(limbKey))
            {
                currentLimb.ToggleHighlight(false);
                if(currentLimb.IsExtending)
                {
                    currentLimb.StopExtending();
                }
            }

            if(currentLimb.IsExtending && _growingUnitsCurrent > 0)
            {
                currentLimb.AdjustLimbLength(_limbGrowthRate);
                currentLimb.GrowingUnits++;
                _growingUnitsCurrent--;
                ChangeBodyScale(-1 * _bodyScaleGrowthRate);
            }
            else if(currentLimb.IsRetracting)
            {
                currentLimb.AdjustLimbLength(-1 * _limbRetractRate);
                _growingUnitsCurrent += currentLimb.GrowingUnits;
                ChangeBodyScale(currentLimb.GrowingUnits * _bodyScaleGrowthRate);
                currentLimb.GrowingUnits = 0;
            }
        }
    }
}
