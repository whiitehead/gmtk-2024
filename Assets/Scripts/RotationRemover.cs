using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationRemover : MonoBehaviour
{
    void Update() => transform.rotation = Quaternion.identity;
}
