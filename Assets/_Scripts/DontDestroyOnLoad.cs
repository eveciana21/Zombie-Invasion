using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DontDestroyOnLoad : MonoBehaviour
{
    [SerializeField] private PlayableDirector _introCutScene;

    private bool _canPlayCutScene = true;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        if (_canPlayCutScene)
        {
            _introCutScene.Play();
            _canPlayCutScene = false;
        }
    }
}
