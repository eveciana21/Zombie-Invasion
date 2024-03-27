using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject _bullet;
    [SerializeField] private GameObject _bulletSpawnPos;
    [SerializeField] private GameObject _muzzleFlashTransform;
    [SerializeField] private GameObject _bloodSplatter;

    private int _headShot = 25;
    private int _bodyShot = 10;

    [SerializeField] private int _playerScore;

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray rayOrigin = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(rayOrigin, out hit, 65f))
            {
                GameObject muzzleFlash = PoolManager.Instance.RequestMuzzleFlash();
                muzzleFlash.transform.position = _muzzleFlashTransform.transform.position;
                muzzleFlash.transform.rotation = _muzzleFlashTransform.transform.rotation;

                if (hit.collider.CompareTag("Enemy"))
                {
                    if (hit.collider.gameObject.name == "Head Collider")
                    {
                        hit.collider.GetComponentInParent<EnemyAI>().SendMessage("EnemyDeath", _headShot, SendMessageOptions.DontRequireReceiver);
                    }
                    if (hit.collider.gameObject.name == "Body Collider")
                    {
                        hit.collider.GetComponentInParent<EnemyAI>().SendMessage("EnemyDeath", _bodyShot, SendMessageOptions.DontRequireReceiver);
                    }
                    GameObject blood = PoolManager.Instance.RequestBlood();
                    blood.transform.position = hit.point + new Vector3(0, 0, 0.05f);
                    blood.transform.rotation = Quaternion.LookRotation(hit.normal);
                }
            }
            else
            {
                GameObject muzzleFlash = PoolManager.Instance.RequestMuzzleFlash();
                muzzleFlash.transform.position = _muzzleFlashTransform.transform.position;
                muzzleFlash.transform.rotation = _muzzleFlashTransform.transform.rotation;
            }
        }
    }

    public void AddToScore(int playerScore)
    {
        _playerScore += playerScore;
    }
}
