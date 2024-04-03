using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Recoil")]

    [SerializeField] private float _recoilAmount = 0.1f;
    [SerializeField] private float _recoilSpeed = 5f;
    [SerializeField] private float _maxRecoil = 0.5f;

    [SerializeField] private float _reticleSensitivity = 500;
    [SerializeField] private RectTransform _reticleTransform;

    private Vector3 _reticleOffset;
    private Vector3 _reticleTargetPos;

    private Vector3 _originalPos;
    private Vector3 _originalReticlePos;
    private Vector3 _recoil;


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
            _reticleOffset = new Vector2(_recoil.x * _reticleSensitivity * 100f, _recoil.y * _reticleSensitivity * 100); //move the reticle 
            _reticleTargetPos = _originalReticlePos + _reticleOffset; //updates reticle by adding movement based on original position
            _reticleTransform.anchoredPosition = Vector2.Lerp(_reticleTransform.anchoredPosition, _reticleTargetPos, _recoilSpeed * Time.deltaTime);
        }
    }

    public void WeaponRecoil()
    {
        _recoil += new Vector3(Random.Range(-_recoilAmount, _recoilAmount), Random.Range(-_recoilAmount, _recoilAmount), Random.Range(-_recoilAmount, _recoilAmount)); // random recoil value
        _recoil = Vector3.ClampMagnitude(_recoil, _maxRecoil); // recoil along this clamped value and don't exceed maxRecoil.
    }
}

