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

    private float _rotateTowardsPlayerSpeed = 3f;
    private Player _player;

    [SerializeField] private int _npcID;
    private bool _nearPlayer;
    private bool _dialogTextOnScreen;

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

        _player = GameObject.Find("Player").GetComponentInChildren<Player>();

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.destination = _wayPoint[0].position;

        _currentPos = 0;

        _currentState = AIState.Idle;
    }

    void Update()
    {
        CurrentAIState();

        float distanceFromPlayer = Vector3.Distance(transform.position, _player.transform.position);
        if (distanceFromPlayer < 7f)
        {
            _currentState = AIState.Talk;
        }
    }

    private void CurrentAIState()
    {
        switch (_currentState)
        {
            case AIState.Idle:
                if (_isWalking == false)
                {
                    StartCoroutine("IdleRoutine");
                }
                break;

            case AIState.Walk:
                CalculateMovement();
                break;

            case AIState.Talk:

                Quaternion targetRotation = Quaternion.LookRotation(_player.transform.position - transform.position, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotateTowardsPlayerSpeed * Time.deltaTime);

                if (_dialogTextOnScreen == false)
                {
                    StopCoroutine("IdleRoutine");
                    _animator.SetBool("Walking", false);
                    UIManager.Instance.DialogText(_npcID, true);
                    _navMeshAgent.isStopped = true;
                    _dialogTextOnScreen = true;
                    _isWalking = false;
                }

                float distanceFromPlayer = Vector3.Distance(transform.position, _player.transform.position);
                if (distanceFromPlayer > 7)
                {
                    UIManager.Instance.DialogText(_npcID, false);
                    _dialogTextOnScreen = false;
                    _currentState = AIState.Idle;
                }

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


}

