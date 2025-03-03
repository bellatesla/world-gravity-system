using UnityEngine;
using static UnityEditor.PlayerSettings;

[ExecuteInEditMode]
public class GravityBakerTester : MonoBehaviour
{
    public Transform target;
    public BakedGravityData bakedData;

    public bool showNeighboorsCells = true;
    public bool showGridCells = true;
    public bool showDataInCells = true;
    private bool showAllGravityVectors = true;

    public Color cellDefaultColor = new Color(1, 1, 1, 0.3f);
    public Color cellTargetColor = new Color(1, 0, 0, 0.8f);
    public Color cellNeighborColor = new Color(0, 1, 0, 0.8f);
    public float dataMultiplier = 1;
    private float arrowHeadSize = .5f;

    public Color targetVectorColor = Color.white;
   

    [ContextMenu("Randomize Data")]
    public void AddRandomVectorDataToCells()
    {
        bakedData.RandomizeData();
    }


    [ContextMenu("Bake Gravity From Sources")]
    public void BakeGravityFromSources()
    {
        if (bakedData != null)
        {
            // Make sure we have the correct cell size
            bakedData.CreateCellsAndSetData();

            // Now bake from sources
            bakedData.BakeGravityFromSources();

            Debug.Log("Gravity baked from all sources in the scene");
        }
        else
        {
            Debug.LogError("BakedGravityData not assigned!");
        }
    }



    #region Gizmos



    void OnDrawGizmos()
    {
        if (bakedData == null) return;

        // Draw the whole cell grid
        for (int y = 0; y < bakedData.cellSize.y; y++)
        {
            for (int z = 0; z < bakedData.cellSize.z; z++)
            {
                for (int x = 0; x < bakedData.cellSize.x; x++)
                {
                    Gizmos.color = cellDefaultColor;
                    Vector3 worldPosition = bakedData.CellToWorldPostion(x, y, z);
                    Gizmos.DrawWireCube(worldPosition, Vector3.one * bakedData.unitSize);

                    // Optionally visualize all gravity vectors
                    if (showAllGravityVectors && bakedData.dictionaryVectorData.TryGetValue(new Vector3Int(x, y, z), out Vector3 gravityDir))
                    {
                        // Only draw non-zero vectors for clarity
                        if (gravityDir.sqrMagnitude > 0.01f)
                        {
                            DrawArrow(worldPosition, gravityDir * dataMultiplier * 0.5f, Color.red);
                        }
                    }
                    //if (bakedData.dictionaryVectorData.TryGetValue(new Vector3Int(x, y, z), out Vector3 gravityDir))
                    //{
                    //    // Only draw non-zero vectors for clarity
                    //    if (gravityDir.sqrMagnitude > 0.01f)
                    //    {
                    //        // Visualize the stored vector in the editor with an arrow
                    //        DrawArrow(neighborPos, gravityDir * dataMultiplier * 0.5f,
                            
                    //        Gizmos.color = new Color(1f, 0.7f, 0f, 0.5f); // Orange
                    //        Gizmos.DrawLine(worldPosition, worldPosition + gravityDir * dataMultiplier * 0.5f);
                    //    }
                    //}
                }
            }
        }

        if (target == null) return;

        // Draw the target cell
        Vector3Int cellPosition = bakedData.WorldPositionToCellPosition(target.position);

        // The cell position is the dictionary key
        if (bakedData.dictionaryVectorData.TryGetValue(cellPosition, out Vector3 dictValue))
        {
            // Get world position from the key
            Vector3 worldPosition = bakedData.CellToWorldPostion(cellPosition.x, cellPosition.y, cellPosition.z);

            // Show Stored Data           
            Gizmos.color = cellTargetColor;
            Gizmos.DrawSphere(worldPosition, 0.1f); // Draws at center of target cube

            // Visualize the stored vector in the editor
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(worldPosition, worldPosition + dictValue * dataMultiplier);

            // Visualize the stored vector in the editor with an arrow
            DrawArrow(worldPosition, dictValue * dataMultiplier, targetVectorColor);

            // Draw the neighboring cells offset from the target cell
            if (showNeighboorsCells)
            {
                DrawNeighbor(cellPosition.x, cellPosition.y + 1, cellPosition.z);
                DrawNeighbor(cellPosition.x, cellPosition.y - 1, cellPosition.z);
                DrawNeighbor(cellPosition.x, cellPosition.y, cellPosition.z - 1);
                DrawNeighbor(cellPosition.x, cellPosition.y, cellPosition.z + 1);
                DrawNeighbor(cellPosition.x + 1, cellPosition.y, cellPosition.z);
                DrawNeighbor(cellPosition.x - 1, cellPosition.y, cellPosition.z);
            }

            // Draws target's red cube cell last to draw on top
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(worldPosition, Vector3.one * bakedData.unitSize);
        }
    }

    private void DrawNeighbor(int x, int y, int z)
    {
        if (bakedData.IsValidCell(x, y, z))
        {
            Vector3 neighborPos = bakedData.CellToWorldPostion(x, y, z);
            Gizmos.color = cellNeighborColor;
            Gizmos.DrawWireCube(neighborPos, Vector3.one * bakedData.unitSize);
        }
    }

    // Replace the DrawArrow method in GravityBakerTester with this:
    private void DrawArrow(Vector3 start, Vector3 direction, Color color)
    {
        // Store original color and set new color
        Color originalColor = Gizmos.color;
        Gizmos.color = color;

        // Draw the arrow
        GizmoArrowUtility.DrawArrow(start, direction, arrowHeadSize * 2, 20.0f, 8);

        // Restore original color
        Gizmos.color = originalColor;
    }

    #endregion
}
