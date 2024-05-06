using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterPropeller : MonoBehaviour
{
    [SerializeField] private Transform _propellerCenter;
    private float _rotateSpeed = 500f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(_propellerCenter.position, Vector3.up, _rotateSpeed * Time.deltaTime);
    }
}
