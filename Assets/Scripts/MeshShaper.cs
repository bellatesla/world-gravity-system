using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
[ExecuteInEditMode]
public class MeshShaper : MonoBehaviour
{
    public float vertHeight = 1;
    [Range(1, 10)] public float maxHeight = 2;
    [Range(1, 100)] public int boundsPercentSize = 10;
    public Bounds bounds;

    public Arrays arrays;

    public Vector3[] vertices;
    public Vector3[] normals;
    public Vector2[] uvs_cached;
    public int[] tris_cached;

    public Vector3[] vertices_cached;
    public Vector3[] normals_cached;

    public List<Vector3> vert_edges;

    public Mesh originalMesh; //The rock mesh we want to alter
    public Mesh newMesh; //the copy of the original rock mesh
    private Vector3 scale;
    [System.Serializable]
    public struct Arrays
    {
        public int[][][] matrixXYZ;//= new int[][][];
        public int[][] matrixXYZ2; // = new int[3][][];
        public int[][][] matrixXYZ3; // = new int[3][][];
        public int[] matrixXYZ4; // = new int[3][][];

    }

    public bool isActive;
    public Color color = Color.white;
    public void StoreMesh()
    {
        //The current assign mesh
        originalMesh = GetComponent<MeshFilter>().sharedMesh;
        vertices_cached = originalMesh.vertices; //Dont invert statement very bad
        normals_cached = originalMesh.normals;
        uvs_cached = originalMesh.uv;
        tris_cached = originalMesh.triangles;
        bounds = GetComponent<MeshRenderer>().bounds;
    }
    public void RestoreMesh()
    {
        //The current assign mesh
        GetComponent<MeshFilter>().sharedMesh = originalMesh;
        bounds = GetComponent<MeshRenderer>().bounds;
        StoreMesh();
        vert_edges.Clear();
    }

    /// <summary>
    /// Get the orig mesh but apply it to a new mesh
    /// </summary>
    public void WarpMesh()
    {
        if (isActive) return;
        isActive = true;
        if (originalMesh == null)
        {
            StoreMesh();
            Debug.Log("original was null ");
        }

        //bounds = new Bounds(transform.position, transform.localScale * boundsPercentSize); //10 for a plane usually 1 for anything else

        //vertices = originalMesh.vertices;
        //normals = originalMesh.normals;
        vertices = vertices_cached;
        normals = normals_cached;

        scale = transform.localScale;

        for (int i = 0; i < vertices.Length; i++)
        {
            //is the vertex forward too bigger than the bounds forward?
            //We only need to look at vertex x and z pos compared to bounds x and z 
            //**cool behavior that because of the equals sign the vets dont move on the edge. ie simple edge detection

            if (vertices[i].x*scale.x >= bounds.extents.x)
            {
                Debug.Log("OuT X:" + bounds.extents.x);
                vert_edges.Add(vertices[i]);
            }
            else if (vertices[i].x*scale.x <= -bounds.extents.x)
            {
                Debug.Log("OuT -X:" + bounds.extents.x);
                vert_edges.Add(vertices[i]);
            }
            else if (vertices[i].z*scale.z >= bounds.extents.z)
            {
                Debug.Log("OuT Z:" + bounds.extents.x);
                vert_edges.Add(vertices[i]);
            }
            else if (vertices[i].z*scale.z <= -bounds.extents.z)
            {
                Debug.Log("OuT -Z:" + bounds.extents.x);
                vert_edges.Add(vertices[i]);
            }
            else
            {
                var time = System.DateTime.Now.GetHashCode();
                //vertices[i].y += (normals[i].y*Mathf.Sin(time))*(Random.Range(-.2f, 1.1f)*maxHeight); //todo sort random later
                //vertices[i].y = vertHeight;
            }
        }

        //Test//
        Bounds b = new Bounds(bounds.center, bounds.size * boundsPercentSize * .01f);
        scale = transform.localScale;
        if (drawVerts)
        {
            for (int i = 0; i < vertices_cached.Count(); i++)
            {
                Vector3 vertPos = new Vector3(vertices_cached[i].x * scale.x, vertices_cached[i].y * scale.y, vertices_cached[i].z * scale.z);
                if (vertPos.y >= b.extents.y)
                {
                    vertices[i].y = vertHeight; 
                }
                //Gizmos.DrawSphere(vertPos + transform.position, 1);
            }
        }




        newMesh = new Mesh();
        GetComponent<MeshFilter>().sharedMesh = newMesh;

        newMesh.vertices = vertices;
        newMesh.normals = normals;
        newMesh.uv = uvs_cached;
        newMesh.triangles = tris_cached;
        isActive = false;
    }
    public bool drawBounds;
    public bool drawEdges;
    public bool drawVerts;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Bounds b = new Bounds(bounds.center, bounds.size*boundsPercentSize*.01f);
        scale = transform.localScale;
        if (drawVerts)
        {
            for (int i = 0; i < vertices_cached.Count(); i++)
            {
                Vector3 vertPos = new Vector3(vertices_cached[i].x*scale.x, vertices_cached[i].y*scale.y, vertices_cached[i].z*scale.z);
                if (vertPos.x >= b.extents.x) Gizmos.color = Color.red;
                else if (vertPos.y >= b.extents.y) Gizmos.color = Color.green;
                else if (vertPos.z >= b.extents.z) Gizmos.color = Color.blue;
                else
                {
                    Gizmos.color = Color.white;
                }
                Gizmos.DrawSphere(vertPos + transform.position, 1);
            }
        }
        //Gizmos.color = Color.green;

        //if (drawEdges)
        //    for (int i = 0; i < vert_edges.Count(); i++)
        //    {
        //        Vector3 pos = new Vector3(vert_edges[i].x*scale.x, vert_edges[i].y*scale.y, vert_edges[i].z*scale.z);

        //        Gizmos.DrawSphere(pos + transform.position, 1);
        //    }

        Gizmos.color = color;
        if (drawBounds) Gizmos.DrawCube(b.center, b.size);
    }
}