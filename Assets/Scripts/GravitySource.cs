using UnityEngine;
using System.Collections.Generic;

public class GravitySource : MonoBehaviour
{
    public float strength = 1f;
    public float radius = 5f;
    public AnimationCurve falloff = AnimationCurve.EaseInOut(0, 1, 1, 0);

    // Optional: visual debugging
    public bool showDebugRadius = true;

    // Reference to the gravity data
    private BakedGravityData _gravityData;

    private void Start()
    {
        // Find gravity data if not assigned
        if (_gravityData == null)
        {
            // This assumes you have a GravityBakerTester in the scene
            var baker = Object.FindFirstObjectByType<GravityBakerTester>();
            if (baker != null)
            {
                _gravityData = baker.bakedData;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (showDebugRadius)
        {
            Gizmos.color = new Color(0, 0.5f, 1f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }

    // This method would be called from a modified BakedGravityData
    public Vector3 CalculateGravityContribution(Vector3 position)
    {
        Vector3 direction = transform.position - position;
        float distance = direction.magnitude;

        if (distance > radius || distance < 0.001f)
            return Vector3.zero;

        // Normalize and calculate strength based on distance
        direction.Normalize();
        float normalizedDistance = distance / radius;
        float actualStrength = strength * falloff.Evaluate(normalizedDistance);

        return direction * actualStrength;
    }
}