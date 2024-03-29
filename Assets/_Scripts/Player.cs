using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class Player : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private GameObject _muzzleFlashTransform;

    [SerializeField] private int _firingDistance = 65;
    [SerializeField] private int _headShot = 25;
    [SerializeField] private int _bodyShot = 10;
    [SerializeField] private int _ammoCount;
    [SerializeField] private int _playerScore;



    [SerializeField] private float _fireRate = 0.5f;
    private float _canFire;

    private bool _barrelDestroyed;
    private bool _isFiring;

    private StarterAssetsInputs _input;

    private void Start()
    {
        _input = GameObject.Find("PlayerCapsule").GetComponent<StarterAssetsInputs>();
        if (_input == null)
        {
            Debug.LogError("Input is NULL");
        }
    }

    private void Update()
    {
        if (_input.fire)
        {
            if (Time.time > _canFire)
            {
                Fire();
                _canFire = Time.time + _fireRate;
            }
        }
    }

    private void Fire()
    {
        Ray rayOrigin = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, out hit, _firingDistance))
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
        }

        if (Physics.Raycast(rayOrigin, out hit, _firingDistance, _layerMask))
        {
            if (_barrelDestroyed == false)
            {
                StartCoroutine(BarrelDestroyedTimer());
                hit.collider.SendMessage("DestroyBarrel", SendMessageOptions.DontRequireReceiver);
                _barrelDestroyed = true;
            }
        }

        GameObject muzzleFlash = PoolManager.Instance.RequestMuzzleFlash(); //Muzzle Flash from gun
        muzzleFlash.transform.position = _muzzleFlashTransform.transform.position;
        muzzleFlash.transform.rotation = _muzzleFlashTransform.transform.rotation;
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
