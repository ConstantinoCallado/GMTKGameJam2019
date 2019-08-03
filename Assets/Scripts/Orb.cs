using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    private Rigidbody rigidbody;
    private Collider collider;
    public bool isInHand = false;
    public bool returningToHand = false;

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

    public Rigidbody GetRigidbody()
    {
        return rigidbody;
    }

    public void SetPhysics(bool value)
    {
        rigidbody.isKinematic = !value;
        collider.enabled = value;
    }
}
