using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform;
    private float _xOffset, _yOffset, _zOffset;

    void Start()
    {
        _xOffset = transform.position.x - _targetTransform.position.x;
        _yOffset = transform.position.y - _targetTransform.position.y;
        _zOffset = transform.position.z - _targetTransform.position.z;
    }

    void Update()
    {
        gameObject.transform.position = new Vector3(
            _targetTransform.position.x + _xOffset,
            _targetTransform.position.y + _yOffset,
            _targetTransform.position.z + _zOffset);
    }
}
