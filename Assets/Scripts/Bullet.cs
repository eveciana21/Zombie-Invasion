using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 10;

    private void OnEnable()
    {
        Invoke("Hide", 1);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
