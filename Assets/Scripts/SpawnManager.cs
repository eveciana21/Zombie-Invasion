using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _zombiePrefab;
    [SerializeField] private Transform _spawnPoint;

    [SerializeField] private List<GameObject> _instances;

    private Transform _wayPointGroup;

    void Start()
    {
        StartCoroutine(ZombieSpawner());
    }

    IEnumerator ZombieSpawner()
    {
        while (true)
        {
            //Spawner(_spawnPoint);
            //_spawnPoint = waypoint;

            _wayPointGroup = GameObject.Find("Waypoints").transform;
            int random = Random.Range(0, _wayPointGroup.childCount); //random waypoint group
            Transform waypointGroup = _wayPointGroup.GetChild(random); //gets the waypoint group parent
            int randomChild = Random.Range(0, waypointGroup.childCount); //random child from the waypoint groups
            Transform waypointChild = waypointGroup.GetChild(randomChild); //gets the random child waypoint from the waypoint group

            Instantiate(_zombiePrefab, waypointChild.transform.position, Quaternion.identity);
            Debug.Log("Waypoint Group: " + waypointGroup.name + " WayPoint Child: " + waypointChild.name);
            yield return new WaitForSeconds(3);
        }
    }


    public void Spawner(Transform waypoint)
    {
        _spawnPoint = waypoint;

        _wayPointGroup = GameObject.Find("Waypoints").transform;
        int random = Random.Range(0, _wayPointGroup.childCount); //random waypoint group
        Transform waypointGroup = _wayPointGroup.GetChild(random); //gets the waypoint group parent
        int randomChild = Random.Range(0, waypointGroup.childCount); //random child from the waypoint groups
        Transform waypointChild = waypointGroup.GetChild(randomChild); //gets the random child waypoint from the waypoint group

        Instantiate(_zombiePrefab, waypoint.transform.position, Quaternion.identity);
        Debug.Log(waypointChild.name);
    }







    /*private void SpawnZombies()
    {
        _instances.Clear();
        for (int i = 0; i < 10; i++)
        {
            GameObject instance = Instantiate(_zombiePrefab);
            _instances.Add(instance);
        }
    }*/

}
