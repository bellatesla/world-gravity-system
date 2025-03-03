using UnityEngine;

public static class GizmoArrowUtility
{
    /// <summary>
    /// Draws a 3D arrow with a cone-shaped head
    /// </summary>
    /// <param name="position">Starting position</param>
    /// <param name="direction">Direction and magnitude</param>
    /// <param name="arrowHeadLength">Length of the arrow head</param>
    /// <param name="arrowHeadAngle">Angle of the arrow head cone in degrees</param>
    /// <param name="arrowHeadSections">Number of sections in the cone (higher = smoother)</param>
    public static void DrawArrow(Vector3 position, Vector3 direction, float arrowHeadLength = 0.25f,
                                float arrowHeadAngle = 20.0f, int arrowHeadSections = 8)
    {
        if (direction == Vector3.zero) return;

        Vector3 endPoint = position + direction;
        Gizmos.DrawLine(position, endPoint);

        // Create arrow head
        Vector3 forward = direction.normalized;
        Vector3 right = Vector3.Slerp(forward, -forward, 0.5f);
        right = Vector3.Cross(forward, right).normalized;
        Vector3 up = Vector3.Cross(forward, right).normalized;

        float angle = arrowHeadAngle * Mathf.Deg2Rad;
        float length = Mathf.Min(direction.magnitude * 0.4f, arrowHeadLength);

        // Draw the cone
        Vector3 baseCenter = endPoint - (forward * length);
        float radius = Mathf.Tan(angle) * length;

        // Draw first line to create the cone shape
        Vector3 lastPoint = baseCenter + right * radius;

        for (int i = 1; i <= arrowHeadSections; i++)
        {
            float t = i / (float)arrowHeadSections;
            float rad = t * Mathf.PI * 2.0f;

            Vector3 pointOnCircle = baseCenter +
                (right * Mathf.Cos(rad) + up * Mathf.Sin(rad)) * radius;

            // Line from cone base to the tip
            Gizmos.DrawLine(pointOnCircle, endPoint);

            // Line connecting base points to form the circle
            Gizmos.DrawLine(lastPoint, pointOnCircle);
            lastPoint = pointOnCircle;
        }
    }
}