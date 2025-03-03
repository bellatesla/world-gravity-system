using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class GravityBakerTester : MonoBehaviour
{

    public Transform target;

    public Vector3Int cellDimension = Vector3Int.one;

    public BakedGravityData bakedData;

    public bool showNeighboorsCells;

    public Color cellColor = new Color(1,1,1,.3f);


    private void OnEnable()
    {
        bakedData.CreateCellsAndSetData(cellDimension);
    }


    void OnDrawGizmos()
    {    
        // Draw the whole cell grid
        for (int y = 0; y < cellDimension.y; y++)
        {
            for (int z = 0; z < cellDimension.z; z++)
            {
                for (int x = 0; x < cellDimension.x; x++)
                {
                    Gizmos.color = cellColor;
                    Vector3 pos = bakedData.CellToWorldPostion(x, y, z);
                    Gizmos.DrawWireCube(pos, Vector3.one * bakedData.unitSize);                  

                }
            }
        }


        // Get the cell position of the target and draw it
        Vector3Int cellPosition = bakedData.WorldPositionToCellPosition(target.position);

       // The cell position is the dictionary key
        if (bakedData.dictionaryVectorData.TryGetValue(cellPosition, out Vector3 dictValue))
        {
            // a world position from the key
            Vector3 world = bakedData.CellToWorldPostion(cellPosition.x, cellPosition.y, cellPosition.z);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(world, .1f);//draws at center of target cube
            
            // Visualize the stored vector in the editor
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(world, world + dictValue);

            // Draw the neighbooring cells offset from the target cell
            if (showNeighboorsCells)
            {
                Vector3 up = bakedData.CellToWorldPostion(cellPosition.x, cellPosition.y + 1, cellPosition.z);
                Vector3 down = bakedData.CellToWorldPostion(cellPosition.x, cellPosition.y - 1, cellPosition.z);
                Vector3 back = bakedData.CellToWorldPostion(cellPosition.x, cellPosition.y, cellPosition.z - 1);
                Vector3 forward = bakedData.CellToWorldPostion(cellPosition.x, cellPosition.y, cellPosition.z + 1);
                Vector3 right = bakedData.CellToWorldPostion(cellPosition.x + 1, cellPosition.y, cellPosition.z);
                Vector3 left = bakedData.CellToWorldPostion(cellPosition.x - 1, cellPosition.y, cellPosition.z);
                
                // TODO: Add validation method to stay inbouds.
                
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(up, Vector3.one * bakedData.unitSize);
                Gizmos.DrawWireCube(down, Vector3.one * bakedData.unitSize);
                Gizmos.DrawWireCube(left, Vector3.one * bakedData.unitSize);
                Gizmos.DrawWireCube(right, Vector3.one * bakedData.unitSize);
                Gizmos.DrawWireCube(forward, Vector3.one * bakedData.unitSize);
                Gizmos.DrawWireCube(back, Vector3.one * bakedData.unitSize);
            }

            //Draws targets red cube cell last to draw on top
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(world, Vector3.one * bakedData.unitSize);


        }  
    }
}