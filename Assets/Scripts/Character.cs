using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Transform orbSocket;
    public float orbThrowSpeed = 10;
    private Orb orb;
    private Camera camera;

    public float delayToPickupOrb = 0.2f;
    private float coolDownToPickupOrb;

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
            if(orb && orb.isInHand)
            {
                ThrowOrb();
            }
        }

        if (orb && orb.isInHand)
        {
            orb.transform.localPosition = Vector3.zero;
        }
    }


    void ThrowOrb()
    {
        if (orb && orb.isInHand)
        {
            orb.Throw();
            orb.transform.parent = null;
            orb.transform.position = camera.transform.position + (camera.transform.forward * 0.75f);
            orb.GetRigidbody().AddForce(camera.transform.forward * orbThrowSpeed, ForceMode.VelocityChange);

            coolDownToPickupOrb = Time.time + delayToPickupOrb;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Orb" && coolDownToPickupOrb < Time.time)
        {
            // Pick up the orb
            orb = other.gameObject.GetComponent<Orb>();
            orb.PickUp();

            orb.transform.position = orbSocket.position;
            orb.transform.parent = orbSocket;
        }
    }
}
