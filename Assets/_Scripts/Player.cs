using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class Player : MonoBehaviour
{
    private StarterAssetsInputs _input;

    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private GameObject _muzzleFlashTransform;
    [SerializeField] private RectTransform _reticleTransform;
    [SerializeField] private Weapon _weapon;

    [Header("Damage")]

    [SerializeField] private int _health = 100;

    [SerializeField] private int _headShot = 25;
    [SerializeField] private int _bodyShot = 10;
    [SerializeField] private int _playerScore;

    private bool _barrelDestroyed;
    private bool _playerIsAlive = true;

    [Header("Weapon Characteristics")]

    [SerializeField] private float _fireRate = 0.2f;
    [SerializeField] private int _firingDistance = 65;

    [SerializeField] private int _ammo = 30;
    [SerializeField] private int _ammoSubCount = 60;
    private int _maxAmmo = 30;

    private bool _ammoRemaining = true;
    private bool _canReload = true;
    private bool _isReloading;

    private float _canFire;

    [SerializeField] private GameObject[] _bloodScreen;

    private void Start()
    {
        _input = GameObject.Find("PlayerCapsule").GetComponent<StarterAssetsInputs>();
        if (_input == null)
            Debug.LogError("Input is NULL");

        _weapon.GetComponent<Weapon>();
        if (_weapon == null)
            Debug.LogError("Weapon is NULL");

        for (int i = 0; i < _bloodScreen.Length; i++)
        {
            _bloodScreen[i].SetActive(false);
        }
    }

    private void Update()
    {
        Shoot();

        if (Keyboard.current.rKey.isPressed)
        {
            if (_ammoSubCount <= 0)
            {
                _canReload = false;
            }
            else
            {
                _canReload = true;
            }

            if (_canReload && !_isReloading)
            {
                if (_ammo < _maxAmmo)
                {
                    StartCoroutine(ReloadAmmo());
                }
            }
        }
    }

    private void Shoot()
    {
        if (_ammoRemaining && _input.fire)
        {
            if (Time.time > _canFire)
            {
                Fire();
                _weapon.WeaponRecoil();
                _ammo--;

                if (_ammo <= 0)
                {
                    _ammo = 0;
                    if (!_isReloading)
                    {
                        StartCoroutine(ReloadAmmo());
                    }
                }
                _canFire = Time.time + _fireRate;
            }
        }
        UIManager.Instance.AmmoCount(_ammo);
    }

    IEnumerator ReloadAmmo()
    {
        _isReloading = true;

        if (_canReload == true)
        {
            Debug.Log("Reloading!");

            _ammoRemaining = false;

            yield return new WaitForSeconds(2);

            int spaceLeftInChamber = _maxAmmo - _ammo;
            if (spaceLeftInChamber > 0 && _ammoSubCount > 0)
            {
                int bulletsToReload = Mathf.Min(spaceLeftInChamber, _ammoSubCount);
                _ammo += bulletsToReload;
                _ammoSubCount -= bulletsToReload;

                Debug.Log("Reloaded " + bulletsToReload + " bullets.");

                _ammoRemaining = true;
            }
        }
        else
        {
            _ammoRemaining = false;
            Debug.Log("NO AMMO!");
        }

        _isReloading = false;

        UIManager.Instance.AmmoSubCount(_ammoSubCount);
    }

    private void Fire()
    {
        Ray rayOrigin = Camera.main.ScreenPointToRay(_reticleTransform.position);
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, out hit, _firingDistance))
        {
            if (hit.collider.tag == "Enemy")
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
        UIManager.Instance.Score(_playerScore);
    }

    public void DamagePlayer(int health)
    {
        if (_playerIsAlive)
        {
            _health -= health;

            int random = Random.Range(0, _bloodScreen.Length);

            while (_bloodScreen[random].activeInHierarchy)
            {
                Debug.Log("Reroll. Value = " + random);
                random = Random.Range(0, _bloodScreen.Length);
            }
            _bloodScreen[random].SetActive(true);

            if (_health <= 0)
            {
                _health = 0;
                _playerIsAlive = false;
            }
        }
        UIManager.Instance.HealthRemaining(_health);
    }


    public void AmmoPickup()
    {
        _ammoSubCount += 60;

        _canReload = true;
        _ammoRemaining = true;

        if (_ammo <= 0 && !_isReloading)
        {
            StartCoroutine(ReloadAmmo());
        }

        UIManager.Instance.AmmoSubCount(_ammoSubCount);
    }

    public void HealthPickup()
    {
        _health = 100;
        UIManager.Instance.HealthRemaining(_health);

        for (int i = 0; i < _bloodScreen.Length; i++)
        {
            _bloodScreen[i].SetActive(false);
        }
    }
}
