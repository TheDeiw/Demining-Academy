using UnityEngine;
using System.Collections.Generic;

public class GrassSpawner : MonoBehaviour
{
    [Header("Main Settings")]
    public GameObject grassPrefab; 
    public Collider groundCollider; 
    public LayerMask groundLayer;

    [Header("Variation")]
    public float minScale = 0.8f; // NEW: Minimum size
    public float maxScale = 1.2f; // NEW: Maximum size

    [Header("Obstacle Avoidance")]
    public LayerMask obstacleLayer; 
    public float obstacleCheckRadius = 0.5f; 

    [Header("Grid Config")]
    public float chunkSize = 10f; 
    public float density = 2f;    

    [Header("LOD Settings")]
    [Range(0.01f, 0.9f)] 
    public float lodTransition = 0.3f; 
    public float lowQualityPercentage = 0.25f; 

    void Start()
    {
        if (grassPrefab == null || groundCollider == null) return;
        GenerateGrid();
    }

    void GenerateGrid()
    {
        Bounds bounds = groundCollider.bounds;
        for (float x = bounds.min.x; x < bounds.max.x; x += chunkSize)
        {
            for (float z = bounds.min.z; z < bounds.max.z; z += chunkSize)
            {
                // We keep the Y at the center, but rays will start higher
                Vector3 chunkCenter = new Vector3(x + chunkSize / 2, bounds.center.y, z + chunkSize / 2);
                CreateChunk(chunkCenter, new Vector2(chunkSize, chunkSize));
            }
        }
    }

    void CreateChunk(Vector3 center, Vector2 size)
    {
        GameObject chunkRoot = new GameObject($"Chunk_{center.x}_{center.z}");
        chunkRoot.transform.SetParent(this.transform);
        chunkRoot.transform.position = center; 

        List<CombineInstance> highDetail = new List<CombineInstance>();
        List<CombineInstance> lowDetail = new List<CombineInstance>();

        int grassCount = Mathf.CeilToInt(size.x * size.y * density);

        for (int i = 0; i < grassCount; i++)
        {
            // Start 50 units ABOVE the chunk center to ensure we hit the hills
            Vector3 pos = new Vector3(
                Random.Range(-size.x / 2, size.x / 2),
                50f, 
                Random.Range(-size.y / 2, size.y / 2)
            );
            
            Vector3 worldRayStart = chunkRoot.transform.TransformPoint(pos);

            // Raycast down (Distance 100f covers 50f up + 50f down variation)
            if (Physics.Raycast(worldRayStart, Vector3.down, out RaycastHit hit, 100f, groundLayer))
            {
                // Obstacle Check
                if (Physics.CheckSphere(hit.point, obstacleCheckRadius, obstacleLayer))
                {
                    continue; 
                }

                // Random Rotation
                Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal) * Quaternion.Euler(0, Random.Range(0, 360), 0);
                
                // NEW: Use the custom Min/Max scale
                Vector3 scale = Vector3.one * Random.Range(minScale, maxScale);
                
                Matrix4x4 baseMatrix = Matrix4x4.TRS(chunkRoot.transform.InverseTransformPoint(hit.point), rot, scale);

                MeshFilter[] filters = grassPrefab.GetComponentsInChildren<MeshFilter>();
                foreach (MeshFilter mf in filters)
                {
                    CombineInstance ci = new CombineInstance();
                    ci.mesh = mf.sharedMesh;
                    ci.transform = baseMatrix * mf.transform.localToWorldMatrix; 

                    highDetail.Add(ci);
                    if (Random.value < lowQualityPercentage) lowDetail.Add(ci);
                }
            }
        }

        if (highDetail.Count > 0)
        {
            GameObject goHigh = CreateMeshObject("LOD0_High", chunkRoot.transform, highDetail);
            GameObject goLow = CreateMeshObject("LOD1_Low", chunkRoot.transform, lowDetail);

            LODGroup lodGroup = chunkRoot.AddComponent<LODGroup>();
            LOD[] lods = new LOD[2];

            lods[0] = new LOD(lodTransition, new Renderer[] { goHigh.GetComponent<Renderer>() });
            lods[1] = new LOD(0.05f, new Renderer[] { goLow.GetComponent<Renderer>() });

            lodGroup.SetLODs(lods);
            lodGroup.RecalculateBounds();
        }
        else
        {
            Destroy(chunkRoot);
        }
    }

    GameObject CreateMeshObject(string name, Transform parent, List<CombineInstance> instances)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        
        MeshFilter mf = go.AddComponent<MeshFilter>();
        MeshRenderer mr = go.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.CombineMeshes(instances.ToArray(), true, true);
        
        mf.mesh = mesh;
        mr.sharedMaterial = grassPrefab.GetComponentInChildren<MeshRenderer>().sharedMaterial;
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return go;
    }
}