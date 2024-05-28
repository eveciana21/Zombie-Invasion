using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using System;
using UnityEngine.UI;
using UnityEngine.Playables;

public class UIManager : MonoSingleton<UIManager>
{
    [Header("Ammo")]
    [SerializeField] private GameObject _ammoGO;
    [SerializeField] private TextMeshProUGUI _ammoCount;
    [SerializeField] private TextMeshProUGUI _ammoSubCount;

    [Header("Health")]
    [SerializeField] private GameObject _healthGO;
    [SerializeField] private TextMeshProUGUI _health;
    [SerializeField] private GameObject _heart, _heartBroken;
    [SerializeField] private GameObject _screenBlood;

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private GameObject _scoreGO;

    [Header("Text")]
    [SerializeField] private GameObject _proveYourWorthText;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private TextMeshProUGUI _introDialogueText;
    [SerializeField] private GameObject _introDialogBox;
    [SerializeField] private GameObject _okayButton;
    [SerializeField] private GameObject _dialogueBox;
    [SerializeField] private GameObject _getToTheChopperDialogueBox;
    [SerializeField] private TextMeshProUGUI _getToTheChopperText;
    [SerializeField] private float _textSpeed = 0.06f;

    [SerializeField] private int _startMinutes;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private GameObject _youAreWorthyText;
    private bool _timerActive;
    private float _currentTime;

    [Header("Sliders")]

    [SerializeField] private Slider _sprintSlider;
    [SerializeField] private Slider _dayNightSlider;
    [SerializeField] private Slider _sensitivitySlider;
    [SerializeField] private TMP_Text _sensitivityText;

    [Header("TimeLine")]

    [SerializeField] private PlayableDirector _endSceneTimeline;
    [SerializeField] private PlayableDirector _helicopterEnterSceneTimeline;
    [SerializeField] private PlayableDirector _introSceneTimeline;
    [SerializeField] private PlayableDirector _playerRopeTimeline;

    [Header("Other")]

    [SerializeField] private Image _reticle;
    [SerializeField] private GameObject _miniMap;
    [SerializeField] private GameObject _potions;

    [SerializeField] private GameObject _endGameTrigger;
    [SerializeField] private GameObject _helicopterIcon;
    [SerializeField] private GameObject _helicopter;
    [SerializeField] private GameObject _helicopterNPCs;
    [SerializeField] private GameObject _poisonGas;


    private Image _sliderFillColor;
    private Image _sliderBackgroundColor;
    private float _fadeTimer = 0;
    private float _fadeSpeed = 1f;
    private bool _fadingOut;
    private bool _isPlayerAlive = true;
    private bool _interactedWithPlayer;
    private bool _allPotionsCollected;
    private bool _lastHoorah;

    private int _potionCount;
    private bool _endGame;
    private bool _isOnLastScene;

    private Color _originalTimerTextColor;

    private Dictionary<string, int> _npcKillThreshold = new Dictionary<string, int>()
    {
        {"NPC1", 10 },
        {"NPC2", 20 },
        {"NPC3", 30 },
        {"NPC4", 40 }
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
    public override void Init()
    {
        base.Init(); //Turns this class into a singleton
    }

    private void Start()
    {
        if (_sprintSlider != null)
        {
            SprintSlider(100f);
            _sprintSlider.gameObject.SetActive(false);

            _sliderFillColor = _sprintSlider.fillRect.GetComponent<Image>();
            _sliderBackgroundColor = _sprintSlider.GetComponentInChildren<Image>();
        }

        _currentTime = _startMinutes * 60;

        if (_timerText != null)
        {
            _originalTimerTextColor = _timerText.color;
        }
    }

    private void Update()
    {
        SliderFade();
        Timer();
    }

    private void Timer()
    {
        if (_timerText != null && _timerActive)
        {
            _currentTime = _currentTime - Time.deltaTime;

            if (_currentTime <= 0)
            {
                _timerText.gameObject.SetActive(false);
                IsPlayerAlive(false);
                _timerActive = false;
            }

            if (_currentTime < 11 && _currentTime > 0)
            {
                if (Mathf.FloorToInt(_currentTime) != Mathf.FloorToInt(_currentTime + Time.deltaTime))
                {
                    AudioManager.Instance.SFX(3);
                    _timerText.color = Color.red;
                }
            }
            else
            {
                _timerText.color = _originalTimerTextColor;
            }

            if (_currentTime <= 4)
            {
                _poisonGas.SetActive(true);
            }

            LastHoorah();

            TimeSpan time = TimeSpan.FromSeconds(_currentTime);
            _timerText.text = time.Minutes.ToString("00") + " : " + time.Seconds.ToString("00");
        }

        if (_allPotionsCollected)
        {
            _currentTime = 60;
            _lastHoorah = true;
            _helicopterNPCs.SetActive(false);
            _helicopterEnterSceneTimeline.Play();
            _allPotionsCollected = false;
        }
    }

    private void LastHoorah() //ending scene 
    {
        if (_lastHoorah && _currentTime < 90 && _currentTime > 25)
        {
            _helicopterIcon.SetActive(true);
            _isOnLastScene = true;
        }
        if (_lastHoorah && _currentTime <= 25)
        {
            _endGameTrigger.SetActive(true);
        }
    }


    public void ActivateTimer(bool value)
    {
        _timerActive = value;
    }

    public void CanEndGame(bool value)
    {
        if (_lastHoorah)
        {
            _endGame = value;
            if (value == true)
            {
                DisableUI(false);
                _helicopterNPCs.SetActive(true);
                _endSceneTimeline.Play();
                _timerActive = false;
            }
        }
    }


    private void AddTime(float secondsToAdd)
    {
        _currentTime += secondsToAdd;
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
        if (_isPlayerAlive)
        {
            _sprintSlider.gameObject.SetActive(true);
        }
        else
        {
            _sprintSlider.gameObject.SetActive(false);
        }
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

    public void DialogueText(bool nearPlayer, string npcName, string dialogue, string secondaryDialogue, string tertiaryDialogue, string lastDialogue)
    {
        if (_lastHoorah == true) // <-- test this!!
        {
            _dialogueBox.SetActive(false);
            return;
        }

        _dialogueBox.SetActive(nearPlayer);

        if (!_isOnLastScene)
        {
            if (nearPlayer)
            {
                _proveYourWorthText.SetActive(false);
                StopCoroutine("ProveYourWorthRoutine");

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
                    _interactedWithPlayer = true;
                }
                else
                {
                    StartCoroutine(DialogueTextRoutine(dialogue));
                    _interactedWithPlayer = false;
                }
            }
            else
            {
                StopAllCoroutines();

                if (_confirmedPlayerNotZombie[npcName] == false && _interactedWithPlayer == false)
                {
                    StartCoroutine(ProveYourWorthRoutine());
                    _interactedWithPlayer = true;
                }
                else
                {
                    _proveYourWorthText.SetActive(false);
                }
            }
        }
    }

    IEnumerator ProveYourWorthRoutine()
    {
        _proveYourWorthText.SetActive(true);
        yield return new WaitForSeconds(3);
        _proveYourWorthText.SetActive(false);
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
            health = 0;
            _health.gameObject.SetActive(false);
        }

        if (health <= 30)
        {
            _heartBroken.SetActive(true);
            _heart.SetActive(false);
        }
        if (health > 30)
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
                }
            }
        }
    }

    public void ActivateGift(string npcName, GameObject gift, GameObject potionImage, GameObject minimapPotionIcon)
    {
        if (!_giftGivenDict[npcName] && _canGiveGiftDict[npcName])
        {
            if (gift != null)
            {
                _giftGivenDict[npcName] = true;
                gift.SetActive(true);
                gift.transform.parent = null;
                AddTime(15);
            }
            if (potionImage != null)
            {
                potionImage.SetActive(true);
                _potionCount++;
                if (_potionCount == 4)
                {
                    SpawnManager.Instance.SpawnBoss();
                    StartCoroutine(PlayHelicopterVocal());
                    _allPotionsCollected = true;
                }
            }
            if (minimapPotionIcon != null)
            {
                minimapPotionIcon.SetActive(false);
            }
        }
    }

    IEnumerator PlayHelicopterVocal()
    {
        yield return new WaitForSeconds(5f);
        AudioManager.Instance.Vocals(0);
    }

    public void WorthyText()
    {
        StartCoroutine(YouAreWorthyRoutine());
    }

    IEnumerator YouAreWorthyRoutine()
    {
        int count = 0;
        while (count <= 3)
        {
            _youAreWorthyText.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _youAreWorthyText.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            count++;
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

    public void SensitivitySlider(float rotationSpeed)
    {
        _sensitivitySlider.maxValue = 1f;
        _sensitivitySlider.minValue = 0.1f;

        float roundedValue = Mathf.Round(rotationSpeed * 10) / 10;

        _sensitivitySlider.value = roundedValue;

        _sensitivityText.text = roundedValue.ToString("F1");

        PlayerPrefs.SetFloat("RotationSpeed", roundedValue);
        PlayerPrefs.Save();
    }

    public void LoadSensitivitySetting()
    {
        if (PlayerPrefs.HasKey("RotationSpeed"))
        {
            float rotationSpeed = PlayerPrefs.GetFloat("RotationSpeed");

            SensitivitySlider(rotationSpeed);
        }
    }

    public void DayNightSlider(float daylightAmount)
    {
        _dayNightSlider.maxValue = 1.25f;
        _dayNightSlider.minValue = 0f;

        _dayNightSlider.value = daylightAmount;

        PlayerPrefs.SetFloat("DayLight", daylightAmount);
        PlayerPrefs.Save();
    }

    public void LoadDayNightSetting()
    {
        if (PlayerPrefs.HasKey("DayLight"))
        {
            float dayNightAmount = PlayerPrefs.GetFloat("DayLight");

            DayNightSlider(dayNightAmount);
        }
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
        if (!_isPlayerAlive)
        {
            DisableUI(false);
            GameManager.Instance.PlayerDeadMenu();
        }
    }

    private void DisableUI(bool value)
    {
        _ammoGO.SetActive(value);
        _healthGO.SetActive(value);
        _sprintSlider.gameObject.SetActive(value);
        _dialogueBox.SetActive(value);
        _proveYourWorthText.SetActive(value);
        _reticle.enabled = value;
        _timerText.gameObject.SetActive(value);
        _timerActive = value;
        _miniMap.SetActive(value);

        if (_endGame)
        {
            _potions.SetActive(value);
            _scoreGO.SetActive(value);
            _screenBlood.SetActive(value);
        }
    }

    public void IntroDialogueText()
    {
        if (_introDialogBox != null)
        {
            StartCoroutine(IntroDialogText(_introDialogueText.text));
        }
    }

    IEnumerator IntroDialogText(string dialogue)
    {
        _introDialogueText.text = " ";
        for (int i = 0; i <= dialogue.Length; i++)
        {
            _introDialogueText.text = dialogue.Substring(0, i);
            yield return new WaitForSeconds(_textSpeed);
        }
        _okayButton.SetActive(true);
    }
}