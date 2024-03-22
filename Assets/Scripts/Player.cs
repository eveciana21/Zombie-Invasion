using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject _bullet;
    [SerializeField] private GameObject _bulletSpawnPos;
    [SerializeField] private GameObject _bulletHole;
    [SerializeField] private GameObject _muzzleFlashTransform;

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray rayOrigin = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(rayOrigin, out hit, 50f))
            {
                GameObject muzzleFlash = PoolManager.Instance.RequestMuzzleFlash();
                muzzleFlash.transform.position = _muzzleFlashTransform.transform.position;
                muzzleFlash.transform.rotation = _muzzleFlashTransform.transform.rotation;
                /*GameObject bullet = PoolManager.Instance.RequestBullet();
                bullet.transform.position = _bulletSpawnPos.transform.position; //Where to spawn bullet
                bullet.transform.rotation = _bulletSpawnPos.transform.rotation;*/

                //GameObject bulletHole = Instantiate(_bulletHole, hit.point, Quaternion.LookRotation(hit.normal));
                //Destroy(bulletHole, 2f);
                if (hit.collider.tag == "Enemy")
                {
                    Debug.Log("Hit");
                }
            }
        }
    }


    /*private void FireBullet()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            //Ray rayOrigin = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(_bulletSpawnPos.transform.position, transform.forward, out hit, Mathf.Infinity))
            {
                GameObject bullet = PoolManager.Instance.RequestBullet();
                bullet.transform.position = _bulletSpawnPos.transform.position; //Where to spawn bullet
                bullet.transform.rotation = _bulletSpawnPos.transform.rotation;

                //Instantiate(_bulletHole, hit.point, Quaternion.LookRotation(hit.normal));

                if (hit.collider.tag == "Enemy")
                {
                    Debug.Log("Hit");
                }
            }
        }
    }*/
}
