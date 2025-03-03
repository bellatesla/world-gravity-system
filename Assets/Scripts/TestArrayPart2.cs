using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Drag and drop onto any empty game object and reset position to zero. 
/// Move the transform handles around in the Scene view to move to position in grid and see the closest point.
/// A test class for visually testing 3D Arrays and as a reference to a position. Draws nice visuals with adjustments.
/// </summary>
[ExecuteInEditMode]
public class TestArrayPart2 : MonoBehaviour
{
    public Transform target;

    //public BakedGravityData bakedData;

    [Space, Header("Set 3D Array Sizes")]
    public int sizeX = 10;
    public int sizeY = 10;
    public int sizeZ = 10;
    public int unitSize = 10;

    public Colors color;

    [Range(1f, 100f)]
    public float boxShrink = 100f;

    public WorldInt worldInts;

    public Vector3[,,] gridCubes; //one x,y,z data in a cube

    private Vector3 targetRemappedPosition; //this position is converted to ints [x,y,z] and using that info look up array matrixvalues.

    private Vector3 offset;


    [System.Serializable]
    public class Colors 
    {
        public Color grid = Color.white;
        public Color data = Color.blue;
        public Color active = Color.red;
         
    }

    [System.Serializable]
    public struct WorldInt
    {

        public short x;
        public short y;
        public short z;

        
        public void ClampDomainAndConvert(Vector3 pos, int sizeX, int sizeY, int sizeZ)
        {
            x = (short) Mathf.Clamp(Mathf.RoundToInt(pos.x), 0, sizeX - 1);
            y = (short) Mathf.Clamp(Mathf.RoundToInt(pos.y), 0, sizeY - 1);
            z = (short) Mathf.Clamp(Mathf.RoundToInt(pos.z), 0, sizeZ - 1);
        }
    }


    void OnEnable()
    {

        RandomizeArrayValues();
       
        //test
        int size1 = System.Runtime.InteropServices.Marshal.SizeOf(typeof(WorldInt));
        print(size1);
        int size2 = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector3));
        print(size2);

    }


    public void RandomizeArrayValues()
    {
        float startTime = System.DateTime.Now.Millisecond;

        gridCubes = new Vector3[sizeX, sizeY, sizeZ];

        SetUniversalOffset();//We need to add the proper offsets to center the cube compared to player position
       
        for (int y = 0; y < sizeY; y++)
        {

            for (int z = 0; z < sizeZ; z++)
            {

                for (int x = 0; x < sizeX; x++)
                {

                    gridCubes[x, y, z] = Random.onUnitSphere; //Store a random vector3
                   
                }
            }
        }

        float endtime = System.DateTime.Now.Millisecond - startTime;

        Debug.Log("Time Taken: " + endtime + " ms");


        #region Web_Source


        /** https://msdn.microsoft.com/en-us/library/2yd9wwz4.aspx **/
        // **from msdn, int[,] array6 = new int[10, 10];
        //var allLength = matrix.Length;
        //var total = 1;
        //for (int i = 0; i < matrix.Rank; i++)
        //{
        //    total *= matrix.GetLength(i);
        //}

        //Debug.Log("Matrix Size: " + total);
        //Debug.Log("Matrix Size: " + allLength);


        #endregion
    }

    void SetUniversalOffset()
    {
        //Universal Offset of 1
        offset.x = (sizeX / 2.0f)  - .5f;//the .5f is 1/2 of a block unit of 1
        offset.y = (sizeY / 2.0f)  - .5f;
        offset.z = (sizeZ / 2.0f)  - .5f;
        
    }
    void OnDrawGizmos()
    {
        
        //Re-map pos
        targetRemappedPosition = target.localPosition;
        targetRemappedPosition /= unitSize;
        targetRemappedPosition += offset;
        //set domain
        worldInts.ClampDomainAndConvert(targetRemappedPosition,sizeX, sizeY, sizeZ);
       
        //slider for visual box size
        Vector3 boxsize = Vector3.one*(boxShrink*.01f*unitSize);
        Gizmos.DrawLine(target.localPosition, target.localPosition + gridCubes[worldInts.x,worldInts.y, worldInts.z]);

        //find player data
        for (int y = 0; y < sizeY; y++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    //Un-map position  = V3(x,y,z) - offset * unitSize
                    Vector3 pos = new Vector3(x, y, z);
                    pos -= offset;
                    pos *= unitSize;

                    //Vector3 currentBoxValue = gridCubes[x, y, z];//type value

                    //Show player position and data in red (activeColor) 
                    if (x == worldInts.x && y == worldInts.y && z == worldInts.z)
                    {
                        Gizmos.color = color.data;
                        Gizmos.DrawLine(pos, pos + gridCubes[x, y, z]);

                        Gizmos.color = color.active;
                        Gizmos.DrawCube(pos, boxsize);

                    }
                    else
                    {
                        Gizmos.color = color.data;
                        Gizmos.DrawLine(pos, pos + gridCubes[x, y, z]);
                        Gizmos.color = color.grid;
                        Gizmos.DrawWireCube(pos, boxsize);

                    }


                }
            }
        }
    }
}