using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private int _zombieDeathCount;
    public override void Init()
    {
        base.Init(); // Turns this class into a singleton
    }

    void Start()
    {
        SpawnManager.Instance.SpawnEnemies();
    }

    public void ZombieDeathCount(bool resetCount)
    {
        _zombieDeathCount++;
    }


}
