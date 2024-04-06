using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private List<Transform> _wayPoint;

    [Header("Appearance")]

    [SerializeField] private GameObject[] _outfit;
    [SerializeField] private GameObject[] _head;

    [Header("Characteristics")]

    [SerializeField] private float _rotateTowardsPlayerSpeed = 3;
    [SerializeField] private int _health = 100;
    private float _distanceToAttack = 2f;
    private int _currentPos;

    private bool _inReverse;
    private bool _enemyHasFallen;

    [Header("State")]

    [SerializeField] private AIState _currentState;

    [SerializeField] private bool _isWalking = true;
    [SerializeField] private bool _isDead;
    [SerializeField] private bool _nearPlayer;
    [SerializeField] private bool _canAttack = true;

    [SerializeField] private LayerMask _floorMask;
    [SerializeField] private LayerMask _playerMask;
    [SerializeField] private GameObject _fallDetector;
    [SerializeField] private GameObject _enemyFist;

    private bool _isAttacking;
    private bool _isOutOfGround;

    private NavMeshAgent _navmeshAgent;
    private Animator _animator;
    private Player _player;

    [SerializeField] private GameObject _smokeCloud;

    //private string _animationState;

    private enum AIState
    {
        Idle,
        Walk,
        Attack,
        Death
    }

    void Start()
    {
        _player = GameObject.Find("Player").GetComponentInChildren<Player>();
        _animator = GetComponent<Animator>();
        _navmeshAgent = GetComponent<NavMeshAgent>();

        if (_navmeshAgent == null)
            Debug.LogError("Enemy Navmesh is NULL");

        if (_animator == null)
            Debug.LogError("Enemy Animator is NULL");

        if (_player == null)
            Debug.LogError("Player is NULL");

        GenerateZombie();

        StartCoroutine(EmergingFromGround());

        GameObject smokeCloud = PoolManager.Instance.RequestSmokeCloud();
        smokeCloud.transform.position = transform.position;

        //_navmeshAgent.destination = _wayPoint[0].position; //<-- not sure if I need this yet

        _currentPos = 0;

        int randomAnimation = Random.Range(0, 2);
        if (randomAnimation == 0)
        {
            _animator.SetLayerWeight(0, 1);
            _animator.SetLayerWeight(1, 0);
            Debug.Log("Random Variant 1");
        }
        if (randomAnimation == 1)
        {
            _animator.SetLayerWeight(0, 0);
            _animator.SetLayerWeight(1, 1);
            Debug.Log("Random Variant 2");
        }


    }

    IEnumerator EmergingFromGround()
    {
        yield return new WaitForSeconds(2.5f);
        _currentState = AIState.Walk;
        _isOutOfGround = true;
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
            if (_navmeshAgent.remainingDistance < 1)
            {
                _isWalking = false;
                _animator.SetBool("Walking", false);

                if (_inReverse == true)
                {
                    Reverse();
                }
                else
                {
                    Forward();
                }
                _navmeshAgent.SetDestination(_wayPoint[_currentPos].position); //move enemy to the next destination waypoint
                _currentState = AIState.Idle;
            }
            else
            {
                _isWalking = true;
                _animator.SetBool("Walking", true);
            }
        }
        else
        {
            Debug.Log("Player within Range");
            _navmeshAgent.SetDestination(_player.transform.position); // set new destination to player position

            if (_isWalking == true)
            {
                _animator.SetBool("Walking", true);
            }
            else
            {
                _animator.SetBool("Walking", false);
            }
        }
    }


    private void CurrentAIState()
    {
        switch (_currentState)
        {
            case AIState.Idle:
                if (_isWalking == false && !_isDead)
                {
                    StartCoroutine("IdleRoutine");
                }
                break;

            case AIState.Walk:
                if (!_isDead)
                {
                    CalculateMovement();

                    float distanceFromPlayer = Vector3.Distance(transform.position, _player.transform.position);

                    if (distanceFromPlayer < 20)
                    {
                        _nearPlayer = true;
                        Quaternion targetRotation = Quaternion.LookRotation(_player.transform.position - transform.position, Vector3.up);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotateTowardsPlayerSpeed * Time.deltaTime); //slowly turn to player

                        if (distanceFromPlayer < _distanceToAttack)
                        {
                            _currentState = AIState.Attack;
                        }
                    }
                    /*if (distanceFromPlayer > 20)
                    {
                        _nearPlayer = false;
                        _currentState = AIState.Walk;
                    }*/

                    else
                    {
                        _nearPlayer = false;
                        return;
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
                _isWalking = false;
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
        _isWalking = true;
    }

    IEnumerator AttackRoutine()
    {
        _isWalking = false;
        _animator.SetTrigger("Attack");
        _animator.SetBool("Walking", false);
        _navmeshAgent.isStopped = true;

        yield return new WaitForSeconds(2);

        _navmeshAgent.isStopped = false;
        _navmeshAgent.ResetPath();
        _isAttacking = false;
        _isWalking = true;
        _canAttack = true;

        _currentState = AIState.Walk;
    }

    IEnumerator DeathRoutine()
    {
        _isDead = true;
        _animator.SetBool("Death", true);
        _navmeshAgent.isStopped = true;

        yield return new WaitForSeconds(3);

        _animator.SetBool("Emerge", true);

        yield return new WaitForSeconds(3);

        _animator.SetBool("Death", false);
        _animator.SetBool("Emerge", false);

        _navmeshAgent.ResetPath(); //Reset back to waypoint path

        _navmeshAgent.isStopped = false;
        _enemyHasFallen = false;
        _isWalking = true;
        _canAttack = true;
        _isDead = false;
        _health = 100;

        _currentState = AIState.Walk;
    }

    private void DamagePlayer()
    {
        if (Physics.Raycast(_enemyFist.transform.position, _enemyFist.transform.forward, _distanceToAttack * 1.5f, _playerMask))
        {
            if (_isAttacking == false)
            {
                Debug.Log("Hit Player");
                _player.DamagePlayer();
                _isAttacking = true;
            }
        }
    }

    public void EnemyDeath(int damageTaken) //called from player
    {
        _health -= damageTaken;

        if (_health <= 0)
        {
            if (!_isDead)
            {
                _currentState = AIState.Death;
                _player.AddToScore(50);
            }
        }
        else
        {
            _animator.SetTrigger("Hit");
        }
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
    }
    private void PuddleOfBlood()
    {
        if (!_enemyHasFallen)
        {
            RaycastHit hit;
            if (Physics.Raycast(_fallDetector.transform.position, _fallDetector.transform.forward, out hit, 0.5f, _floorMask))
            {
                if (_isDead)
                {
                    GameObject puddleOfBlood = PoolManager.Instance.RequestPuddleOfBlood();
                    puddleOfBlood.transform.position = hit.point + new Vector3(0, 0.07f, 0);
                    puddleOfBlood.transform.rotation = Quaternion.LookRotation(hit.normal);
                    _enemyHasFallen = true;
                }
            }
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Explosion")
        {
            print("Died");
            _currentState = AIState.Death;
            _health = 0;
        }
    }

}