using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    private Rigidbody rigidbody;
    private Collider collider;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PickUp()
    {
        rigidbody.isKinematic = true;
        collider.enabled = false;
    }

    public void Throw()
    {
        rigidbody.isKinematic = false;
        collider.enabled = true;
    }

    public Rigidbody GetRigidbody()
    {
        return rigidbody;
    }
}
