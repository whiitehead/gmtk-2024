using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{

    [SerializeField] private KeyIcon[] _keyIcons = new KeyIcon[4];

    void Start()
    {
        foreach(KeyIcon keyIcon in _keyIcons)
        {
            keyIcon.TogglePressed(false);
        }
    }

    void Update()
    {
        // for(int i = 0; i < 4; i++)
        // {
        //     bool pressed = Input.GetKey(PlayerController.LIMB_KEYS[i]);
        //     _keyIcons[i].TogglePressed(pressed);
        // }
    }
}
