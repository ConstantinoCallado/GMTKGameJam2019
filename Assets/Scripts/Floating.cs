using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floating : MonoBehaviour
{
    float floatY;
    public float FloatStrength; // Set strength in Unity

    // Update is called once per frame
    void Update()
    {
        Vector3 floatingPosition = transform.position;
        floatY = transform.position.y;
        floatY = (Mathf.Sin(Time.time) * FloatStrength);
        floatingPosition.y += floatY;
        transform.position = floatingPosition;
    }
}
