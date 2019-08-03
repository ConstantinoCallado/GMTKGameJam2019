using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public bool destroyNonPlayerObjects = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag is "Player")
        {
            // if player then tell the player to do its FallDeath
            //TO DO
            //Destroy(gameObject);
        }
        else if (destroyNonPlayerObjects && collision.gameObject.tag is "Enemy")
        { // not playe so just kill object - could be falling enemy for example
            Object.Destroy(collision.gameObject);
        }
    }
}
