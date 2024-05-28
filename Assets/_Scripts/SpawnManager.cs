using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    private Transform _waypointParent;
    [SerializeField] private List<Transform> _wayPoint;
    private EnemyAI _zombie;

    [SerializeField] private GameObject[] _zombiePrefab;
    [SerializeField] private GameObject _zombieContainer;
    [SerializeField] private int _enemiesInGame = 0;

    private NPC _npc;
    [SerializeField] private List<NPC> _npcList;

    [SerializeField] private GameObject _zombieBoss;
    [SerializeField] private Transform _spawnBossPos;

    public override void Init()
    {
        base.Init(); //Turns this class into a singleton
    }

    private void Start()
    {
        _npc = FindObjectOfType<NPC>();
    }
    public void SpawnEnemies()
    {
        StartCoroutine(ZombieSpawner());
    }


    public IEnumerator ZombieSpawner()
    {
        while (_enemiesInGame <= 65)
        {
            _waypointParent = GameObject.Find("Waypoints").transform;
            int random = Random.Range(0, _waypointParent.childCount);
            Transform waypointGroup = _waypointParent.GetChild(random); //gets a random waypoint group

            _wayPoint.Clear();

            for (int i = 0; i < waypointGroup.childCount; i++)
            {
                Transform waypointChild = waypointGroup.GetChild(i);
                _wayPoint.Add(waypointChild); //adds the waypoint children to the list
            }

            int randomChild = Random.Range(0, _wayPoint.Count);
            Transform randomWaypoint = _wayPoint[randomChild]; //gets the specific child from the for loop iteration

            int randomZombieGender = Random.Range(0, _zombiePrefab.Length);
            _zombie = Instantiate(_zombiePrefab[randomZombieGender], randomWaypoint.position, Quaternion.identity).GetComponent<EnemyAI>();
            _zombie.transform.parent = _zombieContainer.transform;
            _zombie.SelectWayPoint(_wayPoint); //gives this individual zombie prefab a set of waypoints

            if (_npc != null)
            {
                foreach (NPC npc in _npcList)
                {
                    npc.AddEnemyToList(_zombie.GetComponent<EnemyAI>()); //add zombie to NPC list 
                }
            }
            _enemiesInGame++;
            yield return new WaitForSeconds(3f);
        }
        if (_enemiesInGame >= 65)
        {
            Debug.Log("Enemy Count has reached max");
        }
    }

    public void SpawnBoss()
    {
        _waypointParent = GameObject.Find("Other Waypoints").transform;
        Transform bossWaypoints = _waypointParent.GetChild(0);

        _wayPoint.Clear();

        for (int i = 0; i < bossWaypoints.childCount; i++)
        {
            Transform waypointChild = bossWaypoints.GetChild(i);
            _wayPoint.Add(waypointChild); //adds the waypoint children to the list
        }
        _zombie = Instantiate(_zombieBoss, _spawnBossPos.position, Quaternion.identity).GetComponent<EnemyAI>();
        _zombie.SelectWayPoint(_wayPoint);

        if (_npc != null)
        {
            foreach (NPC npc in _npcList)
            {
                npc.AddEnemyToList(_zombie.GetComponent<EnemyAI>()); //add zombie to NPC list 
            }
        }
    }
}


