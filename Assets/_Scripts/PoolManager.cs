using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoSingleton<PoolManager>
{
    [SerializeField] private GameObject _muzzleFlash;
    [SerializeField] private GameObject _muzzleFlashContainer;
    [SerializeField] private List<GameObject> _muzzleFlashPool;

    [SerializeField] private GameObject[] _blood;
    [SerializeField] private GameObject _bloodContainer;
    [SerializeField] private List<GameObject> _bloodPool;

    [SerializeField] private GameObject _puddleOfBlood;
    [SerializeField] private GameObject _puddleOfBloodContainer;
    [SerializeField] private List<GameObject> _puddleOfBloodPool;

    [SerializeField] private GameObject _smokeCloud;
    [SerializeField] private GameObject _smokeCloudContainer;
    [SerializeField] private List<GameObject> _smokeCloudPool;

    public override void Init()
    {
        base.Init(); //Turns this class into a singleton
    }

    private void Start()
    {
        _muzzleFlashPool = GenerateMuzzleFlash(10);
        _bloodPool = GenerateBlood(10);
        _puddleOfBloodPool = GeneratePuddleOfBlood(10);
        _smokeCloudPool = GenerateSmokeCloud(10);
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

    // // // // // // // // // // // // // // // // // // // // // // // //
    //BlOOD SPLATTER POOL//

    List<GameObject> GenerateBlood(int bloodQuantity)
    {

        for (int i = 0; i < bloodQuantity; i++)
        {
            int random = Random.Range(0, _blood.Length);
            GameObject blood = Instantiate(_blood[random]);
            blood.transform.parent = _bloodContainer.transform;
            blood.SetActive(false);
            _bloodPool.Add(blood);
        }
        return _bloodPool;
    }

    public GameObject RequestBlood()
    {
        foreach (var blood in _bloodPool)
        {
            if (blood.activeInHierarchy == false)
            {
                blood.SetActive(true);
                return blood;
            }
        }
        int random = Random.Range(0, _blood.Length);
        GameObject newBlood = Instantiate(_blood[random]);
        newBlood.transform.parent = _bloodContainer.transform;
        _bloodPool.Add(newBlood);

        return newBlood;
    }

    // // // // // // // // // // // // // // // // // // // // // // // //
    //PUDDLE OF BLOOD POOL//

    List<GameObject> GeneratePuddleOfBlood(int puddleOfBloodAmount)
    {
        for (int i = 0; i < puddleOfBloodAmount; i++)
        {
            GameObject puddleOfBlood = Instantiate(_puddleOfBlood);
            puddleOfBlood.transform.parent = _puddleOfBloodContainer.transform;
            puddleOfBlood.SetActive(false);
            _puddleOfBloodPool.Add(puddleOfBlood);
        }
        return _puddleOfBloodPool;
    }

    public GameObject RequestPuddleOfBlood()
    {
        foreach (var puddleOfBlood in _puddleOfBloodPool)
        {
            if (puddleOfBlood.activeInHierarchy == false)
            {
                puddleOfBlood.SetActive(true);
                return puddleOfBlood;
            }
        }
        GameObject newPuddleOfBlood = Instantiate(_puddleOfBlood);
        newPuddleOfBlood.transform.parent = _puddleOfBloodContainer.transform;
        _muzzleFlashPool.Add(newPuddleOfBlood);

        return newPuddleOfBlood;
    }

    // // // // // // // // // // // // // // // // // // // // // // // //
    //SMOKE CLOUD POOL//

    List<GameObject> GenerateSmokeCloud(int smokeCloudQuantity)
    {

        for (int i = 0; i < smokeCloudQuantity; i++)
        {
            GameObject smokeCloud = Instantiate(_smokeCloud);
            smokeCloud.transform.parent = _smokeCloudContainer.transform;
            smokeCloud.SetActive(false);
            _smokeCloudPool.Add(smokeCloud);
        }
        return _smokeCloudPool;
    }

    public GameObject RequestSmokeCloud()
    {
        foreach (var smokeCloud in _smokeCloudPool)
        {
            if (smokeCloud.activeInHierarchy == false)
            {
                smokeCloud.SetActive(true);
                return smokeCloud;
            }
        }
        GameObject newSmokeCloud = Instantiate(_smokeCloud);
        newSmokeCloud.transform.parent = _smokeCloudContainer.transform;
        _smokeCloudPool.Add(newSmokeCloud);

        return newSmokeCloud;
    }
}

