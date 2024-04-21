using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class Player : MonoBehaviour
{
    private StarterAssetsInputs _input;
    [SerializeField] private Animator _animator;

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

    [SerializeField] private bool _ammoRemaining = true;
    [SerializeField] private bool _canReload = true;
    private bool _isReloading;
    private bool _isEngagingInDialogue;

    private float _canFire;
    [SerializeField] private bool _clipEmpty;

    private int _killCount;

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
        if (!_isEngagingInDialogue)
        {
            Shoot();
        }

        if (_input.reload)
        {
            if (_ammoSubCount <= 0)
            {
                _canReload = false;
            }
            else
            {
                if (_ammo < _maxAmmo)
                {
                    _canReload = true;
                }
            }

            if (_canReload && !_isReloading)
            {
                if (_ammo < _maxAmmo)
                {
                    StartCoroutine(ReloadAmmo());
                }
            }
            _input.reload = false;
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
                    if (!_isReloading && _canReload && _ammoRemaining)
                    {
                        StartCoroutine(ReloadAmmo());
                        _canReload = false;
                    }
                    else
                    {
                        _ammoRemaining = false;
                        _clipEmpty = true;
                    }
                }
                _canFire = Time.time + _fireRate;
            }
        }
        else
        {
            if (_clipEmpty && _input.fire)
            {
                if (Time.time > _canFire && !_isReloading)
                {
                    AudioManager.Instance.PlaySFX(3);
                    _canFire = Time.time + 1f;
                }
            }
        }
        UIManager.Instance.AmmoCount(_ammo);
    }

    IEnumerator ReloadAmmo()
    {
        _isReloading = true;

        if (_ammoRemaining)
        {
            _animator.SetBool("Reload", true);
        }
        _ammoRemaining = false;

        AudioManager.Instance.PlaySFX(4);
        yield return new WaitForSeconds(1.3f);
        AudioManager.Instance.PlaySFX(5);
        yield return new WaitForSeconds(0.7f);

        int spaceLeftInChamber = _maxAmmo - _ammo;
        if (spaceLeftInChamber > 0 && _ammoSubCount > 0)
        {
            int bulletsToReload = Mathf.Min(spaceLeftInChamber, _ammoSubCount);
            _ammo += bulletsToReload;
            _ammoSubCount -= bulletsToReload;

            _ammoRemaining = true;
            _clipEmpty = false;

            if (_ammoSubCount > 0)
            {
                _canReload = true;
            }
        }
        _animator.SetBool("Reload", false);

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

        int randomGunAudio = Random.Range(0, 3);
        AudioManager.Instance.PlaySFX(randomGunAudio);
    }

    IEnumerator BarrelDestroyedTimer()
    {
        yield return new WaitForSeconds(5f);
        _barrelDestroyed = false;
    }

    public void AddToScore(int playerScore)
    {
        _playerScore += playerScore;
        _killCount++;
        UIManager.Instance.Score(_playerScore, _killCount);
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
        _clipEmpty = false;
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

    public void isEngagingInDialogue(bool value)
    {
        _isEngagingInDialogue = value;
    }
}
