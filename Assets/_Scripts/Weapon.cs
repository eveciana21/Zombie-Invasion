using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Recoil")]

    [SerializeField] private float _recoilAmount = 0.1f;
    [SerializeField] private float _recoilSpeed = 5f;
    [SerializeField] private float _maxRecoil = 0.5f;

    [SerializeField] private RectTransform _reticleTransform;
    [SerializeField] private float _reticleSensitivity = 500;
    private Vector3 _originalPos;
    private Vector3 _originalReticlePos;
    private Vector3 _recoil;

    [Header("Camera Shake")]

    [SerializeField] private Camera _camera;
    [SerializeField] private float _screenShakeAmount = 0.1f;
    [SerializeField] private float _screenShakeDuration = 0.1f;

    void Start()
    {
        _originalPos = transform.localPosition;
        _originalReticlePos = _reticleTransform.anchoredPosition;
    }

    void Update()
    {
        _recoil = Vector3.Lerp(_recoil, Vector3.zero, _recoilSpeed * Time.deltaTime); //slowly reduce recoil effect over time
        transform.localPosition = _originalPos + _recoil; //updates local position by adding recoil to the original position

        if (_reticleTransform != null)
        {
            Vector3 recoilOffset = new Vector3(_recoil.x * _reticleSensitivity, _recoil.y * _reticleSensitivity);
            Vector3 targetPosition = _originalReticlePos + recoilOffset;
            _reticleTransform.anchoredPosition = Vector3.Lerp(_reticleTransform.anchoredPosition, targetPosition, _recoilSpeed * Time.deltaTime);
        }
    }

    public void WeaponRecoil()
    {
        //CameraShake.Instance.ShakeCamera();

        _recoil += new Vector3(Random.Range(-_recoilAmount, _recoilAmount), Random.Range(-_recoilAmount, _recoilAmount), 0f);
        _recoil = Vector3.ClampMagnitude(_recoil, _maxRecoil);
    }
}
