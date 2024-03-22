using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private static PoolManager _instance;
    public static PoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Pool Manager is NULL");
            }
            return _instance;
        }
    }

    [SerializeField] private GameObject _bullet;
    [SerializeField] private GameObject _bulletContainer;
    [SerializeField] private List<GameObject> _bulletPool;

    private void Awake()
    {
        _instance = this;
    }
    private void Start()
    {
        _bulletPool = GenerateBullet(10);
    }


    List<GameObject> GenerateBullet(int amountOfBullets)
    {

        for (int i = 0; i < amountOfBullets; i++)
        {
            GameObject bullet = Instantiate(_bullet);
            bullet.transform.parent = _bulletContainer.transform;
            bullet.SetActive(false);
            _bulletPool.Add(bullet);
        }
        return _bulletPool;
    }

    public GameObject RequestBullet()
    {
        foreach (var bullet in _bulletPool)
        {
            if (bullet.activeInHierarchy == false)
            {
                bullet.SetActive(true);
                return bullet;
            }
        }
        GameObject newBullet = Instantiate(_bullet);
        newBullet.transform.parent = _bulletContainer.transform;
        _bulletPool.Add(newBullet);

        return newBullet;
    }
}

