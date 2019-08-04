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
    public float delayToReturnToHand = 1f;

    public float fakingGravityTime = 0.3f;
    private float fakingGravityUntil;
    private Character playerRef;

    public bool invocationStarted = false;
    
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

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Orb hitted " + collision.gameObject.name);


        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<EnemyController>().HitByOrb(this, collision);
        }

        fakingGravityUntil = 0;
        Hit();
    }

    public void SetPlayerRef(Character character)
    {
        playerRef = character;
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

    public void Hit()
    {
        if(enabled)
        {
            energyContainer.DrainEnergy(100);

            if(energyContainer.pickedFromFountain)
            {
                energyContainer.pickedFromFountain.ResetFountain();
            }

            if (playerRef && !invocationStarted)
            {
                invocationStarted = true;
                StartCoroutine(InvokeOrbCorutine());
            }
        }
    }

    public IEnumerator InvokeOrbCorutine()
    {
        yield return new WaitForSeconds(delayToReturnToHand);
        playerRef.InvokeOrb();

        invocationStarted = false;
    }
}
