using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{

    [SerializeField] private TextMeshProUGUI _ammoCount;
    [SerializeField] private TextMeshProUGUI _health;

    public override void Init()
    {
        base.Init(); //Turns this class into a singleton
    }


    public void AmmoCount(int currentAmmo)
    {
        _ammoCount.text = "Ammo: " + currentAmmo.ToString();
    }

    public void HealthRemaining(int health)
    {
        _health.text = "Health: " + health.ToString();
    }


}
