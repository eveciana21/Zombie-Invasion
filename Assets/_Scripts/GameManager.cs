using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets;


public class GameManager : MonoSingleton<GameManager>
{
    private StarterAssetsInputs _input;

    [SerializeField] private GameObject _quitGameButtons;
    private bool _gameStarted;
    private bool _menuOnScreen;

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
        }


        if (_gameStarted)
        {
            SpawnManager.Instance.SpawnEnemies();
        }

        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.buildIndex == 1)
        {
            if (_input != null)
            {
                _input.SetCursorVisible(false);
            }
        }
    }

    private void Update()
    {
        if (_input != null && _input.escapeKey)
        {
            PauseGame();
            _input.escapeKey = false;
        }
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

        if (_quitGameButtons != null)
        {
            _quitGameButtons.SetActive(false);
        }
        if (_input != null)
        {
            _input.SetCursorVisible(false);
        }
        Debug.Log("Continue Game");
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
        Debug.Log("Main Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
