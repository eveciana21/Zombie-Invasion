using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class PlayFromMenu : MonoBehaviour
{
    [SerializeField] private PlayableDirector _director;
    [SerializeField] private bool _isLoadingFromMainMenu;

    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Movie");

        if (objs.Length > 1)
            Destroy(this.gameObject);
        else
        {
            DontDestroyOnLoad(this.gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _director = GameObject.FindAnyObjectByType<PlayableDirector>();
        if (_director == null)
            return;

        if (_isLoadingFromMainMenu)
        {
            Debug.Log("Playing");
            _director.Play();
            _isLoadingFromMainMenu = false;
        }
    }

    public void ResetMainMenuLoad()
    {
        _isLoadingFromMainMenu = true;
    }
}
