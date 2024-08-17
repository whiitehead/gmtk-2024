using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Tweakables")]
    [SerializeField] private float _limbGrowthRate = 0.05f;
    [SerializeField] private int _maxLimbs = 4;

    [Header("References")]
    [SerializeField] private GameObject _limbPrefab;
    [SerializeField] private Transform _limbsParent;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Transform _directionArrow; //TODO: replace with eyes
    
    private Dictionary<KeyCode, Limb> _limbMap = new Dictionary<KeyCode, Limb>();
    private KeyCode[] _limbKeys = {KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F};

    private bool _isExtending = false;
    private bool _isRetracting = false;
    
    private void Awake()
    {
        for(int i = 0; i < _maxLimbs; i++)
        {
            GameObject limbGameObject = Instantiate(_limbPrefab);
            limbGameObject.transform.SetParent(_limbsParent);

            Limb newLimb = limbGameObject.GetComponent<Limb>();
            newLimb.ConnectToBody(_rigidbody);

            _limbMap.Add(_limbKeys[i], newLimb);
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
        if (Input.GetMouseButtonDown(0))
        {
            _isExtending = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isExtending = false;
        }

        if (Input.GetMouseButtonDown(1))
        {
            _isRetracting = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            _isRetracting = false;
        }

        if (_isExtending)
        {
            foreach (KeyCode limbKey in _limbKeys)
            {
                if (Input.GetKey(limbKey))
                {
                    _limbMap[limbKey].PrepareToExtend(this.transform);
                    _limbMap[limbKey].ExtendLimb(_limbGrowthRate);
                }
            }
        }

        if (_isRetracting)
        {
            foreach (KeyCode limbKey in _limbKeys)
            {
                if (Input.GetKey(limbKey))
                {
                    _limbMap[limbKey].gameObject.SetActive(false);
                    _limbMap[limbKey].RetractLimb();
                }
            }
        }
    }
}
