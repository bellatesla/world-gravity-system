using UnityEngine;
using UnityEditor;

/// <summary>
/// Shows a Gizmos ring in with unit increaments relative to script transform.
/// </summary>
public class DrawWorldLines : MonoBehaviour
{
    public Color gridColor = Color.yellow;

    public float worldRadiusMeters = 100;

    public int units = 10;

    private Vector3 worldcenter = Vector3.zero;


    private void OnDrawGizmos()
    {
        // Draw extended handle color lines
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(worldcenter, worldcenter + transform.forward * worldRadiusMeters);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(worldcenter, worldcenter + transform.right * worldRadiusMeters);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(worldcenter, worldcenter + transform.up * worldRadiusMeters);
        Gizmos.color = gridColor;


        // Draw rings at each unit distancw
        for (int i = 0; i < worldRadiusMeters/units+.1f; i++)
        {
            float dist = units*(i);

            Handles.color = gridColor;
            
            Handles.Label(transform.forward * dist, dist + "m");
            Handles.Label(transform.forward * -dist, -dist + "m");
           
            Handles.DrawWireArc(transform.position, transform.up, -transform.right, 360, dist);
        }
    }
}