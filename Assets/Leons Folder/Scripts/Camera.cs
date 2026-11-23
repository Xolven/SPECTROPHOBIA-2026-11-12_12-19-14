using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject target;
    public float speed;
    public Vector3 offset;
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.transform.position + offset, Time.fixedDeltaTime * speed);
    }
}
