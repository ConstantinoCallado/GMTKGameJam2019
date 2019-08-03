using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnergyContainer))]
public class Orb : MonoBehaviour
{
    private Rigidbody rigidbody;
    private Collider collider;

    public EnergyContainer energyContainer;

    [Header("Movement settings")]
    public bool isInHand = false;
    public bool returningToHand = false;

    public float fakingGravityTime = 0.3f;
    private float fakingGravityUntil;

    // Start is called before the first frame update
    void Start()
    {
        energyContainer = GetComponent<EnergyContainer>();
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FixedUpdate()
    {
        if (Time.time < fakingGravityUntil)
        {
            rigidbody.AddForce(-Physics.gravity, ForceMode.Acceleration);
        }
    }

    public void OnCollisionEnter(Collision other)
    {
        Debug.Log("Orb hitted " + other.gameObject.name);

        fakingGravityUntil = 0;
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

    public void PickUp()
    {
        SetPhysics(false);
        isInHand = true;
    }

    public void Throw()
    {
        SetPhysics(true);
        isInHand = false;
        fakingGravityUntil = Time.time + fakingGravityTime;
    }
}
