using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private List<Transform> _wayPoint;
    private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _audioClip;

    [Header("Appearance")]

    [SerializeField] private GameObject[] _outfit;
    [SerializeField] private GameObject[] _head;
    [SerializeField] private GameObject[] _enemyWeapon;

    [Header("Characteristics")]

    [SerializeField] private int _enemyID;
    [SerializeField] private float _rotateTowardsPlayerSpeed = 3;
    [SerializeField] private int _health = 100;
    [SerializeField] private float _distanceToFollowPlayer = 25;
    [SerializeField] private float _distanceToAttack = 2f;
    [SerializeField] private float _distanceToAttackMultiplier = 4;
    private int _currentPos;

    private bool _inReverse;
    private bool _enemyHasFallen;

    [Header("State")]

    [SerializeField] private AIState _currentState;

    [SerializeField] private bool _isDead;
    [SerializeField] private bool _nearPlayer;
    [SerializeField] private bool _canAttack = true;

    [SerializeField] private LayerMask _floorMask;
    [SerializeField] private LayerMask _playerMask;
    [SerializeField] private GameObject _fallDetector;
    [SerializeField] private GameObject _enemyRightFist, _enemyLeftFist;
    [SerializeField] private GameObject _smokeCloud;
    [SerializeField] private GameObject _puddleOfBlood;


    private bool _isAttacking;
    private bool _hitByExplosion;
    private int _randomAnim;

    private NavMeshAgent _navmeshAgent;
    private Animator _animator;
    private Player _player;
    private float _originalSpeed;

    [SerializeField] private GameObject _ammoPickup;

    private enum AIState
    {
        Idle,
        Walk,
        Attack,
        Death
    }

    void Start()
    {
        _player = GameObject.Find("PlayerCapsule").GetComponent<Player>();
        _animator = GetComponent<Animator>();
        _navmeshAgent = GetComponent<NavMeshAgent>();
        _audioSource = GetComponent<AudioSource>();

        if (_navmeshAgent == null)
            Debug.LogError("Enemy Navmesh is NULL");

        if (_animator == null)
            Debug.LogError("Enemy Animator is NULL");

        if (_audioSource == null)
            Debug.LogError("Enemy AudioSource is NULL");

        if (_player == null)
            Debug.LogError("Player is NULL");

        GenerateZombie();

        _originalSpeed = _navmeshAgent.speed;

        StartCoroutine(EmergingFromGround());

        _currentPos = 0;
    }

    IEnumerator EmergingFromGround()
    {
        GameObject smokeCloud = PoolManager.Instance.RequestSmokeCloud();
        smokeCloud.transform.position = transform.position;
        yield return new WaitForSeconds(2.5f);
        _currentState = AIState.Idle;
    }

    void Update()
    {
        CurrentAIState(); //Finite State Machine
        PuddleOfBlood(); //will instantiate a puddle of blood if the player has fallen
    }

    public void SelectWayPoint(List<Transform> waypoint)
    {
        _wayPoint = new List<Transform>(waypoint); // gives this individual zombie a specific set of waypoints
    }

    private void CalculateMovement()
    {
        if (_nearPlayer == false)
        {
            MoveToWayPoint();
        }
        else
        {
            MoveTowardsPlayer();
            Debug.Log("Player within Range");
        }
    }

    private void MoveTowardsPlayer()
    {
        if (_player != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_player.transform.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotateTowardsPlayerSpeed * Time.deltaTime); //slowly turn to player

            float distanceFromPlayer = Vector3.Distance(transform.position, _player.transform.position);
            if (distanceFromPlayer <= _distanceToAttack)
            {
                _currentState = AIState.Attack;
            }
            else
            {
                _navmeshAgent.SetDestination(_player.transform.position); // set new destination to player position

                _animator.SetBool("Walking", true);
            }
        }
    }

    private void MoveToWayPoint()
    {
        if (_navmeshAgent.remainingDistance <= 1)
        {
            _animator.SetBool("Walking", false);

            if (_inReverse == true)
            {
                Reverse();
            }
            else
            {
                Forward();
            }

            if (!_nearPlayer)
            {
                _currentState = AIState.Idle;
            }
        }
        else
        {
            _animator.SetBool("Walking", true);
        }

        _navmeshAgent.SetDestination(_wayPoint[_currentPos].position); //move enemy to the next destination waypoint
    }

    private void CurrentAIState()
    {
        switch (_currentState)
        {
            case AIState.Idle:
                if (!_isDead)
                {
                    StartCoroutine("IdleRoutine");
                }
                break;

            case AIState.Walk:
                if (_player != null && !_isDead)
                {
                    CalculateMovement();

                    float distanceFromPlayer = Vector3.Distance(transform.position, _player.transform.position);
                    if (distanceFromPlayer >= _distanceToFollowPlayer)
                    {
                        _nearPlayer = false;
                    }
                    else
                    {
                        _nearPlayer = true;
                    }
                }
                break;

            case AIState.Attack:

                DamagePlayer();

                if (_canAttack == true)
                {
                    StopCoroutine("IdleRoutine");
                    StartCoroutine("AttackRoutine");
                    _canAttack = false;
                }
                break;

            case AIState.Death:
                _canAttack = false;

                if (!_isDead)
                {
                    StopCoroutine("IdleRoutine");
                    StopCoroutine("DeathRoutine");
                    StartCoroutine("DeathRoutine");
                }
                break;
        }
    }

    IEnumerator IdleRoutine()
    {
        _navmeshAgent.isStopped = true;
        yield return new WaitForSeconds(2);
        _navmeshAgent.isStopped = false;
        _currentState = AIState.Walk;
    }

    IEnumerator AttackRoutine()
    {
        _animator.SetTrigger("Attack");
        _animator.SetBool("Walking", false);
        _navmeshAgent.isStopped = true;

        yield return new WaitForSeconds(2);

        _navmeshAgent.isStopped = false;
        _animator.SetBool("Walking", true);
        _navmeshAgent.ResetPath();
        _isAttacking = false;
        _canAttack = true;

        _currentState = AIState.Walk;
    }

    IEnumerator DeathRoutine()
    {
        _isDead = true;
        _animator.SetBool("Death", true);
        _navmeshAgent.isStopped = true;

        yield return new WaitForSeconds(Random.Range(4, 6));

        if (_enemyID == 0)
        {
            _animator.SetBool("Emerge", true);

            yield return new WaitForSeconds(3);

            _animator.SetBool("Death", false);
            _animator.SetBool("Emerge", false);

            _navmeshAgent.ResetPath(); //Reset back to waypoint path

            _navmeshAgent.isStopped = false;
            _enemyHasFallen = false;
            _canAttack = true;
            _hitByExplosion = false;
            _isDead = false;
            _health = 100;

            _currentState = AIState.Walk;
        }
    }
    private void DamagePlayer()
    {
        if (_randomAnim == 0)
        {
            if (Physics.Raycast(_enemyRightFist.transform.position, _enemyRightFist.transform.up, _distanceToAttack * _distanceToAttackMultiplier, _playerMask))
            {
                Debug.DrawRay(_enemyRightFist.transform.position, _enemyRightFist.transform.up * (_distanceToAttack), Color.red);

                if (_player != null && _isAttacking == false)
                {
                    _player.DamagePlayer(10);
                    _isAttacking = true;
                }
            }
        }
        else if (_randomAnim == 1)
        {
            if (Physics.Raycast(_enemyLeftFist.transform.position, _enemyLeftFist.transform.up, _distanceToAttack * _distanceToAttackMultiplier, _playerMask))
            {
                Debug.DrawRay(_enemyLeftFist.transform.position, _enemyLeftFist.transform.up * (_distanceToAttack), Color.red);

                if (_player != null && _isAttacking == false)
                {
                    _player.DamagePlayer(10);
                    _isAttacking = true;
                }
            }
        }
        else if (_randomAnim == 2)
        {
            if (Physics.Raycast(_enemyRightFist.transform.position, -_enemyRightFist.transform.forward, _distanceToAttack * _distanceToAttackMultiplier, _playerMask))
            {
                Debug.DrawRay(_enemyRightFist.transform.position, -_enemyRightFist.transform.forward * (_distanceToAttack * _distanceToAttackMultiplier), Color.red);

                if (_player != null && _isAttacking == false)
                {
                    _player.DamagePlayer(30);
                    _isAttacking = true;
                }
            }
        }
    }

    public void EnemyDamage(int damageTaken) //called from player
    {
        _health -= damageTaken;

        if (_health <= 0)
        {
            if (_player != null && !_isDead)
            {
                _currentState = AIState.Death;
                int random = Random.Range(0, 101);
                if (_enemyID == 0 && random <= 50)
                {
                    Instantiate(_ammoPickup, transform.position, Quaternion.identity);
                }
                int randomSFX = Random.Range(0, _audioClip.Length);
                PlaySFX(randomSFX);
                _player.AddToScore(50);
            }
        }
        else
        {
            int randomSFX = Random.Range(0, _audioClip.Length);
            PlaySFX(randomSFX);
            _animator.SetTrigger("Hit");
        }
    }

    private void PlaySFX(int audioclip)
    {
        AudioManager.Instance.PlaySFX(_audioSource, _audioClip[audioclip]);
    }

    private void Reverse()
    {
        if (_currentPos == 0)
        {
            _currentPos = 0;
            _inReverse = false;
        }
        else
        {
            _currentPos--;
        }
    }
    private void Forward()
    {
        if (_currentPos == _wayPoint.Count - 1)
        {
            _inReverse = true;
            _currentPos--;
        }
        else
        {
            _currentPos++;
        }
    }
    private void GenerateZombie()
    {
        for (int i = 0; i < _outfit.Length; i++)
        {
            _outfit[i].SetActive(false);
        }
        for (int i = 0; i < _head.Length; i++)
        {
            _head[i].SetActive(false);
        }

        int randomOutfit = Random.Range(0, _outfit.Length);
        _outfit[randomOutfit].SetActive(true);

        int randomHead = Random.Range(0, _head.Length);
        _head[randomHead].SetActive(true);

        for (int i = 0; i < _enemyWeapon.Length; i++)
        {
            _enemyWeapon[i].SetActive(false);
        }

        int chanceOfHavingWeapon = Random.Range(0, 101);
        if (chanceOfHavingWeapon > 10)
        {
            _randomAnim = Random.Range(0, 2);
        }
        else
        {
            int randomWeapon = Random.Range(0, _enemyWeapon.Length);
            _enemyWeapon[randomWeapon].SetActive(true);
            _randomAnim = 2;
        }

        for (int i = 0; i < 3; i++)
        {
            _animator.SetLayerWeight(i, (_randomAnim == i) ? 1 : 0); //chooses animation layer at random
        }

        if (_enemyID == 0)
        {
            float randomSpeed = Random.Range(0.75f, 2f);
            _navmeshAgent.speed = randomSpeed;

            if (_randomAnim == 0 || _randomAnim == 1)
            {
                _distanceToAttackMultiplier = 1.25f;
            }
            else if (_randomAnim == 2)
            {
                _distanceToAttackMultiplier = 2f;
            }
        }
        else if (_enemyID == 1)
        {
            _distanceToAttackMultiplier = 4f;
        }
    }
    private void PuddleOfBlood()
    {
        if (!_enemyHasFallen)
        {
            RaycastHit hit;
            if (_randomAnim == 0 || _randomAnim == 2)
            {
                if (Physics.Raycast(_fallDetector.transform.position, _fallDetector.transform.forward, out hit, 0.5f, _floorMask))
                {
                    if (_isDead)
                    {
                        if (_enemyID == 0)
                        {
                            GameObject puddleOfBlood = PoolManager.Instance.RequestPuddleOfBlood();
                            puddleOfBlood.transform.position = hit.point + new Vector3(0, 0.05f, 0);
                            puddleOfBlood.transform.rotation = Quaternion.LookRotation(hit.normal);
                        }

                        else if (_enemyID == 1)
                        {
                            Instantiate(_smokeCloud, hit.point + new Vector3(0, 0.07f, 0), Quaternion.identity);
                            Instantiate(_puddleOfBlood, hit.point + new Vector3(0, 0.08f, 0), Quaternion.LookRotation(hit.normal));
                        }
                        _enemyHasFallen = true;
                    }
                }
            }
            else if (_randomAnim == 1)
            {
                if (Physics.Raycast(_fallDetector.transform.position, -_fallDetector.transform.forward, out hit, 1f, _floorMask))
                {
                    if (_isDead)
                    {
                        if (_enemyID == 0)
                        {
                            GameObject puddleOfBlood = PoolManager.Instance.RequestPuddleOfBlood();
                            puddleOfBlood.transform.position = hit.point + new Vector3(0, 0.05f, 0);
                            puddleOfBlood.transform.rotation = Quaternion.LookRotation(hit.normal);
                        }

                        else if (_enemyID == 1)
                        {
                            Instantiate(_smokeCloud, hit.point + new Vector3(0, 0.07f, 0), Quaternion.identity);
                            Instantiate(_puddleOfBlood, hit.point + new Vector3(0, 0.08f, 0), Quaternion.LookRotation(hit.normal));
                        }
                        _enemyHasFallen = true;
                    }
                }
            }
        }
    }

    public void PauseEnemyMovement(bool playerInDialogue)
    {
        if (_navmeshAgent != null)
        {
            if (playerInDialogue)
            {
                _navmeshAgent.speed = 0.1f;
            }
            else
            {
                _navmeshAgent.speed = _originalSpeed;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Explosion")
        {
            if (!_hitByExplosion)
            {
                EnemyDamage(100);
                _hitByExplosion = true;
            }
        }
    }
}