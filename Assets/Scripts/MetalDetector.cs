using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DynamicGrassSpawner : MonoBehaviour
{
    [Header("Main Settings")]
    public Transform playerTransform; // Drag your VR Camera or XR Origin here
    public GameObject grassPrefab; 
    public Collider groundCollider; 
    public LayerMask groundLayer;

    [Header("Radius & Performance")]
    public int chunkRenderDistance = 3; // Radius in chunks (3 = ~60m range). Keep low for Quest!
    public float chunkSize = 20f; // Size of one square grid
    public int maxChunksGeneratedPerFrame = 1; // Limit to 1 to prevent stutter

    [Header("Variation")]
    public float minScale = 1.0f; 
    public float maxScale = 1.5f; 

    [Header("Obstacle Avoidance")]
    public LayerMask obstacleLayer; 
    public float obstacleCheckRadius = 0.5f; 

    [Header("Density & LOD")]
    public float density = 0.8f; 
    [Range(0.01f, 0.9f)] public float lodTransition = 0.2f; 
    public float lowQualityPercentage = 0.15f; 

    // Internal tracking
    private Dictionary<Vector2Int, GameObject> activeChunks = new Dictionary<Vector2Int, GameObject>();
    private Vector2Int currentChunkCoord;
    private Bounds mapBounds;

    void Start()
    {
        if (grassPrefab == null || groundCollider == null || playerTransform == null)
        {
            Debug.LogError("Missing Assignments! Assign Player, Prefab, and Ground.");
            return;
        }
        mapBounds = groundCollider.bounds;
        StartCoroutine(UpdateChunksRoutine());
    }

    IEnumerator UpdateChunksRoutine()
    {
        while (true)
        {
            // 1. Where is the player on the grid?
            Vector3 playerPos = playerTransform.position;
            int pX = Mathf.RoundToInt(playerPos.x / chunkSize);
            int pZ = Mathf.RoundToInt(playerPos.z / chunkSize);
            Vector2Int newPlayerChunk = new Vector2Int(pX, pZ);

            // 2. Only update if player moved to a new chunk (or at start)
            if (newPlayerChunk != currentChunkCoord || activeChunks.Count == 0)
            {
                currentChunkCoord = newPlayerChunk;
                yield return UpdateVisibleChunks();
            }

            // Check every 0.5 seconds to save CPU
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator UpdateVisibleChunks()
    {
        List<Vector2Int> chunksToCreate = new List<Vector2Int>();
        List<Vector2Int> chunksToRemove = new List<Vector2Int>(activeChunks.Keys);

        // A. Identify what should be visible
        for (int xOffset = -chunkRenderDistance; xOffset <= chunkRenderDistance; xOffset++)
        {
            for (int zOffset = -chunkRenderDistance; zOffset <= chunkRenderDistance; zOffset++)
            {
                Vector2Int coord = new Vector2Int(currentChunkCoord.x + xOffset, currentChunkCoord.y + zOffset);

                // If this coordinate is currently active, don't remove it
                if (chunksToRemove.Contains(coord))
                {
                    chunksToRemove.Remove(coord);
                }
                // If it's not active, we might need to create it
                else if (!activeChunks.ContainsKey(coord))
                {
                    chunksToCreate.Add(coord);
                }
            }
        }

        // B. Remove Far Chunks (Free up memory immediately)
        foreach (var coord in chunksToRemove)
        {
            Destroy(activeChunks[coord]);
            activeChunks.Remove(coord);
        }

        // C. Create New Chunks (Spread over frames to avoid lag)
        int chunksProcessed = 0;
        foreach (var coord in chunksToCreate)
        {
            // Calculate world position
            Vector3 chunkCenter = new Vector3(coord.x * chunkSize, mapBounds.center.y, coord.y * chunkSize);

            // Check if this chunk is actually inside the game map
            if (mapBounds.Contains(new Vector3(chunkCenter.x, mapBounds.center.y, chunkCenter.z)))
            {
                CreateChunk(coord, chunkCenter);
                chunksProcessed++;

                // Pause if we did too much work this frame
                if (chunksProcessed >= maxChunksGeneratedPerFrame)
                {
                    chunksProcessed = 0;
                    yield return null; 
                }
            }
        }
    }

    void CreateChunk(Vector2Int coord, Vector3 center)
    {
        GameObject chunkRoot = new GameObject($"Chunk_{coord.x}_{coord.y}");
        chunkRoot.transform.SetParent(this.transform);
        chunkRoot.transform.position = center; 
        
        // Add to dictionary immediately so we don't create it twice
        activeChunks.Add(coord, chunkRoot);

        List<CombineInstance> highDetail = new List<CombineInstance>();
        List<CombineInstance> lowDetail = new List<CombineInstance>();

        int grassCount = Mathf.CeilToInt(chunkSize * chunkSize * density);

        for (int i = 0; i < grassCount; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-chunkSize / 2, chunkSize / 2),
                50f, 
                Random.Range(-chunkSize / 2, chunkSize / 2)
            );
            
            Vector3 worldRayStart = chunkRoot.transform.TransformPoint(pos);

            if (Physics.Raycast(worldRayStart, Vector3.down, out RaycastHit hit, 100f, groundLayer))
            {
                if (Physics.CheckSphere(hit.point, obstacleCheckRadius, obstacleLayer)) continue;

                Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal) * Quaternion.Euler(0, Random.Range(0, 360), 0);
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
            lods[1] = new LOD(0.02f, new Renderer[] { goLow.GetComponent<Renderer>() });
            lodGroup.SetLODs(lods);
            lodGroup.RecalculateBounds();
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
        mr.receiveShadows = false;
        return go;
    }
}