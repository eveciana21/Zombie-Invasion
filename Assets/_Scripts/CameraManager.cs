using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets;

public class CameraManager : MonoSingleton<CameraManager>
{
    [SerializeField] private GameObject _assaultRifle;
    private StarterAssetsInputs _input;

    public override void Init()
    {
        base.Init(); //Turns this class into a singleton
    }


    private void Start()
    {
        GameObject player = GameObject.Find("PlayerCapsule");
        if (player != null)
        {
            _input = player.GetComponent<StarterAssetsInputs>();
            if (_input == null)
                Debug.LogError("Input is NULL");
        }
    }

    public void EnableAssaultRifle() // called on signal emitter in timeline
    {
        if (_assaultRifle != null)
        {
            _assaultRifle.SetActive(true);
        }
    }

    public void DisableAssaultRifle() // called on signal emitter in timeline
    {
        if (_assaultRifle != null)
        {
            _assaultRifle.SetActive(false);
        }
    }

    public void MainMenu()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene(0);

        if (_input != null)
        {
            _input.SetCursorVisible(true);
        }
    }

    public void DisplayCursor()
    {
        _input.SetCursorVisible(true);
    }
}
