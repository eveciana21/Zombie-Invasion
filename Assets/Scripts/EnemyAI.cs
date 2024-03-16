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

    [SerializeField] private GameObject[] _outfit;
    [SerializeField] private GameObject[] _head;

    private SpawnManager _spawnManager;

    void Start()
    {
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();

        RandomWayPointPath();
        GenerateZombie();

        _navmeshAgent = GetComponent<NavMeshAgent>();
        _navmeshAgent.destination = _wayPoint[0].position;

        _currentPos = 0;
    }

    void Update()
    {
        CalculateMovement();
    }

    private void CalculateMovement()
    {
        if (_navmeshAgent.remainingDistance < 1f)
        {
            if (_inReverse == true)
            {
                Reverse();
            }
            else
            {
                Forward();
            }
            _navmeshAgent.SetDestination(_wayPoint[_currentPos].position);
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

    private void RandomWayPointPath()
    {
        Transform childWayPoint = GameObject.Find("Waypoints").transform;
        int random = Random.Range(0, childWayPoint.childCount);
        Transform waypointGroup = childWayPoint.GetChild(random);

        _wayPoint.Clear();

        for (int i = 0; i < waypointGroup.childCount; i++)
        {
            _wayPoint.Add(waypointGroup.GetChild(i));

            //_spawnManager.Spawner(waypointGroup.GetChild(random));

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
        int randomHead = Random.Range(0, _head.Length);
        _outfit[randomOutfit].SetActive(true);
        _head[randomHead].SetActive(true);
    }
}
