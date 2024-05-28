using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private int _powerupID;

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
                        player.AmmoPickup(90);
                        AudioManager.Instance.SFX(4);
                        break;

                    case 1:
                        Debug.Log("You Picked Up Health!");
                        AudioManager.Instance.SFX(0);
                        player.HealthPickup();
                        break;

                    case 2:
                        Debug.Log("You Picked Up Small Ammo!");
                        player.AmmoPickup(30);
                        AudioManager.Instance.SFX(4);
                        break;
                }
            }
            else
            {
                Debug.LogError("Player is NULL");
            }

            Destroy(this.gameObject, 0.2f);
        }
    }
}

