using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public override void Init()
    {
        base.Init(); // Turns this class into a singleton
    }

    void Start()
    {
        SpawnManager.Instance.SpawnEnemies();
    }


}
