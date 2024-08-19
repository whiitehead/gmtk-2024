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

    // Exposed to Inspector for debugging
    [SerializeField] private int _growingUnitsCurrent = 0;

    private float _bodyScaleGrowthRate;

    private List<Limb> _limbsToExtend = new List<Limb>();
    private List<Limb> _limbsToRetract = new List<Limb>();
    
    private void Awake()
    {
        GenerateAllLimbKeys();
        InstantiateLimbs();

        AdjustGrowingUnitsMaximum(0);
        _growingUnitsCurrent = _growingUnitsMaximum;
    }

    private void InstantiateLimbs()
    {
        for(int i = 0, count = AllLimbKeys.Length; i < count; i++)
        {
            if(_limbMap.ContainsKey(AllLimbKeys[i]))
            {
                continue;
            }

            GameObject limbGameObject = Instantiate(_limbPrefab);
            limbGameObject.transform.SetParent(_limbsParent, false);

            Limb newLimb = limbGameObject.GetComponent<Limb>();
            newLimb.Player = this;

            _limbMap.Add(AllLimbKeys[i], newLimb);
            limbGameObject.SetActive(false);
        }
    }

    private void AdjustGrowingUnitsMaximum(int additionalUnits)
    {
        _growingUnitsMaximum += additionalUnits;
        _bodyScaleGrowthRate = (_bodySizeMaximum - _bodySizeMinimum) / _growingUnitsMaximum;
    }
    
    private void ChangeBodyScale(int growingUnitsDelta)
    {
        _growingUnitsCurrent = Mathf.Clamp(_growingUnitsCurrent + growingUnitsDelta, 0, _growingUnitsMaximum);
        
        float oldScaleAmount = _bodyScale.transform.localScale.x;
        float newScaleDelta = _bodyScaleGrowthRate * growingUnitsDelta;
        float newScaleAmount = Mathf.Clamp(oldScaleAmount + newScaleDelta, _bodySizeMinimum, _bodySizeMaximum);
        _bodyScale.transform.localScale = new Vector3(newScaleAmount, newScaleAmount);
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
                continue;
            }

            // Ignore the Windows keys
            if(keyCode == KeyCode.LeftWindows || keyCode == KeyCode.RightWindows
             || keyCode == KeyCode.LeftMeta || keyCode == KeyCode.RightMeta)
            {
                continue;
            }

            tempList.Add(keyCode);
        }
        AllLimbKeys = tempList.ToArray();
    }

// can run once, zero, or several times per frame, depending on how many physics frames per second are set in the time settings, and how fast/slow the framerate is.
    private void FixedUpdate()
    {
        if(_growingUnitsCurrent > 0)
        {
            foreach(Limb extendingLimb in _limbsToExtend)
            {
                extendingLimb.AdjustLimbLength(_limbGrowthRate);
                extendingLimb.GrowingUnits++;
                ChangeBodyScale(-1);
            }
        }

        for (int i = _limbsToRetract.Count - 1; i >= 0; i--)
        {
            Limb retractingLimb = _limbsToRetract[i];
            retractingLimb.AdjustLimbLength(-1 * _limbRetractRate);
            
            if(retractingLimb.GrowingUnits > 0)
            {
                ChangeBodyScale(retractingLimb.GrowingUnits);
                retractingLimb.GrowingUnits = 0;
            }

            if(retractingLimb.IsRetracted)
            {
                _limbsToRetract.RemoveAt(i);
            }
        }
    }

//Called EVERY frame
    private void Update()
    {
        HandleButtonInputs();
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
        _retractButtonPressed = Input.GetMouseButton(0);

        // Right Click
        if(Input.GetMouseButtonDown(1))
        {
            _forceRetractAllLimbs = true;
            _retractButtonPressed = false;
        }

        // Note that this checks all 100+ keyboard KeyCodes every Update.
        // It's okay, computers are fast
        foreach (KeyCode limbKey in AllLimbKeys)
        {
            Limb currentLimb = _limbMap[limbKey];
            if(_forceRetractAllLimbs)
            {
                if (!currentLimb.IsRetracting)
                {
                    currentLimb.StartRetracting();
                    _limbsToRetract.Add(currentLimb);
                }
                
                _limbsToExtend.Clear();
                currentLimb.GrowingUnits = 0;
                ChangeBodyScale(_growingUnitsMaximum);
                continue;
            }

            if (Input.GetKeyDown(limbKey))
            {
                currentLimb.ToggleHighlight(true);

                if(_retractButtonPressed)
                {
                    currentLimb.StartRetracting();
                    _limbsToRetract.Add(currentLimb);
                }
                else if( _growingUnitsCurrent > 0 && (currentLimb.IsExtended || currentLimb.IsRetracted))
                {
                    currentLimb.StartExtending(this.transform);
                    _limbsToExtend.Add(currentLimb);
                }
            }
            else if(Input.GetKeyUp(limbKey))
            {
                currentLimb.ToggleHighlight(false);

                if(currentLimb.IsExtending)
                {
                    currentLimb.StopExtending();
                    _limbsToExtend.Remove(currentLimb);
                }
            }
        }
        
        if (_forceRetractAllLimbs)
        {
            _forceRetractAllLimbs = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col == null)
        {
            return;
        }
        
        PowerUp powerUp = col.gameObject.GetComponent<PowerUp>();
        if (powerUp != null)
        {
            AdjustGrowingUnitsMaximum(powerUp.BodyUnitsRewarded);
            ChangeBodyScale(powerUp.BodyUnitsRewarded);
            Destroy(powerUp.gameObject);
        }
        
        Hazard touchedHazard = col.gameObject.GetComponent<Hazard>();
        if (touchedHazard != null)
        {
            //Collider2D myCollider = col.GetContact
        }
    }

    public void MarkLimbAsHurt(Limb hurtLimb)
    {
        hurtLimb.StartRetracting();
        _limbsToExtend.Remove(hurtLimb);
        _limbsToRetract.Add(hurtLimb);
    }
}
