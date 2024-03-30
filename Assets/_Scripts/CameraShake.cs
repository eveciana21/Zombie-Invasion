using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoSingleton<CameraShake>
{
    public override void Init()
    {
        base.Init(); // turns this class into a singleton
    }

    public void ShakeCamera()
    {
        StartCoroutine(ShakeScreen());
    }


    IEnumerator ShakeScreen()
    {
        Vector3 originalPos = transform.position;
        float screenShakeDuration = Time.time + 0.2f;

        while (screenShakeDuration > Time.time)
        {
            float x = Random.Range(-1f, 1f);
            float y = Random.Range(-1f, 1f);

            transform.position = new Vector3(transform.position.x + x, transform.position.y + y, 0);

            yield return new WaitForEndOfFrame();
        }

        transform.position = originalPos;
    }
}
