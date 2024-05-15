using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class Player : MonoBehaviour
{
    private StarterAssetsInputs _input;
    private FirstPersonController _fpsController;

    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Animator _animator;
    [SerializeField] private Animator _playerAnimator;
    [SerializeField] private GameObject[] _bloodScreen;

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _audioClip;

    [Header("Head Bob")]

    [SerializeField] private Transform _headTransform;
    [SerializeField] private float _headBobAmount;
    [SerializeField] private float _headBobSpeed;
    private Vector3 _initialHeadPosition;
    private float _timer = 0f;

    [Space]

    private float _sprintRefuelSpeed;
    private float _sprintRemaining;
    private bool _canSprint = true;

    [Header("Damage")]

    [SerializeField] private int _health = 100;

    [SerializeField] private int _headShot = 25;
    [SerializeField] private int _bodyShot = 10;
    private int _playerScore;

    [Header("Weapon Characteristics")]

    [SerializeField] private float _fireRate = 0.2f;
    [SerializeField] private int _firingDistance = 65;

    [SerializeField] private int _ammo = 30;
    [SerializeField] private int _ammoSubCount = 60;

    [SerializeField] private GameObject _muzzleFlashTransform;
    [SerializeField] private RectTransform _reticleTransform;
    [SerializeField] private Weapon _weapon;

    private int _maxAmmo = 30;
    private int _killCount;
    private float _canFire;

    private bool _ammoRemaining = true;
    private bool _canReload = true;
    private bool _isReloading;
    private bool _isEngagingInDialogue;
    private bool _clipEmpty;
    private bool _barrelDestroyed;
    private bool _playerIsAlive = true;
    private bool _canTakeDamage = true;

    private void Start()
    {
        GameObject playerCapsule = GameObject.Find("PlayerCapsule");
        if (playerCapsule == null)
            Debug.LogError("playerCapsule is NULL");

        _fpsController = playerCapsule.GetComponent<FirstPersonController>();
        if (_fpsController == null)
            Debug.LogError("FPS Controller is NULL");

        _input = playerCapsule.GetComponent<StarterAssetsInputs>();
        if (_input == null)
            Debug.LogError("Input is NULL");

        _weapon.GetComponent<Weapon>();
        if (_weapon == null)
            Debug.LogError("Weapon is NULL");

        for (int i = 0; i < _bloodScreen.Length; i++)
        {
            _bloodScreen[i].SetActive(false);
        }

        _initialHeadPosition = _headTransform.localPosition;

        _sprintRemaining = 100f;
        _input.enabled = true;
        _playerAnimator.applyRootMotion = true;
    }

    private void Update()
    {
        if (_playerIsAlive)
        {
            if (_input.enabled == true && !_input.IsMenuOnScreen() && !_isEngagingInDialogue)
            {
                Shoot();
            }

            Reload();
            Sprint();
        }
    }

    private void Reload()
    {
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

    private void Sprint()
    {
        bool isMoving = (_input.move != Vector2.zero);

        if (isMoving)
        {
            float bobSpeed;
            float verticalBob;

            if (_canSprint && _input.sprint)
            {
                GameManager.Instance.IncreaseChromaticAberration(0.4f, 4f);
                bobSpeed = _headBobSpeed * 2f;

                SprintSliderDecrease(30);
            }
            else
            {
                GameManager.Instance.IncreaseChromaticAberration(0f, 6f);
                bobSpeed = _headBobSpeed * 1.25f;
                _input.sprint = false;
                SprintSliderIncrease(15);
            }

            verticalBob = Mathf.Cos(_timer * bobSpeed) * _headBobAmount * 0.5f;
            Vector3 headPosition = _initialHeadPosition + Vector3.up * verticalBob;
            _headTransform.localPosition = headPosition;

            _timer += Time.deltaTime;
        }
        else
        {
            GameManager.Instance.IncreaseChromaticAberration(0f, 8f);
            _headTransform.localPosition = _initialHeadPosition;
            _timer = 0;

            SprintSliderIncrease(20);
        }
    }

    private void SprintSliderIncrease(float speed)
    {
        _sprintRefuelSpeed = speed;

        UIManager.Instance.SprintSlider(_sprintRemaining += Time.deltaTime * _sprintRefuelSpeed);
        if (_sprintRemaining >= 100)
        {
            _sprintRemaining = 100;
        }
    }
    private void SprintSliderDecrease(float speed)
    {
        _sprintRefuelSpeed = speed;

        UIManager.Instance.SprintSlider(_sprintRemaining -= Time.deltaTime * _sprintRefuelSpeed);
        if (_sprintRemaining <= 0)
        {
            _sprintRemaining = 0;
            _canSprint = false;
            StartCoroutine(SprintCooldownRoutine());
        }
    }

    IEnumerator SprintCooldownRoutine()
    {
        yield return new WaitForSeconds(1);
        _canSprint = true;
    }

    private void Shoot()
    {
        if (_ammoRemaining && _input.fire && Time.time > _canFire)
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

        else if (_clipEmpty && _input.fire && Time.time > _canFire && !_isReloading)
        {
            PlaySFX(3); // empty clip
            _canFire = Time.time + 1f;
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

        yield return new WaitForSeconds(0.2f);
        PlaySFX(4); //reload audio
        yield return new WaitForSeconds(1.3f);
        PlaySFX(5); // reload audio
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
            else
            {
                _canReload = false;
            }
        }
        else
        {
            _canReload = false;
        }
        _animator.SetBool("Reload", false);

        _isReloading = false;

        UIManager.Instance.AmmoSubCount(_ammoSubCount);
    }

    private void Fire()
    {
        Ray rayOrigin = Camera.main.ScreenPointToRay(_reticleTransform.position);
        RaycastHit hit;

        bool hitEnemy = false; // prevents shooting through enemy to hit barrel

        if (Physics.Raycast(rayOrigin, out hit, _firingDistance))
        {
            if (hit.collider.tag == "Enemy")
            {
                hitEnemy = true;

                if (hit.collider.gameObject.name == "Head Collider") // headshot damage
                {
                    hit.collider.GetComponentInParent<EnemyAI>().SendMessage("EnemyDamage", _headShot, SendMessageOptions.DontRequireReceiver);
                }
                if (hit.collider.gameObject.name == "Body Collider") // bodyshot damage
                {
                    hit.collider.GetComponentInParent<EnemyAI>().SendMessage("EnemyDamage", _bodyShot, SendMessageOptions.DontRequireReceiver);
                }
                GameObject blood = PoolManager.Instance.RequestBlood(); //instantiate blood when hitting enemy
                blood.transform.position = hit.point + new Vector3(0, 0, 0.05f);
                blood.transform.rotation = Quaternion.LookRotation(hit.normal);
            }
        }

        if (!hitEnemy && Physics.Raycast(rayOrigin, out hit, _firingDistance, _layerMask))
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
        PlaySFX(randomGunAudio);
    }

    IEnumerator BarrelDestroyedTimer()
    {
        yield return new WaitForSeconds(3f);
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
        if (_canTakeDamage && _playerIsAlive)
        {
            _health -= health;

            int randomSFX = Random.Range(6, _audioClip.Length);
            PlaySFX(randomSFX); //player hurt sfx

            if (_bloodScreen != null)
            {
                int random = Random.Range(0, _bloodScreen.Length);

                while (_bloodScreen[random].activeInHierarchy)
                {
                    Debug.Log("Reroll. Value = " + random);
                    random = Random.Range(0, _bloodScreen.Length);
                }
                _bloodScreen[random].SetActive(true);
            }

            if (_health <= 0)
            {
                _health = 0;
                IsPlayerAlive(false);
                UIManager.Instance.IsPlayerAlive(false);
            }
        }

        UIManager.Instance.HealthRemaining(_health);
    }

    public void IsPlayerAlive(bool isPlayerAlive)
    {
        _playerIsAlive = isPlayerAlive;
        _playerAnimator.applyRootMotion = false;
        _playerAnimator.SetBool("Death", true);
        _fpsController.IsPlayerAlive(false);
        _input.IsPlayerAlive(false);
        _input.CanPlayerMove(false);
    }

    public void AmmoPickup(int ammoQuantity)
    {
        _ammoSubCount += ammoQuantity;

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

        if (_bloodScreen != null)
        {
            for (int i = 0; i < _bloodScreen.Length; i++)
            {
                _bloodScreen[i].SetActive(false);
            }
        }
    }

    private void PlaySFX(int audioclip)
    {
        AudioManager.Instance.PlaySFX(_audioSource, _audioClip[audioclip]);
    }

    public void isEngagingInDialogue(bool value)
    {
        _isEngagingInDialogue = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "End Game Trigger")
        {
            _canTakeDamage = false;
            _input.enabled = false;
            UIManager.Instance.CanEndGame(true);
            GameManager.Instance.YouWinScreen();
        }
    }
}
