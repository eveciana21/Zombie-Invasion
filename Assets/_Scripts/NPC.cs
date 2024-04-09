using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class NPC : MonoBehaviour
{
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    [SerializeField] private List<Transform> _wayPoint;
    private int _currentPos;

    private bool _isWalking;
    private bool _inReverse;

    private enum AIState
    {
        Idle,
        Walk,
        Talk
    }

    [SerializeField] private AIState _currentState;


    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.destination = _wayPoint[0].position;

        _currentPos = 0;

        _currentState = AIState.Idle;
    }

    void Update()
    {
        switch (_currentState)
        {
            case AIState.Idle:
                if (_isWalking == false)
                {
                    StartCoroutine(IdleRoutine());
                }
                break;

            case AIState.Walk:
                CalculateMovement();
                break;

            case AIState.Talk:
                break;
        }
    }


    IEnumerator IdleRoutine()
    {
        _animator.SetBool("Walking", false);
        _navMeshAgent.isStopped = true;
        yield return new WaitForSeconds(2);
        _navMeshAgent.isStopped = false;
        _currentState = AIState.Walk;
    }

    private void CalculateMovement()
    {
        if (_navMeshAgent.remainingDistance < 1)
        {
            _isWalking = false;
            _animator.SetBool("Walking", false);

            if (_currentPos == _wayPoint.Count)
            {
                _currentPos = 0;
            }

            _navMeshAgent.SetDestination(_wayPoint[_currentPos++].position); //move enemy to the next destination waypoint
            _currentState = AIState.Idle;
        }
        else
        {
            _isWalking = true;
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
}

