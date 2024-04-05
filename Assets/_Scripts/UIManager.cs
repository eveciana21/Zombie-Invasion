using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{

    [SerializeField] private TextMeshProUGUI _ammoCount;
    [SerializeField] private TextMeshProUGUI _health;

    [SerializeField] private GameObject _heart, _heartBroken;
    [SerializeField] private GameObject _skull;

    [SerializeField] private TextMeshProUGUI _scoreText;


    public override void Init()
    {
        base.Init(); //Turns this class into a singleton
    }


    public void AmmoCount(int currentAmmo)
    {
        _ammoCount.text = currentAmmo.ToString();
    }

    public void HealthRemaining(int health)
    {
        _health.text = health.ToString();

        if (health <= 25)
        {
            _heartBroken.SetActive(true);
            _heart.SetActive(false);

            if (health <= 0)
            {
                _heartBroken.SetActive(false);
                _heart.SetActive(false);
                _skull.SetActive(true);
            }
        }
        if (health > 25)
        {
            _heartBroken.SetActive(false);
            _heart.SetActive(true);
        }
    }

    public void Score(int score)
    {
        _scoreText.text = score.ToString();
    }


}
