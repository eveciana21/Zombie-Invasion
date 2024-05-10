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
    private bool _dialogueTextOnScreen;

    private float _rotateTowardsPlayerSpeed = 3f;
    private Player _player;

    [Header("Dialogue")]
    [SerializeField] private string _dialogueText;
    [SerializeField] private string _secondaryDialogueText;
    [SerializeField] private string _tertiaryDialogueText;

    [Space]

    [SerializeField] private int _interactionDistance;

    [SerializeField] private string _npcName;
    [SerializeField] private GameObject _gift;
    [SerializeField] private GameObject _potionImage;
    [SerializeField] private GameObject _potionMiniMapIcon;

    [SerializeField] private List<EnemyAI> _enemyList = new List<EnemyAI>();

    private enum AIState
    {
        Idle,
        Walk,
        Talk
    }

    [SerializeField] private AIState _currentState;

    void Start()
    {
        _animator = GetComponent<Animator>();

        _player = GameObject.Find("Player").GetComponentInChildren<Player>();
        if (_player == null)
            Debug.LogError("Player is NULL");

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.destination = _wayPoint[0].position;

        _currentPos = 0;

        _currentState = AIState.Idle;
    }

    void Update()
    {
        if (_player != null)
        {
            CurrentAIState();
   
            float distanceFromPlayer = Vector3.Distance(transform.position, _player.transform.position);
            if (distanceFromPlayer <= _interactionDistance)
            {
                _currentState = AIState.Talk;
                SlowEnemySpeed();
                _player.isEngagingInDialogue(true);
            }
        }
    }

    public void AddEnemyToList(EnemyAI enemy)
    {
        _enemyList.Add(enemy);
    }

    private void SlowEnemySpeed()
    {
        foreach (var enemy in _enemyList)
        {
            enemy.PauseEnemyMovement(true);
        }
    }

    private void ResumeEnemySpeed()
    {
        foreach (var enemy in _enemyList)
        {
            enemy.PauseEnemyMovement(false);
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

                if (_dialogueTextOnScreen == false)
                {
                    UIManager.Instance.DialogueText(true, _npcName, _dialogueText, _secondaryDialogueText, _tertiaryDialogueText);
                    StopCoroutine("IdleRoutine");
                    _animator.SetBool("Walking", false);
                    _navMeshAgent.isStopped = true;
                    _dialogueTextOnScreen = true;
                    _isWalking = false;
                }

                float distanceFromPlayer = Vector3.Distance(transform.position, _player.transform.position);
                if (distanceFromPlayer >= _interactionDistance)
                {
                    UIManager.Instance.DialogueText(false, _npcName, _dialogueText, _secondaryDialogueText, _tertiaryDialogueText);
                    _dialogueTextOnScreen = false;
                    ResumeEnemySpeed();
                    _player.isEngagingInDialogue(false);
                    _currentState = AIState.Idle;
                }
                UIManager.Instance.ActivateGift(_npcName, _gift, _potionImage, _potionMiniMapIcon);

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

