using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public bool destroyNonPlayerObjects = true;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag is "Player")
        {
            // if player then tell the player to do its FallDeath
            collider.gameObject.GetComponent<Character>().Kill();
        }
        else if (destroyNonPlayerObjects && collider.gameObject.tag is "Enemy")
        { // not playe so just kill object - could be falling enemy for example
            Object.Destroy(collider.gameObject);
        }
    }
}
