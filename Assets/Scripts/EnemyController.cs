using System;
using UnityEngine;

public enum EnemyState { Idle, SeekingPlayer, SeekingSpot, PreAttacking, Attacking, AttackResting, Knocked, Stunned, Falling, Dying };

[RequireComponent(typeof (UnityEngine.AI.NavMeshAgent))]

public class EnemyController : MonoBehaviour
{
       
    public UnityEngine.AI.NavMeshAgent agent { get; private set; }   // the navmesh agent required for the path finding
    public Transform character { get; private set; }

    public LayerMask whatIsGround;

    public Transform target;                                        // target to aim for

    public float maximumSeekDistance;

    public float preAttackDuration;
    public float attackingDistance;
    public float attackRestingTime;

    // stun
    public float stunTime;

    // knockback
    public float knockbackDistance;
    public float knockbackSpeed = 1.0F;                             // Movement speed in units/sec.
    public float knockbackTime;

    [Header("Initial state")]
    public EnemyState initialState;

    public EnemyAttackArea attackArea;
    public ParticleSystem stunParticles;

    [SerializeField] public bool isGuard = false;
    public Transform guardingSpot;
    public float guardingMinRadius = 2f;
    public float guardingMaxRadius = 6f;

    Rigidbody m_Rigidbody;
    Animator m_Animator;
    EnemyState m_state;

    [SerializeField] float m_GroundCheckDistance = 1f;
    bool m_IsGrounded;
    Vector3 m_GroundNormal;

    private float m_stunEndTime;
    private float m_knockbackEndTime;
    private float m_preAttackTime;
    private float m_restingEndTime;

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
        Debug.Log(m_state);
        switch (m_state)
        {
            case EnemyState.Idle:
                if (target != null)
                {
                    float distanceToPlayer = Vector3.Distance(target.transform.position, transform.position);

                    if (!isGuard || guardingSpot == null)
                    {
                        if (distanceToPlayer <= maximumSeekDistance)
                        {
                            m_state = EnemyState.SeekingPlayer;
                            m_Animator.SetBool("seeking", true);
                        }
                    }
                    else
                    {
                        float distanceToSpot = Vector3.Distance(guardingSpot.transform.position, transform.position);
                        float playerDistanceToSpot = Vector3.Distance(guardingSpot.transform.position, target.transform.position);

                        // The player is visible
                        if (distanceToPlayer <= maximumSeekDistance)
                        {
                            // The player is inside the guarding range
                            if (playerDistanceToSpot <= guardingMaxRadius)
                            {
                                m_state = EnemyState.SeekingPlayer;
                                m_Animator.SetBool("seeking", true);
                            }
                            else
                            {
                                RotateTowards(target);
                            }
                        }
                        // if player is out of range I go back the spot
                        else if (distanceToPlayer > maximumSeekDistance && distanceToSpot > guardingMinRadius)
                        {
                            m_state = EnemyState.SeekingSpot;
                            m_Animator.SetBool("seeking", true);
                        }
                    }
                }
                break;
            case EnemyState.SeekingPlayer:
                if (target != null)
                {
                    float distanceToPlayer = Vector3.Distance(target.transform.position, transform.position);
                    float playerDistanceToSpot = Vector3.Distance(guardingSpot.transform.position, target.transform.position);

                    if (agent.enabled)
                        agent.SetDestination(target.position);
                    
                    // enemy is guard and player is too far from the guarding spot 
                    if (isGuard && guardingSpot != null && playerDistanceToSpot > guardingMaxRadius)
                    {
                        m_state = EnemyState.Idle;
                        m_Animator.SetBool("seeking", false);
                    }
                    // player is out of sight
                    else if (distanceToPlayer > maximumSeekDistance)
                    {
                        m_state = EnemyState.Idle;
                        m_Animator.SetBool("seeking", false);
                    }
                    // player is in reach for an attack
                    else if (distanceToPlayer < attackingDistance)
                    {
                        m_preAttackTime = Time.time + preAttackDuration;
                        m_state = EnemyState.PreAttacking;
                        m_Animator.SetBool("seeking", false);

                        RotateTowards(target);
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
                    stunParticles.enableEmission = false;
                    m_state = EnemyState.Idle;
                }
                break;
            case EnemyState.PreAttacking:
                if (m_preAttackTime < Time.time)
                {
                    m_Animator.SetTrigger("attack");
                    m_state = EnemyState.Attacking;
                }
                break;
            case EnemyState.Attacking:
                // nothing at the moment
                break;
            case EnemyState.AttackResting:
                if (m_restingEndTime < Time.time)
                {
                    m_state = EnemyState.Idle;
                }
                break;
            case EnemyState.SeekingSpot:
                if (guardingSpot != null)
                {
                    float distance = Vector3.Distance(guardingSpot.transform.position, transform.position);

                    if (agent.enabled)
                        agent.SetDestination(guardingSpot.transform.position);

                    if(distance < guardingMinRadius)
                        m_state = EnemyState.Idle;
                }
                else
                {
                    m_state = EnemyState.Idle;
                }

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
        // calculate effect depending on energy type
        switch (orb.energyContainer.energyType)
        {
            case EnergyType.None:
            case EnergyType.Key:
				if(m_state != EnemyState.Stunned)
					Knocked();
                break;
            case EnergyType.Light:
				if(m_state != EnemyState.Dying)
					Stunned();
                break;
            case EnergyType.Damage:
                StartDying();
                break;
            default:
                break;
        }

        // Calculate knockback
        Vector3 knockback = Vector3.zero;

        // if the ball is on the floor or bouncing, we don't take it into account
        if (!orb.isInHand && !orb.returningToHand && orb.GetRigidbody().velocity.magnitude > 2f)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Debug.DrawRay(contact.point, contact.normal, Color.white);

                knockback.x -= contact.normal.x;
                knockback.z -= contact.normal.z;
                knockback.y = contact.normal.y;
            }
            knockback = knockback + Vector3.up;
            knockback.Normalize();
            knockback.Scale(new Vector3(knockbackDistance, knockbackDistance, knockbackDistance));

            // apply knockback
            m_Rigidbody.AddForce(knockback, ForceMode.VelocityChange);
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

        stunParticles.enableEmission = true;

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
        
        m_restingEndTime = Time.time + attackRestingTime;

		if(m_state != EnemyState.Knocked && m_state != EnemyState.Stunned)
			m_state = EnemyState.AttackResting;
    }

    private void RotateTowards(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed);
    }
}
