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
            Player player = other.transform.GetComponentInChildren<Player>();

            if (player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        Debug.Log("You Picked Up Ammo!");
                        player.AmmoPickup();
                        break;

                    case 1:
                        Debug.Log("You Picked Up Health!");
                        player.HealthPickup();
                        break;
                }
            }
            //Destroy(this.gameObject);
        }
    }
}
