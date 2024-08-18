using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [Header("Tweakables")]
    [SerializeField] private float _limbGrowthRate = 0.05f;
    [SerializeField] private float _limbRetractRate = 1f;

    [Header("References")]
    [SerializeField] private GameObject _limbPrefab;
    [SerializeField] private Transform _limbsParent;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Transform _directionArrow; //TODO: replace with eyes
    [SerializeField] private ObjectPool _limbPool;
    
    private Dictionary<KeyCode, Limb> _limbMap = new Dictionary<KeyCode, Limb>();

    [HideInInspector] public KeyCode[] AllLimbKeys = null;
    //public static readonly KeyCode[] LIMB_KEYS = {KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F};

    private bool _forceRetractAllLimbs = false;
    private bool _retractButtonPressed = false;
    
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
            limbGameObject.transform.SetParent(_limbsParent);

            Limb newLimb = limbGameObject.GetComponent<Limb>();

            _limbMap.Add(AllLimbKeys[i], newLimb);
            limbGameObject.SetActive(false);
        }
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
            if(keyCode >= KeyCode.Mouse0)
            {
                break;
            }

            tempList.Add(keyCode);
        }
        AllLimbKeys = tempList.ToArray();
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
/// Press Mouse Right Click to force-retract all Limbs
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
                continue;
            }

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
                currentLimb.ToggleHighlight(false);
                if(currentLimb.IsExtending)
                {
                    currentLimb.StopExtending();
                }
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
