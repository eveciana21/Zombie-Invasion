using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private int _powerupID;
    private bool _isVaccine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player player = GameObject.Find("PlayerCapsule").GetComponent<Player>();

            if (player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        Debug.Log("You Picked Up Ammo!");
                        player.AmmoPickup(60);
                        AudioManager.Instance.SFX(0);
                        break;

                    case 1:
                        Debug.Log("You Picked Up Health!");
                        AudioManager.Instance.SFX(0);
                        player.HealthPickup();
                        break;

                    case 2:
                        Debug.Log("You Picked Up Small Ammo!");
                        player.AmmoPickup(10);
                        AudioManager.Instance.SFX(0);
                        break;
                }
            }
            else
            {
                Debug.LogError("Player is NULL");
            }

            Destroy(this.gameObject, 0.3f);
        }
    }
}

