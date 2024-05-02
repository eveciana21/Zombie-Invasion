using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    [SerializeField] private GameObject _poofParticle;
    void Start()
    {
        AudioManager.Instance.SFX(2);
        StartCoroutine(DestroyRoutine());
    }

    IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(3);
        Instantiate(_poofParticle, transform.position + new Vector3(0, 0.3F, 0), Quaternion.identity);
        Destroy(this.gameObject);
    }
}
