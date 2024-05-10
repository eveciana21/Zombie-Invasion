using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoSingleton<CameraManager>
{
    [SerializeField] private GameObject[] _camera;
    public override void Init()
    {
        base.Init(); //Turns this class into a singleton
    }

    public void SetMasterCam(int camSelected)
    {
        foreach (var cam in _camera)
        {
            cam.GetComponent<CinemachineVirtualCamera>().Priority = 10;
        }

        _camera[camSelected].GetComponent<CinemachineVirtualCamera>().Priority = 15;
    }
}
