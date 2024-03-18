using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent _navmeshAgent;
    [SerializeField] private List<Transform> _wayPoint;

    private int _currentPos;
    private bool _inReverse;
    private bool _isWalking;
    private bool _isDead;

    [SerializeField] private GameObject[] _outfit;
    [SerializeField] private GameObject[] _head;

    private Animator _animator;

    private enum AIState
    {
        Idle,
        Walk,
        Run,
        Attack,
        Death
    }

    [SerializeField] private AIState _currentState;

    void Start()
    {
        GenerateZombie();

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

        _navmeshAgent.destination = _wayPoint[0].position;

        _currentPos = 0;
    }

    void Update()
    {
        CurrentAIState();
    }

    public void SelectWayPoint(List<Transform> waypoint)
    {
        _wayPoint = new List<Transform>(waypoint);
    }

    private void CalculateMovement()
    {
        if (_navmeshAgent.remainingDistance < 1f)
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
            _navmeshAgent.SetDestination(_wayPoint[_currentPos].position);
            _currentState = AIState.Idle;
        }
        else
        {
            _animator.SetBool("Walking", true);
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

    private void CurrentAIState()
    {
        switch (_currentState)
        {
            case AIState.Idle:
                if (_isWalking == false || _isDead == false)
                {
                    StartCoroutine("IdleRoutine");
                    _isWalking = true;
                }
                break;

            case AIState.Walk:
                if (_isDead == false)
                {
                    CalculateMovement();
                }
                break;




            //////////////////////////////////////////////////////////////////////////////////





            case AIState.Run:
                Debug.Log("Run State");
                break;

            case AIState.Attack:
                Debug.Log("Attack State");
                break;

            case AIState.Death:
                StopCoroutine("IdleRoutine");
                StartCoroutine("DeathRoutine");
                _isDead = false;
                Debug.Log("Death State");
                break;
        }
    }
    IEnumerator IdleRoutine()
    {
        _navmeshAgent.isStopped = true;
        yield return new WaitForSeconds(3);
        _navmeshAgent.isStopped = false;
        _currentState = AIState.Walk;
        _isWalking = false;
    }


    IEnumerator DeathRoutine()
    {
        _isDead = true;
        _navmeshAgent.isStopped = true;
        _animator.SetBool("Walking", false);
        _animator.SetTrigger("Death");
        yield return new WaitForSeconds(3);
        _animator.SetTrigger("Emerge");
        _navmeshAgent.isStopped = false;
        _currentState = AIState.Walk;
    }
}
