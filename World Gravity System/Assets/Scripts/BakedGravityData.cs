using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Gravity Data")]
public class BakedGravityData : ScriptableObject // 24 bytes 
{

    // Stored data in cells
    public Dictionary<Vector3Int, Vector3> dictionaryVectorData;


    // The size of the cell array in 3D
    [SerializeField]
    private Vector3Int _cellSize;


    // World unit size = 1 meter
    public int unitSize = 1;   

    
    [SerializeField]
    private Vector3 _offsets;
    public bool centerToWorld;


    public void CreateCellsAndSetData(Vector3Int cellSize)
    {
        _cellSize = cellSize;        
        RandomizeArrayValues();

    }



    /// <summary>
    /// Set some world data for now
    /// </summary>
    private void RandomizeArrayValues()
    {


        dictionaryVectorData = new Dictionary<Vector3Int, Vector3>();

        for (int y = 0; y < _cellSize.y; y++)
        {

            for (int z = 0; z < _cellSize.z; z++)
            {

                for (int x = 0; x < _cellSize.x; x++)
                {

                    Vector3Int newKey = new Vector3Int(x, y, z);
                    Vector3 randomData = Random.onUnitSphere;
                    dictionaryVectorData.Add(newKey, randomData);

                }
            }
        }
    }    

   

    /// <summary>
    /// The offset is used to center the array to the world.
    /// We need to add the proper offsets to center the cube compared to player position
    /// </summary>
    void SetUniversalOffset()
    {
        //Universal offset of 1
        _offsets.x = (_cellSize.x / 2.0f) - .5f; //the .5f is 1/2 of a block unit of 1
        
        // Y offset to 0
        if (centerToWorld)
        {
            _offsets.y = (_cellSize.y / 2.0f) - .5f;
        }
        else
        {
            _offsets.y = -.5f;
        }

        _offsets.z = (_cellSize.z / 2.0f) - .5f;


    }    



   /// <summary>
   /// Converts from the target position to an offset array world position
   /// </summary>
   /// <param name="player"></param>
   /// <returns></returns>
    public Vector3Int WorldPositionToCellPosition(Vector3 position)
    {
        SetUniversalOffset();

        //divide by unit size
        position /= unitSize;
        
        //add offsets
        position.x += _offsets.x;
        position.y += _offsets.y;
        position.z += _offsets.z;

        //Clamp domain to array size
        int x = (short)Mathf.Clamp(Mathf.RoundToInt(position.x), 0, _cellSize.x - 1);
        int y = (short)Mathf.Clamp(Mathf.RoundToInt(position.y), 0, _cellSize.y - 1);
        int z = (short)Mathf.Clamp(Mathf.RoundToInt(position.z), 0, _cellSize.z - 1);


        return new Vector3Int(x,y,z);
    }



    /// <summary>
    /// Returns the world position of the 3D array from the array indecies. 
    /// </summary>
    public Vector3 CellToWorldPostion(int x, int y, int z)
    {
        SetUniversalOffset();
        

        Vector3 position = new Vector3(x, y, z);

        position.x -= _offsets.x;
        position.y -= _offsets.y;
        position.z -= _offsets.z;

        position *= unitSize;

        return position;
    }

    public bool IsValidCell(Vector3Int cellPosition)
    {
        return  dictionaryVectorData.ContainsKey(cellPosition);
    }


    public bool IsValidCell(int x, int y, int z)
    {        

        return IsValidCell(new Vector3Int(x, y, z));
    }
}