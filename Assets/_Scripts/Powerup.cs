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
                        player.AmmoPickup();
                        break;

                    case 1:
                        Debug.Log("You Picked Up Health!");
                        player.HealthPickup();
                        break;
                    case 2:
                        Debug.Log("You picked up a Vaccine Component!");
                        _isVaccine = true;
                        break;
                }
            }
            else
            {
                Debug.LogError("Player is NULL");
            }

            if (!_isVaccine)
            {
                Destroy(this.gameObject, 0.5f);
            }
            else
            {
                Destroy(this.gameObject, 2f);
            }
        }
    }
}
