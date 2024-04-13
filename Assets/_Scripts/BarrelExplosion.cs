using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelExplosion : MonoBehaviour
{
    [SerializeField] private GameObject _explosion;
    [SerializeField] private GameObject _barrel;
    [SerializeField] private GameObject _explosionCollider;
    private Player _player;

    private void Start()
    {
        _player = GameObject.Find("PlayerCapsule").GetComponent<Player>();
    }

    public void DestroyBarrel()
    {
        StartCoroutine(DestroySequence());
    }

    IEnumerator DestroySequence()
    {
        _explosion.SetActive(true);
        _player.AddToScore(50);
        yield return new WaitForSeconds(0.5f);
        _explosionCollider.SetActive(true);
        _barrel.SetActive(false);
        Destroy(this.gameObject, 2);
    }
}
