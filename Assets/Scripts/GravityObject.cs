using UnityEngine;

public class GravityObject : MonoBehaviour
{
    public BakedGravityData gravityData;
    public float forceMultiplier = 1;
    public Vector3 initialForce = new Vector3(0, 0, 1);
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(initialForce, ForceMode.VelocityChange);

    }
    private void FixedUpdate()
    {
        Vector3 force = gravityData.GetGravityAtPosition(transform.position);
        rb.AddForce(force * forceMultiplier);
    }
}
