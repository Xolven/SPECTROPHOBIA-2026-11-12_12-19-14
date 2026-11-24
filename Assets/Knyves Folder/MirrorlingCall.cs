using Pathfinding;
using UnityEngine;

public class MirrorlingCall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            FindAnyObjectByType<ReflektionPath>().AI.destination = transform.position;
        }
    }
}
