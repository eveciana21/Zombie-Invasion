using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent _navmeshAgent;
    [SerializeField] private List<Transform> _wayPoint;

    private int _currentPos;
    [SerializeField] private int _health = 100;
    [SerializeField] private int _score;

    private bool _inReverse;
    private bool _enemyHasFallen;
    [SerializeField] private bool _isWalking = true;
    [SerializeField] private bool _isDead;

    [SerializeField] private GameObject[] _outfit;
    [SerializeField] private GameObject[] _head;

    [SerializeField] private GameObject _fallDetector;
    [SerializeField] private LayerMask _floorMask;

    private Animator _animator;

    private Player _player;

    [SerializeField] private AIState _currentState;

    private enum AIState
    {
        Idle,
        Walk,
        Attack,
        Death
    }

    void Start()
    {
        _health = 100;
        _currentState = AIState.Walk;

        GenerateZombie();

        _player = GameObject.Find("Player").GetComponent<Player>();
        _animator = GetComponent<Animator>();
        _navmeshAgent = GetComponent<NavMeshAgent>();

        if (_navmeshAgent == null)
        {
            Debug.Log("Enemy Navmesh is Null");
        }
        if (_animator == null)
        {
            Debug.Log("Enemy Animator is Null");
        }

        //_navmeshAgent.destination = _wayPoint[0].position; //<-- not sure if I need this yet

        _currentPos = 0;
    }

    void Update()
    {
        CurrentAIState(); //Finite State Machine
        PuddleOfBlood(); //will instantiate a puddle of blood if the player has fallen
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

    public void SelectWayPoint(List<Transform> waypoint)
    {
        _wayPoint = new List<Transform>(waypoint); // gives this individual zombie a specific set of waypoints
    }

    private void CalculateMovement()
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
                }
                break;

            case AIState.Death:
                _isWalking = false;
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

        _currentState = AIState.Walk;
        _navmeshAgent.isStopped = false;
        _enemyHasFallen = false;
        _isDead = false;
        _health = 100;
    }


    public void EnemyDeath(int damageTaken)
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
}


