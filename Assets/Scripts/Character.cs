using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class Character : MonoBehaviour
{
    public Transform orbSocket;
    public float orbThrowSpeed = 10;
    public float orbReturningSpeed = 40;
    private Orb orb;
    private Camera camera;

    public float delayToPickupOrb = 1.5f;
    private float coolDownToPickupOrb;

    public Interactable interactableInRange;

    public Animator fadeInOutAnimator;
    public Animator damageFadeAnimator;
    public FirstPersonController firstPersonController;
    public bool isAlive = true;

    public GameObject rightMouseHint;

    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponentInChildren<Camera>();
        firstPersonController = GetComponent<FirstPersonController>();
        fadeInOutAnimator.SetTrigger("FadeOut");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && orb)
        {
            if(orb.isInHand)
            {
                ThrowOrb();
            }
            else if(!orb.returningToHand)
            {
                //InvokeOrb();
            }
        }

        // Correct the orb position
        if (orb && orb.isInHand)
        {
            orb.transform.localPosition = Vector3.zero;
        }

        // Check for near interactables
        if(interactableInRange && interactableInRange.CanBeUsed(this))
        {
            rightMouseHint.SetActive(true);
            Debug.Log("Press E to interact with " + interactableInRange.gameObject.name);

            if (Input.GetMouseButtonDown(1))
            {
                interactableInRange.Interact(this);
            }
        }
        else
        {
            rightMouseHint.SetActive(false);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Orb" && coolDownToPickupOrb < Time.time && isAlive)
        {
            orb = other.gameObject.GetComponent<Orb>();
            orb.SetPlayerRef(this);
            PickUpOrb();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Interactable")
        {
            interactableInRange = other.GetComponent<Interactable>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Interactable")
        {
            interactableInRange = null;
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

    void PickUpOrb()
    {
        orb.PickUp();
        orb.transform.position = orbSocket.position;
        orb.transform.parent = orbSocket;
    }

    public void InvokeOrb()
    {
        if (!orb.returningToHand && isAlive)
        {
            orb.SetPhysics(false);
            orb.returningToHand = true;

            StartCoroutine(CorutineInvokeOrb());
        }
    }

    public IEnumerator CorutineInvokeOrb()
    {
        Vector3 incomingOrbDirection = Vector3.zero;

        while(Vector3.Distance(orb.transform.position, orbSocket.position) > 0.02f)
        {
            incomingOrbDirection = (orbSocket.position - orb.transform.position).normalized;
            orb.transform.position = Vector3.MoveTowards(orb.transform.position, orbSocket.position, Time.deltaTime * orbReturningSpeed);

            if (Vector3.Distance(orb.transform.position, orbSocket.position) < 0.5f)
            {
                orb.transform.parent = orbSocket;
            }

            yield return new WaitForEndOfFrame();
        }
        
        Vector3 overShootOffset = incomingOrbDirection * 0.25f;

        while (Vector3.Distance(orb.transform.position, orbSocket.position + overShootOffset) > 0.01f)
        {
            orb.transform.position = Vector3.MoveTowards(orb.transform.position, orbSocket.position + overShootOffset, Time.deltaTime * orbReturningSpeed * 0.2f);
            //Vector3.Lerp(orb.transform.position, overShootTarget, Time.deltaTime * orbReturningSpeed);    
            yield return new WaitForEndOfFrame();
        }

        while (Vector3.Distance(orb.transform.position, orbSocket.position) > 0.01f)
        {
            orb.transform.position = Vector3.MoveTowards(orb.transform.position, orbSocket.position, Time.deltaTime * orbReturningSpeed * 0.04f);
            //Vector3.Lerp(orb.transform.position, orbSocket.position, Time.deltaTime * orbReturningSpeed);
            yield return new WaitForEndOfFrame();
        }

        orb.returningToHand = false;
        PickUpOrb();
    }

    public Orb GetOrb()
    {
        return orb;
    }

    public void TakeDamage(float amount = 1)
    {
        damageFadeAnimator.SetTrigger("TakeDamage");
        Debug.Log("Took damage!");

        if(orb && orb.isInHand && orb.energyContainer.energy > 0)
        {
            orb.Explode();
            amount -= 1;
        }

        audioSource.Play();

        if(amount > 0) Kill();
    }

    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(TransitionToSceneCoroutine(sceneName));
    }

    IEnumerator TransitionToSceneCoroutine(string sceneName)
    {
        fadeInOutAnimator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
    }

    public void Kill()
    {
        if (!isAlive) return;
        isAlive = false;

        if(orb && orb.isInHand)
        {
            orb.enabled = false;
            orb.transform.parent = null;
            orb.SetPhysics(true);
        }

        Destroy(firstPersonController);
        Destroy(GetComponent<CharacterController>());

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.AddTorque(transform.forward * 2, ForceMode.VelocityChange);
        rigidbody.AddForce(Vector3.up * 5, ForceMode.VelocityChange);

        this.enabled = false;

        StartCoroutine(CorutineRetry());
    }

    public IEnumerator CorutineRetry()
    {
        yield return new WaitForSeconds(1.5f);
        fadeInOutAnimator.SetTrigger("fadeIn");
        yield return new WaitForSeconds(1);
        TransitionToScene(SceneManager.GetActiveScene().name);
    }
}

