using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GrassSpawner : MonoBehaviour
{
    [Header("Main Settings")]
    public Transform playerTransform; // Drag your VR Camera or XR Origin here
    public GameObject grassPrefab; 
    public Collider[] groundColliders; // Assign EVERY terrain/ground piece here
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

    [Header("Density")]
    public float density = 0.8f;

    // Internal tracking
    private Dictionary<Vector2Int, GameObject> activeChunks = new Dictionary<Vector2Int, GameObject>();
    private Vector2Int currentChunkCoord;
    private Bounds[] groundBoundsArray;

    void Start()
    {
        if (grassPrefab == null || groundColliders == null || groundColliders.Length == 0 || playerTransform == null)
        {
            Debug.LogError("Missing Assignments! Assign Player, Prefab, and at least one Ground collider.");
            return;
        }

        groundBoundsArray = new Bounds[groundColliders.Length];
        for (int i = 0; i < groundColliders.Length; i++)
        {
            groundBoundsArray[i] = groundColliders[i].bounds;
        }

        StartCoroutine(UpdateChunksRoutine());
    }

    // Finds which terrain's bounds contain this XZ position (ignoring Y),
    // and returns that terrain's bounds so we can use its correct height.
    // Using "any match", not a merged box, so gaps between separate terrain
    // pieces (e.g. islands) correctly stay empty instead of spawning chunks over nothing.
    bool TryGetGroundBounds(float worldX, float worldZ, out Bounds bounds)
    {
        for (int i = 0; i < groundBoundsArray.Length; i++)
        {
            Bounds b = groundBoundsArray[i];
            if (worldX >= b.min.x && worldX <= b.max.x && worldZ >= b.min.z && worldZ <= b.max.z)
            {
                bounds = b;
                return true;
            }
        }
        bounds = default;
        return false;
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
                Debug.Log($"[Grass] Player entered chunk {newPlayerChunk} (was {currentChunkCoord}). Updating...");
                currentChunkCoord = newPlayerChunk;
                yield return UpdateVisibleChunks();
                Debug.Log($"[Grass] Update finished. Active chunks now: {activeChunks.Count}");
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
            // Calculate world position — find which terrain (if any) this XZ falls on,
            // and use THAT terrain's height, since pieces may sit at different elevations
            if (TryGetGroundBounds(coord.x * chunkSize, coord.y * chunkSize, out Bounds hitBounds))
            {
                Vector3 chunkCenter = new Vector3(coord.x * chunkSize, hitBounds.center.y, coord.y * chunkSize);

                try
                {
                    CreateChunk(coord, chunkCenter);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[Grass] Failed to create chunk {coord}: {e.Message}\n{e.StackTrace}");
                    // Make sure a failed chunk doesn't stay "reserved" forever
                    if (activeChunks.ContainsKey(coord)) activeChunks.Remove(coord);
                }
                chunksProcessed++;

                // Pause if we did too much work this frame
                if (chunksProcessed >= maxChunksGeneratedPerFrame)
                {
                    chunksProcessed = 0;
                    yield return null;
                }
            }
            else
            {
                Debug.Log($"[Grass] Chunk {coord} (world X={coord.x * chunkSize}, Z={coord.y * chunkSize}) is outside all assigned ground colliders, skipped.");
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

        List<CombineInstance> grassInstances = new List<CombineInstance>();

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

                    grassInstances.Add(ci);
                }
            }
        }

        if (grassInstances.Count > 0)
        {
            CreateMeshObject("Grass", chunkRoot.transform, grassInstances);
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