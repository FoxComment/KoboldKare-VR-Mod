using UnityEngine;
public class SmoothFollow : MonoBehaviour
{
    [SerializeField] private Transform follow;
    [SerializeField] private Vector3 offset;
    [SerializeField] [Range(.01f, 1)] private float stiffness;
    [SerializeField] private bool smoothRotation;
    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, follow.position + offset, stiffness);
        if(smoothRotation)transform.rotation = Quaternion.Lerp(transform.rotation, follow.rotation, stiffness);
    }
}
