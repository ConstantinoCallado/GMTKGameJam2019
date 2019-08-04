using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Switch[] switches;

    public bool isOpened = false;

    public float closedHeight = 2.34f;
    public float openHeight = 6.4f;
    public Transform doorTransform;
    public float speed = 2f;

    public void Awake()
    {
        if(switches.Length == 0)
        {
            Debug.Log("Door doesn't have any switch to control it");
        }   
    }
    
    public void Start()
    {
        doorTransform.localPosition = new Vector3(0, isOpened ? openHeight : closedHeight, 0);

        StartCoroutine(DoorCoroutine());
    }

    public void Update()
    {
        bool isPowered = true;

        for(int i=0; i<switches.Length; i++)
        {
            isPowered = isPowered && switches[i].isPowered;
        }

        isOpened = isPowered;
    }

    public IEnumerator DoorCoroutine()
    {
        while(true)
        {
            Vector3 target = new Vector3(0, isOpened ? openHeight : closedHeight, 0);

            if(Vector3.Distance(doorTransform.localPosition, target) > 0.1f)
            {
                doorTransform.localPosition = Vector3.MoveTowards(doorTransform.localPosition, target, Time.deltaTime * speed);
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
