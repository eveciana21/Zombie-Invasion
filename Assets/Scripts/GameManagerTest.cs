using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerTest : MonoBehaviour
{   
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameObject.FindObjectOfType<PlayFromMenu>().ResetMainMenuLoad();
            SceneManager.LoadScene("MainMenuScene");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("PlayOnLoadScene");
            Debug.Log("Scene Reloaded");
        }
        
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("PlayOnLoadScene");
    }
}
