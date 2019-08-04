using System;
using UnityEngine;

public enum EnemyState { Idle, SeekingPlayer, SeekingSpot, Attacking, Knocked, Stunned, Falling, Dying };

[RequireComponent(typeof (UnityEngine.AI.NavMeshAgent))]

public class EnemyController : MonoBehaviour
{
       
    public UnityEngine.AI.NavMeshAgent agent { get; private set; }   // the navmesh agent required for the path finding
    public Transform character { get; private set; }

    public LayerMask whatIsGround;

    public Transform target;                                        // target to aim for

    public float maximumSeekDistance;
    public float attackingDistance;

    // stun
    public float stunTime;

    // knockback
    public float knockbackDistance;
    public float knockbackSpeed = 1.0F;                             // Movement speed in units/sec.
    public float knockbackTime;

    [Header("Initial state")]
    public EnemyState initialState;

    public EnemyAttackArea attackArea;

    Rigidbody m_Rigidbody;
    Animator m_Animator;
    EnemyState m_state;

    [SerializeField] float m_GroundCheckDistance = 1f;
    bool m_IsGrounded;
    Vector3 m_GroundNormal;

    private float m_stunEndTime;
    private float m_knockbackEndTime;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Animator = GetComponent<Animator>();
        m_state = initialState;

        // get the components on the object we need ( should not be null due to require component so no need to check )
        agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
        character = GetComponent<Transform>();
        
        agent.updateRotation = true;
	    agent.updatePosition = true;
    }


    private void Update()
    {
        switch (m_state)
        {
            case EnemyState.Idle:
                if (target != null)
                {
                    float targetDistance = Vector3.Distance(target.transform.position, transform.position);

                    if (targetDistance <= maximumSeekDistance)
                    {
                        m_state = EnemyState.SeekingPlayer;
                        m_Animator.SetBool("playerIsVisible", true);
                    }
                }
                break;
            case EnemyState.SeekingPlayer:
                if (target != null)
                {
                    float targetDistance = Vector3.Distance(target.transform.position, transform.position);
                    
                    if(agent.enabled)
                        agent.SetDestination(target.position);

                    if (targetDistance > maximumSeekDistance)
                    {
                        m_state = EnemyState.Idle;
                        m_Animator.SetBool("playerIsVisible", false);
                    }
                    else if (targetDistance < attackingDistance)
                    {
                        m_state = EnemyState.Attacking;
                        m_Animator.SetTrigger("attack");
                    }
                }

                break;
            case EnemyState.Knocked:
                if (m_knockbackEndTime < Time.time)
                {
                    m_Animator.SetBool("knocked", false);
                    m_Rigidbody.isKinematic = true;
                    agent.enabled = true;
                    m_state = EnemyState.Idle;
                }
                break;
            case EnemyState.Falling:
                if (m_IsGrounded)
                {
                    m_Animator.SetBool("falling", false);
                    m_Rigidbody.isKinematic = true;
                    agent.enabled = true;
                    m_state = EnemyState.Idle;
                }
                break;
            case EnemyState.Stunned:
                if (m_stunEndTime < Time.time)
                {
                    m_Animator.SetBool("stunned", false);
                    m_Rigidbody.isKinematic = true;
                    agent.enabled = true;
                    m_state = EnemyState.Idle;
                }
                break;
            case EnemyState.Attacking:
                // nothing at the moment
                break;
            case EnemyState.SeekingSpot:

                break;
            case EnemyState.Dying:

                break;
            default:
                break;
        }

        CheckGroundStatus();

        if (!m_IsGrounded)
        {
            // Start falling
            m_Rigidbody.isKinematic = false;
            agent.enabled = false;
            m_Animator.SetBool("falling", true);
            m_state = EnemyState.Falling;
        }
    }


    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void HitByOrb(Orb orb, Collision collision)
    {
        // Calculate knockback
        Vector3 knockback = Vector3.zero;

        // if the ball is on the floor or bouncing, we don't take it into account
        if (!orb.returningToHand && orb.GetRigidbody().velocity.magnitude > 2f)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Debug.DrawRay(contact.point, contact.normal, Color.white);

                knockback.x -= contact.normal.x;
                knockback.z -= contact.normal.z;
            }
            knockback = knockback + Vector3.up;
            knockback.Normalize();
            knockback.Scale(new Vector3(knockbackDistance, knockbackDistance, knockbackDistance));

            // apply knockback
            m_Rigidbody.AddForce(knockback, ForceMode.VelocityChange);

            // calculate effect depending on energy type
            switch (orb.energyContainer.energyType)
            {
                case EnergyType.None:
                case EnergyType.Key:
                    Knocked();
                    break;
                case EnergyType.Light:
                    Stunned();
                    break;
                case EnergyType.Damage:
                    StartDying();
                    break;
                default:
                    break;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    void CheckGroundStatus()
    {
        RaycastHit hitInfo;
#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance, whatIsGround))
        {
            m_IsGrounded = true;
        }
        else
        {
            m_IsGrounded = false;
        }
    }

    private void Knocked()
    {
        m_Rigidbody.isKinematic = false;
        agent.enabled = false;

        m_state = EnemyState.Knocked;
        m_Animator.SetBool("knocked", true);

        m_knockbackEndTime = Time.time + knockbackTime;
    }

    private void Stunned()
    {
        m_Rigidbody.isKinematic = false;
        agent.enabled = false;

        m_state = EnemyState.Stunned;
        m_Animator.SetBool("stunned", true);

        m_stunEndTime = Time.time + stunTime;
    }

    private void StartDying()
    {
        m_Rigidbody.isKinematic = false;
        agent.enabled = false;

        m_state = EnemyState.Dying;
        m_Animator.SetTrigger("killed");
    }

    public void Kill()
    {
        Destroy(gameObject);
    }

    public void Attack()
    {
        // if player is in the attack area, give damage
        if (attackArea != null && attackArea.active && target != null && target.tag == "Player")
            target.gameObject.GetComponent<Character>().TakeDamage();

        m_state = EnemyState.Idle;
    }
}
