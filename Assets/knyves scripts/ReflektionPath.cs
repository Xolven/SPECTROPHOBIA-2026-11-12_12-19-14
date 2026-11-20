using Pathfinding;
using UnityEngine;

[RequireComponent(typeof(IAstarAI))]
public class ReflektionPath : MonoBehaviour
{
    public enum State
    {
        Wander, Chase
    }

    private State currentState;
    private GameObject player;
    public float Maxdistance;
    public LayerMask blockinglayer;
    public GameObject[] wandwe;
    private GameObject CurrentWanderPos;

    private IAstarAI AI;

    private void Wander()
    {
        if (Vector2.Distance(transform.position, CurrentWanderPos.transform.position) < 3f)
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
        player = GameObject.FindGameObjectWithTag("Player");
        
        CurrentWanderPos = wandwe[Random.Range(0, wandwe.Length)];
    }

    void Update()
    {
        currentState = GetState();

        if(currentState == State.Wander)
        {
            Wander();
        }
        else if(currentState == State.Chase)
        {
            Chase();
        }
    }

    private State GetState()
    {
        if (Vector2.Distance(transform.position, player.transform.position) > Maxdistance)
        {
            return State.Wander;
        }

        if (Physics2D.Linecast(transform.position, player.transform.position, blockinglayer))
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
            Gizmos.DrawLine(transform.position, player.transform.position);
        }
    }
}
