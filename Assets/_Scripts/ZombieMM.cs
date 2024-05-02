using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieMM : MonoBehaviour
{
    private NavMeshAgent _navmeshAgent;

    private bool _inReverse;

    private int _currentPos;

    [SerializeField] private List<Transform> _wayPoint;

    void Start()
    {
        _navmeshAgent = GetComponent<NavMeshAgent>();
        if (_navmeshAgent == null)
            Debug.LogError("Enemy Navmesh is NULL");
        _currentPos = 0;
    }

    void Update()
    {
        MoveToWayPoint();
    }

    private void MoveToWayPoint()
    {
        if (_navmeshAgent.remainingDistance <= 1)
        {

            if (_inReverse == true)
            {
                Reverse();
            }
            else
            {
                Forward();
            }
        }

        _navmeshAgent.SetDestination(_wayPoint[_currentPos].position); //move enemy to the next destination waypoint
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
