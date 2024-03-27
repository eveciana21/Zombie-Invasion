using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 10;

    private void OnEnable()
    {
        StopCoroutine("DisableBullet");
        StartCoroutine("DisableBullet");
    }

    void Update()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }

    IEnumerator DisableBullet()
    {
        yield return new WaitForSeconds(2);
        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Hide();
        }
    }
}
