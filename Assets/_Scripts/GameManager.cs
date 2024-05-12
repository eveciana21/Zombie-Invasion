using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Playables;

public class GameManager : MonoSingleton<GameManager>
{
    private StarterAssetsInputs _input;
    private Player _player;

    [Header("Slider")]

    [SerializeField] private GameObject _sensitivitySlider;
    [SerializeField] private GameObject _dayNightSlider;

    [Header("TimeLine")]

    [SerializeField] private PlayableDirector _mainToOptionsTimeline;
    [SerializeField] private PlayableDirector _optionsToMainTimeline;
    [SerializeField] private PlayableDirector _optionsToControlsTimeline;
    [SerializeField] private PlayableDirector _controlsToMainMenuTimeline;
    [SerializeField] private PlayableDirector _controlsToOptionsTimeline;

    [Space]

    [SerializeField] private PostProcessVolume _postProcessVolume;
    private ChromaticAberration _chromaticAberration;
    private Vignette _vignette;

    [SerializeField] private GameObject _quitGameButtons;
    [SerializeField] private GameObject _deathMenu;
    [SerializeField] private GameObject _skullsParticle;
    [SerializeField] private GameObject _reticle;

    [SerializeField] private GameObject _optionsScreen;
    [SerializeField] private GameObject _mainMenuScreen;
    [SerializeField] private GameObject _controlsScreen;

    private bool _gameStarted;
    private bool _playerDead;

    public override void Init()
    {
        base.Init(); // Turns this class into a singleton
    }

    void Start()
    {
        GameObject player = GameObject.Find("PlayerCapsule");
        if (player != null)
        {
            _input = player.GetComponent<StarterAssetsInputs>();
            if (_input == null)
                Debug.LogError("Input is NULL");
            _player = player.GetComponent<Player>();
            if (_player == null)
                Debug.LogError("Player is NULL");
        }

        if (_postProcessVolume == null)
            Debug.LogError("Post Process Volume is NULL");
        else
            _postProcessVolume.profile.TryGetSettings(out _vignette);
        _postProcessVolume.profile.TryGetSettings(out _chromaticAberration);

        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.buildIndex == 1)
        {
            if (_input != null)
            {
                _input.SetCursorVisible(false);
                _gameStarted = true;
            }
        }
        if (_gameStarted && !_playerDead)
        {
            SpawnManager.Instance.SpawnEnemies();
        }

        VignetteIntensity(0.3f);

        UIManager.Instance.LoadSensitivitySetting();
        UIManager.Instance.LoadDayNightSetting();
    }

    private void Update()
    {
        if (_input != null && !_playerDead && _input.escapeKey)
        {
            PauseGame();
            VignetteIntensity(0.5f);
            _input.escapeKey = false;
        }

        if (_playerDead)
        {
            FadeVignette(0.9f);
        }
    }
    public void IncreaseChromaticAberration(float targetIntensity, float speed)
    {
        if (_chromaticAberration != null)
        {
            float currentIntensity = _chromaticAberration.intensity.value;
            float newIntensity = Mathf.Lerp(currentIntensity, targetIntensity, speed * Time.deltaTime);
            _chromaticAberration.intensity.value = newIntensity;
        }
    }

    private void VignetteIntensity(float intensity)
    {
        // Set the vignette intensity of the post processing volume
        if (_vignette != null)
        {
            _vignette.intensity.value = intensity;
        }
    }

    public void FadeVignette(float intensity)
    {
        _vignette.color.value = Color.red;

        float newIntensity = Mathf.Lerp(_vignette.intensity.value, intensity, 0.2f * Time.deltaTime);
        _vignette.intensity.value = newIntensity;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);

        Time.timeScale = 1;

        if (_input != null)
        {
            _input.SetCursorVisible(false);
        }
        _gameStarted = true;

    }

    public void PauseGame()
    {
        Time.timeScale = 0;

        if (_quitGameButtons != null)
        {
            _quitGameButtons.SetActive(true);
        }
        if (_sensitivitySlider != null)
        {
            _sensitivitySlider.SetActive(true);
        }
        if (_dayNightSlider != null)
        {
            _dayNightSlider.SetActive(true);
        }
        if (_input != null)
        {
            _input.SetCursorVisible(true);
        }
        if (_reticle != null)
        {
            _reticle.SetActive(false);
        }
    }

    public void ContinueGame()
    {
        Time.timeScale = 1;

        if (_input != null)
        {
            _input.SetCursorVisible(false);
        }
        if (_reticle != null)
        {
            _reticle.SetActive(true);
        }

        VignetteIntensity(0.2f);
    }

    public void MainMenu()
    {
        _gameStarted = false;
        Time.timeScale = 1;

        SceneManager.LoadScene(0);

        if (_input != null)
        {
            _input.SetCursorVisible(true);
        }
    }

    public void MainMenuToOptions()
    {
        _mainToOptionsTimeline.Play();
        StartCoroutine(MainMenuToOptionsScreenDelay());
    }

    IEnumerator MainMenuToOptionsScreenDelay()
    {
        _mainMenuScreen.SetActive(false);
        yield return new WaitForSeconds(2f);
        _optionsScreen.SetActive(true);
        _optionsToMainTimeline.Stop();
        _controlsToMainMenuTimeline.Stop();
    }

    public void OptionsToMainMenu()
    {
        _optionsToMainTimeline.Play();
        StartCoroutine(OptionsToMainMenuDelay());
    }

    IEnumerator OptionsToMainMenuDelay()
    {
        _optionsScreen.SetActive(false);
        yield return new WaitForSeconds(2f);
        _mainMenuScreen.SetActive(true);
        _mainToOptionsTimeline.Stop();
    }

    public void OptionsToControls()
    {
        _optionsToControlsTimeline.Play();
        StartCoroutine(OptionsToControlsDelay());
    }
    IEnumerator OptionsToControlsDelay()
    {
        _optionsScreen.SetActive(false);
        yield return new WaitForSeconds(2f);
        _controlsScreen.SetActive(true);
        _mainToOptionsTimeline.Stop();
        _controlsToOptionsTimeline.Stop();
    }

    public void ControlsToMainMenu()
    {
        _controlsToMainMenuTimeline.Play();
        StartCoroutine(ControlsToMainMenuDelay());
    }

    IEnumerator ControlsToMainMenuDelay()
    {
        _controlsScreen.SetActive(false);
        yield return new WaitForSeconds(2);
        _mainMenuScreen.SetActive(true);
        _optionsToControlsTimeline.Stop();
    }

    public void ControlsToOptions()
    {
        _controlsToOptionsTimeline.Play();
        StartCoroutine(ControlsToOptionsDelay());
    }

    IEnumerator ControlsToOptionsDelay()
    {
        _controlsScreen.SetActive(false);
        yield return new WaitForSeconds(2);
        _optionsScreen.SetActive(true);
        _optionsToControlsTimeline.Stop();
    }


    public void PlayerDeadMenu()
    {
        if (_player != null)
        {
            _playerDead = true;
            _player.IsPlayerAlive(false);
            _skullsParticle.SetActive(true);
            AudioManager.Instance.SFX(1);
            StartCoroutine(DeathMenuDelay());
        }
    }

    IEnumerator DeathMenuDelay()
    {
        if (_input != null)
        {
            yield return new WaitForSeconds(4);
            _input.SetCursorVisible(true);
            _deathMenu.SetActive(true);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void YouWinScreen()
    {
        Debug.Log("You Win");
    }
}
