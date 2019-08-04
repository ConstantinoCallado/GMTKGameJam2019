using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackArea : MonoBehaviour
{
    public bool active { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag is "Player")
        {
            active = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag is "Player")
        {
            active = false;
        }
    }
}
