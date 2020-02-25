using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieBehaviour : MonoBehaviour
{
    private int currentHealth;
    public int health = 100;

    private Animator anim;

    public enum State
    {
        Idle,
        Follow,
        Die,
        Attack,
    }

    public State state = State.Idle;

    // The object the enemy wants to follow
    public Transform target;

    // How fast should the enemy rotate?
    public float rotateSpeed = 3.0f;

    // How close should the enemy be before they follow?
    public float followRange = 10.0f;

    // How far should the target be before the enemy gives up following it? 
    // Needs to be >= followRange
    public float idleRange = 10.0f;

    private NavMeshAgent agent;

    IEnumerator IdleState()
    {
        //OnEnter
        Debug.Log("Idle: Enter");

        agent.isStopped = true;
        anim.SetFloat("speed", 0);
        anim.SetBool("attacking", false);

        while (state == State.Idle)
        {
            //OnUpdate
            if (GetDistance() < followRange)
            {
                state = State.Follow;
            }

            yield return 0;
        }
        //OnEnd
        Debug.Log("Idle: Exit");
        GoToNextState();
    }

    IEnumerator FollowState()
    {
        Debug.Log("Follow: Enter");
        while (state == State.Follow)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);

            anim.SetFloat("speed", agent.velocity.magnitude);
            anim.SetBool("attacking", false);

            if (GetDistance() > idleRange)
            {
                state = State.Idle;
            }
            else if ((GetDistance() <= agent.stoppingDistance + 0.5f) && (agent.pathStatus == NavMeshPathStatus.PathComplete))
            {
                state = State.Attack;
            }

            yield return new WaitForSeconds(0.2f);
        }
        Debug.Log("Follow: Exit");
        GoToNextState();
    }

    IEnumerator AttackState()
    {
        //OnEnter
        Debug.Log("Attack: Enter");

        anim.SetFloat("speed", 0);
        anim.SetBool("attacking", true);

        while (state == State.Attack)
        {
            RotateTowards(target);

            //OnUpdate
            if (GetDistance() > (agent.stoppingDistance + 1))
            {
                state = State.Follow;
            }

            yield return 0;
        }
        //OnEnd
        Debug.Log("Attack: Exit");
        GoToNextState();
    }

    private void RotateTowards(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotateSpeed);
    }

    IEnumerator DieState()
    {
        agent.isStopped = true;

        anim.SetBool("attacking", false);
        anim.SetBool("dead", true);


        Debug.Log("Die: Enter");

        Destroy(this.gameObject, 5);

        yield return 0;
    }

    public float GetDistance()
    {
        return (transform.position - target.transform.position).magnitude;
    }

    void GoToNextState()
    {
        // Stop any state if active
        StopAllCoroutines();

        string methodName = state.ToString() + "State";
        System.Reflection.MethodInfo info = GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        StartCoroutine((IEnumerator)info.Invoke(this, null));
    }


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = health;

        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        GoToNextState();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(currentHealth > 0)
        {
            TakeDamage(UnityEngine.Random.Range(5, 20));
        }
    }

    private void TakeDamage(int damageToDeal = 0)
    {
        currentHealth -= damageToDeal;

        if (currentHealth <= 0)
        {
            state = State.Die;
        }
        else
        {
            //If we're not dead, now that we hit them the enemy knows where we are
            followRange = Mathf.Max(GetDistance(), followRange);
            state = State.Follow;

            // Play the hit animation
            anim.SetTrigger("hit");
        }

        GoToNextState();

    }

    public int damageAmount = 20;

    public void PhysicalAttack()
    {
        if (GetDistance() <= agent.stoppingDistance + 0.5f)
        {
            target.SendMessage("TakeDamage", 
                                damageAmount, 
                                SendMessageOptions.DontRequireReceiver);
        }
    }
}
