using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[CreateAssetMenu(menuName = "Gravity Data")]
public class BakedGravityData : ScriptableObject
{
    // Dictionary for storing vector data
    [SerializeField]
    private SerializableDictionary<Vector3Int, Vector3> _dictionaryVectorData = new SerializableDictionary<Vector3Int, Vector3>();

    public Dictionary<Vector3Int, Vector3> dictionaryVectorData
    {
        get
        {
            if (_dictionaryVectorData == null)
                _dictionaryVectorData = new SerializableDictionary<Vector3Int, Vector3>();
            return _dictionaryVectorData;
        }
    }

    // The size of the cell array in 3D
    public Vector3Int cellSize;

    // World unit size = 1 meter
    public int unitSize = 1;

    public Vector3 offsets;
    //public Vector3 offsets2 => offsets;

    public bool centerToWorld = true;

    // Cache the offset calculation
    private bool _offsetsCalculated = false;


    public void CreateCellsAndSetData()
    {
        _offsetsCalculated = false; // Reset offset flag
        InitializeEmptyDictionary();
    }


    public void RandomizeData()
    {
        InitializeRandomDictionary();
    }

    private void InitializeRandomDictionary()
    {
        _dictionaryVectorData = new SerializableDictionary<Vector3Int, Vector3>();

        for (int y = 0; y < cellSize.y; y++)
        {
            for (int z = 0; z < cellSize.z; z++)
            {
                for (int x = 0; x < cellSize.x; x++)
                {
                    Vector3Int newKey = new Vector3Int(x, y, z);
                    Vector3 randomData = Random.onUnitSphere;
                    _dictionaryVectorData.Add(newKey, randomData);
                }
            }
        }
    }

    /// <summary>
    /// Gets gravity value from world position
    /// </summary>
    public Vector3 GetGravityAtPosition(Vector3 worldPosition)
    {
        Vector3Int cell = GetKey(worldPosition);

        if (IsValidCell(cell) && dictionaryVectorData.TryGetValue(cell, out Vector3 gravityDir))
        {
            return gravityDir;
        }

        return Vector3.zero; // Default gravity if not found
    }

    /// <summary>
    /// Converts from world position to key
    /// </summary>
    public Vector3Int GetKey(Vector3 worldPosition)
    {
        EnsureOffsetIsCalculated();

        // Scale by unit size
        worldPosition /= unitSize;

        // Add offsets
        worldPosition.x += offsets.x;
        worldPosition.y += offsets.y;
        worldPosition.z += offsets.z;

        // Clamp domain to array size
        int x = Mathf.Clamp(Mathf.RoundToInt(worldPosition.x), 0, cellSize.x - 1);
        int y = Mathf.Clamp(Mathf.RoundToInt(worldPosition.y), 0, cellSize.y - 1);
        int z = Mathf.Clamp(Mathf.RoundToInt(worldPosition.z), 0, cellSize.z - 1);

        return new Vector3Int(x, y, z);
    }

    /// <summary>
    /// Returns the world position from cell indices
    /// </summary>
    public Vector3 CellToWorldPostion(int x, int y, int z)
    {
        EnsureOffsetIsCalculated();

        Vector3 position = new Vector3(x, y, z);

        position.x -= offsets.x;
        position.y -= offsets.y;
        position.z -= offsets.z;

        position *= unitSize;

        return position;
    }

    public bool IsValidCell(Vector3Int cellPosition)
    {
        return dictionaryVectorData.ContainsKey(cellPosition) &&
               cellPosition.x >= 0 && cellPosition.x < cellSize.x &&
               cellPosition.y >= 0 && cellPosition.y < cellSize.y &&
               cellPosition.z >= 0 && cellPosition.z < cellSize.z;
    }

    public bool IsValidCell(int x, int y, int z)
    {
        return IsValidCell(new Vector3Int(x, y, z));
    }



#if UNITY_EDITOR
    public void SaveScriptableObjectData()
    {
        EditorUtility.SetDirty(this); // Marks the ScriptableObject as modified
        AssetDatabase.SaveAssets();   // Writes all unsaved changes to disk
        Debug.Log("Saved gravity data!");
    }
#endif


    // Initialize dictionary with zero vectors
    private void InitializeEmptyDictionary()
    {
        _dictionaryVectorData = new SerializableDictionary<Vector3Int, Vector3>();

        for (int y = 0; y < cellSize.y; y++)
        {
            for (int z = 0; z < cellSize.z; z++)
            {
                for (int x = 0; x < cellSize.x; x++)
                {
                    Vector3Int newKey = new Vector3Int(x, y, z);
                    _dictionaryVectorData.Add(newKey, Vector3.zero);
                }
            }
        }
    }

    public void BakeGravityFromSources()
    {
        // First clear or initialize the dictionary
        InitializeEmptyDictionary();

        // Get all gravity sources in the scene
        GravitySource[] sources = Object.FindObjectsByType<GravitySource>(FindObjectsSortMode.None);
        Debug.Log($"Found {sources.Length} gravity sources to bake");

        // Create a separate list of keys to iterate over
        var cellKeys = new List<Vector3Int>(_dictionaryVectorData.Keys);

        // For each cell in our grid
        foreach (var cell in cellKeys)
        {
            // Get the world position of this cell
            Vector3 worldPos = CellToWorldPostion(cell.x, cell.y, cell.z);
            Vector3 totalGravity = Vector3.zero;

            // Sum contributions from all gravity sources
            foreach (var source in sources)
            {
                totalGravity += source.CalculateGravityContribution(worldPos);
            }

            // Store the result
            _dictionaryVectorData[cell] = totalGravity;

        }

        SaveScriptableObjectData();
    }

    /// <summary>
    /// The offset is used to center the array to the world.
    /// Calculate only once unless values change.
    /// </summary>
    private void EnsureOffsetIsCalculated()
    {
        if (_offsetsCalculated) return;

        //Universal offset to center the grid
        offsets.x = (cellSize.x / 2.0f) - 0.5f;

        // Y offset 
        offsets.y = centerToWorld ? (cellSize.y / 2.0f) - 0.5f : -0.5f;

        offsets.z = (cellSize.z / 2.0f) - 0.5f;

        _offsetsCalculated = true;
    }

    

   
}