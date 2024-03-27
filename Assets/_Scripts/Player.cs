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

    [SerializeField] private GameObject _barrel, _explosion;

    private int _headShot = 25;
    private int _bodyShot = 10;

    [SerializeField] private int _playerScore;
    private bool _barrelDestroyed;

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray rayOrigin = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(rayOrigin, out hit, 65f))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    if (hit.collider.gameObject.name == "Head Collider") // headshot damage
                    {
                        hit.collider.GetComponentInParent<EnemyAI>().SendMessage("EnemyDeath", _headShot, SendMessageOptions.DontRequireReceiver);
                    }
                    if (hit.collider.gameObject.name == "Body Collider") // bodyshot damage
                    {
                        hit.collider.GetComponentInParent<EnemyAI>().SendMessage("EnemyDeath", _bodyShot, SendMessageOptions.DontRequireReceiver);
                    }
                    GameObject blood = PoolManager.Instance.RequestBlood(); //instantiate blood when hitting enemy
                    blood.transform.position = hit.point + new Vector3(0, 0, 0.05f);
                    blood.transform.rotation = Quaternion.LookRotation(hit.normal);
                }
                if (hit.collider.CompareTag("Barrel")) // destroy explosive barrel
                {
                    if (_barrelDestroyed == false)
                    {
                        StartCoroutine(BarrelDestroyedTimer());
                        hit.collider.SendMessage("DestroyBarrel", SendMessageOptions.DontRequireReceiver);
                        _barrelDestroyed = true;
                    }
                }
            }

            GameObject muzzleFlash = PoolManager.Instance.RequestMuzzleFlash(); //Muzzle Flash from gun
            muzzleFlash.transform.position = _muzzleFlashTransform.transform.position;
            muzzleFlash.transform.rotation = _muzzleFlashTransform.transform.rotation;
        }
    }

    IEnumerator BarrelDestroyedTimer()
    {
        yield return new WaitForSeconds(5f);
        _barrelDestroyed = false;
    }

    public void AddToScore(int playerScore)
    {
        _playerScore += playerScore;
    }
}
