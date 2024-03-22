using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    void OnEnable()
    {
        StopCoroutine("MuzzleFlashRoutine");
        StartCoroutine("MuzzleFlashRoutine");
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    IEnumerator MuzzleFlashRoutine()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
