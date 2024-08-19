using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroController : MonoBehaviour
{
    private CameraController _cameraController;
    private PlayerController _playerController;

    [SerializeField] private GameObject _imageParent;
    
    private void Awake()
    {
        _cameraController = FindObjectOfType<CameraController>();
        _playerController = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if(_playerController.IsInIntroState == false)
        {
            _cameraController.EndIntro();
            _imageParent.SetActive(false); //TODO: fade out!

            this.gameObject.SetActive(false);
        }
    }
}
