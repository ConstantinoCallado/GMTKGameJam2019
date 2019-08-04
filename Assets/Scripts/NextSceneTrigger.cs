using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextSceneTrigger : MonoBehaviour
{
    public string nextSceneName = "Level1";

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<Character>().TransitionToScene(nextSceneName);

            Destroy(gameObject);
        }
    }

}
