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

    [SerializeField] private GameObject _muzzleFlash;
    [SerializeField] private GameObject _muzzleFlashContainer;
    [SerializeField] private List<GameObject> _muzzleFlashPool;

    private void Awake()
    {
        _instance = this;
    }
    private void Start()
    {
        _bulletPool = GenerateBullet(10);
        _muzzleFlashPool = GenerateMuzzleFlash(20);
    }

    //BULLET POOL
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

    // // // // // // // // // // // // // // // // // // // // // // // //

    //MUZZLE FLASH POOL
    List<GameObject> GenerateMuzzleFlash(int muzzleFlashQuantity)
    {

        for (int i = 0; i < muzzleFlashQuantity; i++)
        {
            GameObject muzzleFlash = Instantiate(_muzzleFlash);
            muzzleFlash.transform.parent = _muzzleFlashContainer.transform;
            muzzleFlash.SetActive(false);
            _muzzleFlashPool.Add(muzzleFlash);
        }
        return _muzzleFlashPool;
    }

    public GameObject RequestMuzzleFlash()
    {
        foreach (var muzzleFlash in _muzzleFlashPool)
        {
            if (muzzleFlash.activeInHierarchy == false)
            {
                muzzleFlash.SetActive(true);
                return muzzleFlash;
            }
        }
        GameObject newMuzzleFlash = Instantiate(_muzzleFlash);
        newMuzzleFlash.transform.parent = _muzzleFlashContainer.transform;
        _muzzleFlashPool.Add(newMuzzleFlash);

        return newMuzzleFlash;
    }
}

