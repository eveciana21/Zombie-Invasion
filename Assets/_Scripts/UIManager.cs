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
    [SerializeField] private int _score;

    [SerializeField] private GameObject _proveYourWorthText;

    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private GameObject _dialogueBox;
    private float _textSpeed = 0.06f;

    private bool _confirmedPlayerNotAZombie;
    private bool _giftGiven;


    IEnumerator DialogueTextRoutine(string dialogue)
    {
        _dialogueText.text = " ";
        for (int i = 0; i <= dialogue.Length; i++)
        {
            _dialogueText.text = dialogue.Substring(0, i);
            yield return new WaitForSeconds(_textSpeed);
        }
    }
    IEnumerator SecondaryDialogueRoutine(string dialogue)
    {
        _dialogueText.text = " ";
        for (int i = 0; i <= dialogue.Length; i++)
        {
            _dialogueText.text = dialogue.Substring(0, i);
            yield return new WaitForSeconds(_textSpeed);
        }
        _giftGiven = true;
    }
    IEnumerator TertiaryDialogueRoutine(string dialogue)
    {
        _dialogueText.text = " ";
        for (int i = 0; i <= dialogue.Length; i++)
        {
            _dialogueText.text = dialogue.Substring(0, i);
            yield return new WaitForSeconds(_textSpeed);
        }
    }

    public void DialogueText(bool nearPlayer, string dialogue, string secondaryDialogue, string tertiaryDialogue)
    {
        if (nearPlayer)
        {
            _dialogueBox.SetActive(true);

            if (_confirmedPlayerNotAZombie == true)
            {
                if (_giftGiven == false)
                {
                    StartCoroutine(SecondaryDialogueRoutine(secondaryDialogue));
                }

                else
                {
                    StartCoroutine(TertiaryDialogueRoutine(tertiaryDialogue));
                }
            }
            else
            {
                StartCoroutine(DialogueTextRoutine(dialogue));
                StopCoroutine("ProveYourWorthRoutine");
                _proveYourWorthText.SetActive(false);
            }
        }
        else
        {
            _dialogueBox.SetActive(false);
            StopCoroutine("DialogueTextRoutine");
            StopCoroutine("SecondaryDialogueRoutine");
            StopCoroutine("TertiaryDialogueRoutine");

            if (_confirmedPlayerNotAZombie == false)
            {
                StopCoroutine("ProveYourWorthRoutine");
                StartCoroutine(ProveYourWorthRoutine());
            }
        }
    }

    IEnumerator ProveYourWorthRoutine()
    {
        yield return new WaitForSeconds(1);
        _proveYourWorthText.SetActive(true);
        yield return new WaitForSeconds(3);
        _proveYourWorthText.SetActive(false);
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

        if (score == 100)
        {
            _confirmedPlayerNotAZombie = true;
        }
    }


}
