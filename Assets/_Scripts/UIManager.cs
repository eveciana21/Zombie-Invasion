using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{

    [SerializeField] private TextMeshProUGUI _ammoCount;
    [SerializeField] private TextMeshProUGUI _ammoSubCount;
    [SerializeField] private TextMeshProUGUI _health;

    [SerializeField] private GameObject _heart, _heartBroken;
    [SerializeField] private GameObject _skull;

    [SerializeField] private TextMeshProUGUI _scoreText;

    [SerializeField] private TextMeshProUGUI _dialogText;
    [SerializeField] private GameObject _dialogBox;
    private float _textSpeed = 0.06f;
    private string _fullText;


    private void Start()
    {
        _fullText = _dialogText.text;
        _dialogText.text = " ";
    }

    IEnumerator DialogTextRoutine()
    {
        for (int i = 0; i <= _fullText.Length; i++)
        {
            _dialogText.text = _fullText.Substring(0, i);
            yield return new WaitForSeconds(_textSpeed);
        }
    }


    public void DialogText(int npcID, bool nearPlayer)
    {
        if (nearPlayer)
        {
            if (npcID == 0)
            {
                _dialogBox.SetActive(true);
                StartCoroutine("DialogTextRoutine");
            }
        }
        else
        {
            _dialogBox.SetActive(false);
            StopCoroutine("DialogTextRoutine");
        }

    }

    public override void Init()
    {
        base.Init(); //Turns this class into a singleton
    }

    public void AmmoCount(int currentAmmo)
    {
        _ammoCount.text = currentAmmo.ToString();
    }

    public void AmmoSubCount(int currentAmmo)
    {
        _ammoSubCount.text = currentAmmo.ToString();
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
