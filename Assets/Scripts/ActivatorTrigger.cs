using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatorTrigger : MonoBehaviour
{
    public GameObject[] gameObjectList;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            for (int i = 0; i < gameObjectList.Length; i++ )
            {
                if(gameObjectList[i])
                {
                    gameObjectList[i].SetActive(true);
                }
            }

            Destroy(gameObject);
        }
    }

}
