using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Transform orbSocket;
    public float orbThrowSpeed = 10;
    private Orb orb;
    private Camera camera;


    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            ThrowOrb();
        }
    }

    void ThrowOrb()
    {
        if(orb)
        {
            orb.Throw();
            orb.transform.parent = null;
            orb.GetRigidbody().AddForce(camera.transform.forward * orbThrowSpeed, ForceMode.VelocityChange);
            orb = null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Orb")
        {
            // Pick up the orb
            orb = other.gameObject.GetComponent<Orb>();
            orb.PickUp();

            orb.transform.parent = orbSocket;
            orb.transform.localPosition = Vector3.zero;
        }
    }
}
