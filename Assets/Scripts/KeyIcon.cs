using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyIcon : MonoBehaviour
{
    [SerializeField] private Image _background;

    public void TogglePressed(bool pressed)
    {
        _background.color = pressed ? Color.white : Color.gray;
    }
}
