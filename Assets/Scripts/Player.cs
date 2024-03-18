using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject _bullet;
    [SerializeField] private GameObject _bulletSpawnPos;

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            GameObject bullet = PoolManager.Instance.RequestBullet();
            bullet.transform.position = _bulletSpawnPos.transform.position; //Where to spawn bullet
            bullet.transform.rotation = _bulletSpawnPos.transform.rotation; 
        }
    }
}
