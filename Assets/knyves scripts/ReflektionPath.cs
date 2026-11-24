using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(IAstarAI), typeof(Rigidbody2D))]
public class ReflektionPath : MonoBehaviour
{
    public enum State
    {
        Wander, Chase
    }

    private State currentState;
    private GameObject player;
    public float Maxdistance;
    public float distanceToGoal;
    public LayerMask blockinglayer;
    public GameObject[] wandwe;
    private GameObject CurrentWanderPos;

    public float Intrest;
    private float interestTimer;

    private IAstarAI AI;
    private Rigidbody2D Rb;

    private void Wander()
    {
        if (Vector2.Distance(transform.position, CurrentWanderPos.transform.position) < distanceToGoal)
        {
            CurrentWanderPos = wandwe[Random.Range(0, wandwe.Length)];
        }

        AI.destination = CurrentWanderPos.transform.position;
    }

    private void Chase()
    {
        AI.destination = player.transform.position;
    }

    void Awake()
    {
        AI = GetComponent<IAstarAI>();
        Rb = GetComponent<Rigidbody2D>();

        player = GameObject.FindGameObjectWithTag("Player");
        
        CurrentWanderPos = wandwe[Random.Range(0, wandwe.Length)];
    }

    void Update()
    {
        currentState = GetState();

        if(currentState == State.Wander)
        {
            Wander();
            Maxdistance = 10;
        }
        else if(currentState == State.Chase)
        {
            Chase();
            Maxdistance = 25;
        }

        if (AI is not null && AI.hasPath)
        {
            Rb.linearVelocityX = Mathf.Sign(AI.destination.x - AI.position.x) * AI.maxSpeed;
        }

        if (ShouldTrackPlayer())
        {
            interestTimer = Intrest;
        }
        else if (interestTimer >= 0)
        {
            interestTimer -= Time.deltaTime;
        }
    }

    private bool ShouldTrackPlayer()
    {
        return Vector2.Distance(transform.position, player.transform.position) < Maxdistance
            && !Physics2D.Linecast(transform.position, player.transform.position, blockinglayer);
    }

    private State GetState()
    {
        if (interestTimer <= 0 && !ShouldTrackPlayer())
        {
            return State.Wander;
        }

        return State.Chase;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Maxdistance);

        if (player && currentState == State.Chase)
        {
            Gizmos.DrawLine(AI.position, AI.steeringTarget);
        }

        if (AI is not null)
        {
            Gizmos.DrawWireSphere(AI.destination, distanceToGoal);
        }
    }
}
