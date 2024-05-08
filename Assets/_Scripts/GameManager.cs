using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets;
using UnityEngine.Rendering.PostProcessing;

public class GameManager : MonoSingleton<GameManager>
{
    private StarterAssetsInputs _input;
    private Player _player;

    [SerializeField] private PostProcessVolume _postProcessVolume;
    private ChromaticAberration _chromaticAberration;
    private Vignette _vignette;

    [SerializeField] private GameObject _quitGameButtons;
    [SerializeField] private GameObject _deathMenu;
    [SerializeField] private GameObject _skullsParticle;
    [SerializeField] private GameObject _reticle;

    private bool _gameStarted;
    private bool _playerDead;


    public override void Init()
    {
        base.Init(); // Turns this class into a singleton
    }

    void Start()
    {
        //Player player = FindObjectOfType<Player>();
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
        if (_input != null)
        {
            _input.SetCursorVisible(true);
        }
    }

    public void ContinueGame()
    {
        Time.timeScale = 1;

        if (_input != null)
        {
            _input.SetCursorVisible(false);
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

    public void PlayerDeadMenu()
    {
        _playerDead = true;
        _player.IsPlayerAlive(false);
        _skullsParticle.SetActive(true);
        AudioManager.Instance.SFX(1);
        StartCoroutine(DeathMenuDelay());
    }

    IEnumerator DeathMenuDelay()
    {
        yield return new WaitForSeconds(4);
        _input.SetCursorVisible(true);
        _deathMenu.SetActive(true);
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
