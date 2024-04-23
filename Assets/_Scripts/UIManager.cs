using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    [Header("Ammo")]
    [SerializeField] private TextMeshProUGUI _ammoCount;
    [SerializeField] private TextMeshProUGUI _ammoSubCount;

    [Header("Health")]
    [SerializeField] private TextMeshProUGUI _health;
    [SerializeField] private GameObject _heart, _heartBroken;
    [SerializeField] private GameObject _skull;

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private int _score;

    [Header("Text")]
    [SerializeField] private GameObject _proveYourWorthText;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private GameObject _dialogueBox;
    private float _textSpeed = 0.06f;

    [Space]

    [SerializeField] private Slider _sprintSlider;
    [SerializeField] private Image _reticle;

    private Image _sliderFillColor;
    private Image _sliderBackgroundColor;
    private float _fadeTimer = 0;
    private float _fadeSpeed = 1f;
    private bool _fadingOut;
    private bool _isPlayerAlive = true;

    private Dictionary<string, int> _npcKillThreshold = new Dictionary<string, int>()
    {
        {"NPC1", 2 },
        {"NPC2", 4 },
        {"NPC3", 6 },
        {"NPC4", 8 }
    };
    private Dictionary<string, bool> _confirmedPlayerNotZombie = new Dictionary<string, bool>()
    {
        {"NPC1", false },
        {"NPC2", false },
        {"NPC3", false },
        {"NPC4", false }
    };
    private Dictionary<string, bool> _giftGivenDict = new Dictionary<string, bool>()
    {
        {"NPC1", false },
        {"NPC2", false },
        {"NPC3", false },
        {"NPC4", false }
    };
    private Dictionary<string, bool> _canGiveGiftDict = new Dictionary<string, bool>()
    {
        {"NPC1", false },
        {"NPC2", false },
        {"NPC3", false },
        {"NPC4", false }
    };

    private void Start()
    {
        if (_sprintSlider != null)
        {
            SprintSlider(100f);
            _sprintSlider.gameObject.SetActive(false);

            _sliderFillColor = _sprintSlider.fillRect.GetComponent<Image>();
            _sliderBackgroundColor = _sprintSlider.GetComponentInChildren<Image>();
        }

    }

    private void Update()
    {
        SliderFade();

        if (!_isPlayerAlive)
        {
            _proveYourWorthText.SetActive(false);
            _dialogueBox.SetActive(false);
            _reticle.enabled = false;
        }
    }

    private void SliderFade()
    {
        if (_fadingOut)
        {
            _fadeTimer += Time.deltaTime;

            float alpha = Mathf.Clamp01(1f - _fadeTimer * _fadeSpeed);

            SetSliderAlpha(alpha);

            if (_fadeTimer >= 1f)
            {
                _sprintSlider.gameObject.SetActive(false);
                _fadeTimer = 0;
                _fadingOut = false;
            }
        }
    }

    private void SetSliderAlpha(float alpha)
    {
        if (_sliderFillColor != null && _sliderBackgroundColor != null)
        {
            Color fillColor = _sliderFillColor.color;
            Color backgroundColor = _sliderBackgroundColor.color;

            // Set the alpha value for both colors
            fillColor.a = alpha;
            backgroundColor.a = alpha;

            // Set the colors back to the fill and background
            _sliderFillColor.color = fillColor;
            _sliderBackgroundColor.color = backgroundColor;
        }
        else
        {
            Debug.LogError("Slider Fill And/Or Slider Background is NULL");
        }
    }

    private void FadeOutSlider()
    {
        if (!_fadingOut)
        {
            _fadingOut = true;
        }
    }

    public void SliderInUse()
    {
        if (_fadingOut)
        {
            _fadingOut = false;
        }
        SetSliderAlpha(1f);
        _sprintSlider.gameObject.SetActive(true);
    }

    IEnumerator DialogueTextRoutine(string dialogue)
    {
        _dialogueText.text = " ";
        for (int i = 0; i <= dialogue.Length; i++)
        {
            _dialogueText.text = dialogue.Substring(0, i);
            yield return new WaitForSeconds(_textSpeed);
        }
    }
    IEnumerator SecondaryDialogueRoutine(string dialogue, string npcName)
    {
        yield return DialogueTextRoutine(dialogue);
        _canGiveGiftDict[npcName] = true;

    }
    IEnumerator TertiaryDialogueRoutine(string dialogue)
    {
        yield return DialogueTextRoutine(dialogue);
    }

    public void DialogueText(bool nearPlayer, string npcName, string dialogue, string secondaryDialogue, string tertiaryDialogue)
    {

        if (nearPlayer)
        {
            _dialogueBox.SetActive(true);

            if (_confirmedPlayerNotZombie[npcName] == true)
            {
                if (!_giftGivenDict[npcName])
                {
                    StartCoroutine(SecondaryDialogueRoutine(secondaryDialogue, npcName));
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
            StopAllCoroutines();

            if (_confirmedPlayerNotZombie[npcName] == false)
            {
                StopCoroutine("ProveYourWorthRoutine");
                StartCoroutine(ProveYourWorthRoutine());
            }
        }
    }


    IEnumerator ProveYourWorthRoutine()
    {
        yield return new WaitForSeconds(0.5f);
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
        if (_isPlayerAlive)
        {
            _health.text = health.ToString();
        }
        else
        {
            _health.gameObject.SetActive(false);
        }


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

    public void Score(int score, int killCount)
    {
        _scoreText.text = score.ToString();
        foreach (var npc in _npcKillThreshold.Keys)
        {
            if (killCount >= _npcKillThreshold[npc])
            {
                if (!_giftGivenDict[npc])
                {
                    _confirmedPlayerNotZombie[npc] = true;
                    Debug.Log(npc + " Kill Count Confirmed!");
                }
            }
        }
    }
    public void ActivateGift(string npcName, GameObject gift, GameObject potionImage)
    {
        if (!_giftGivenDict[npcName] && _canGiveGiftDict[npcName])
        {
            if (gift != null)
            {
                Debug.Log("Gift Given");
                _giftGivenDict[npcName] = true;
                gift.SetActive(true);
                gift.transform.parent = null;
            }
            if (potionImage != null)
            {
                potionImage.SetActive(true);
            }
        }
    }

    public void SprintSlider(float sprintPercentage)
    {
        _sprintSlider.value = sprintPercentage;

        if (sprintPercentage >= 99)
        {
            FadeOutSlider();
        }
        else
        {
            SliderInUse();
        }
        _sprintSlider.maxValue = 100f;
        _sprintSlider.minValue = 0f;
    }

    public void DisableSlider()
    {
        if (_sprintSlider != null)
        {
            _sprintSlider.gameObject.SetActive(false);
        }
    }

    public void IsPlayerAlive(bool isPlayerAlive)
    {
        _isPlayerAlive = isPlayerAlive;
    }
}
