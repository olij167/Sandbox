using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//using Pathfinding;

// requires a nav mesh in the scene
public class SpawnObjects : MonoBehaviour
{
    public bool showDebug;

    public Transform parent;
    public List<GameObject> prefabs;

    public bool spawningActive;

    Vector3 spawnPos;
    public int spawnNumPerPrefab, totalSpawnNum;

    public bool randomPrefabs;

    public Transform spawnPoint;
    private Vector3 spawnArea;
    public float distanceFromGround;

    public bool spawnInRadius;
    public float spawnRadius = 10;

    public bool spawnOnTimer;
    [field: ReadOnlyField, SerializeField] private float timer;

    //public int maxSpawnAmount = 10;

    public float spawnRate = 30f;
    public bool fluctuateSpawnRate;
    public float fluctuationRange = 30f;

    public float minYPos = 0f;

    public List<GameObject> spawnedObjects;


    private void Awake()
    {
        if (spawnPoint == null)
            spawnPoint = transform;

        if (spawnOnTimer) timer = spawnRate;
    }

    public void Update()
    {
        if (!randomPrefabs) totalSpawnNum = spawnNumPerPrefab * prefabs.Count;


        if (spawningActive)
        {
            if (prefabs != null && prefabs.Count > 0)
                SpawnPrefabs(randomPrefabs, spawnInRadius);
        }

        if (spawnedObjects != null && spawnedObjects.Count > 0)
        {
            CheckIfDestroyedOrFalling();

            //if (spawnedObjects.Count > totalSpawnNum || spawnedObjects.Count > spawnNumPerPrefab * prefabs.Count)
            //    RemoveExcessObjects();
        }
    }

    public virtual void SpawnPrefabs(bool randomPrefabs, bool inRadius)
    {
        if (!randomPrefabs)
        {
            if (spawnedObjects.Count < spawnNumPerPrefab * prefabs.Count)
            {
                for (int i = 0; i < prefabs.Count; i++)
                {
                    if (CheckBeforeSpawning(prefabs[i]))
                    {
                        if (spawnOnTimer)
                        {
                            if (fluctuateSpawnRate)
                            {
                                SpawnSporatic(prefabs[i], parent, spawnRate, inRadius);
                            }
                            else
                            {
                                SpawnConsistent(prefabs[i], parent, spawnRate, inRadius);
                            }
                        }
                        else
                        {
                            SpawnConsistent(prefabs[i], parent, 0, inRadius);
                        }
                    }

                }
            }

        }
        else
        {
            if (spawnedObjects.Count < totalSpawnNum)
            {
                for (int i = 0; i < totalSpawnNum; i++)
                {

                    if (spawnOnTimer)
                    {
                        if (fluctuateSpawnRate)
                        {
                            SpawnSporatic(prefabs[Random.Range(0, prefabs.Count)], parent, spawnRate, inRadius);
                        }
                        else
                        {
                            SpawnConsistent(prefabs[Random.Range(0, prefabs.Count)], parent, spawnRate, inRadius);
                        }
                    }
                    else
                    {
                        SpawnConsistent(prefabs[Random.Range(0, prefabs.Count)], parent, 0, inRadius);
                    }

                }
            }
        }
    }

    public void CheckIfDestroyedOrFalling()
    {
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            if (spawnedObjects[i] == null)
            {
                spawnedObjects.RemoveAt(i);

                if (showDebug)
                    Debug.Log("Spawned Item " + i + " has been destroyed");
            }
            else if (spawnedObjects[i].transform.position.y < minYPos)
            {
                spawnedObjects[i].transform.position = new Vector3(spawnedObjects[i].transform.position.x, transform.position.y, spawnedObjects[i].transform.position.z);
            }
        }
    }

    public bool CheckBeforeSpawning(GameObject prefab)
    {
        int count = 0;

        for (int i = 0; i < spawnedObjects.Count; i++)
        {

            if (spawnedObjects[i].name.StartsWith( prefab.name))
            {

                count += 1;

            }

        }

        if (count < spawnNumPerPrefab)
        {
           // Debug.Log("Can Spawn " + prefab.name + ". [" + count +"/" + spawnNumPerPrefab + "]");
            return true;
        }
        else
        {
           // Debug.Log(prefab.name + " spawn limit reached. [" + count + "/" + spawnNumPerPrefab + "]");
            return false;
        }


        //return false;
    }

    public void RemoveExcessObjects()
    {
        if (!randomPrefabs)
        {
            for (int j = 0; j < prefabs.Count; j++)
            {
                for (int i = 0; i < spawnedObjects.Count; i++)
                {
                    int count = 0;

                    if (spawnedObjects[i] == prefabs[j])
                    {
                        if (count < spawnNumPerPrefab)
                            count++;
                        else
                        {
                            Destroy(spawnedObjects[i]);
                            spawnedObjects.RemoveAt(i);
                        }
                    }
                }
            }
        }
    }

    public void SpawnConsistent(GameObject prefab, Transform parent, float spawnRate, bool inRadius)
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {

            if (inRadius)
            {
                spawnArea = GenerateRandomPointWithinRadius();
                GameObject newObject = Instantiate(prefab, spawnArea, Quaternion.identity);
                newObject.transform.parent = parent;
                newObject.name = prefab.name;
                spawnedObjects.Add(newObject);

                ParkStats.instance.TrackObject(newObject);

            }
            else
            {
                spawnArea = spawnPoint.position;
                SpawnSpecificObjectAtRandomPos(prefab, parent);
            }
            timer = spawnRate;
        }

        //else
        //{
        //    //Debug.Log("Max Objects Reached");
        //    //return null;
        //}

    }

    public void SpawnSporatic(GameObject prefab, Transform parent, float spawnRate, bool inRadius)
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {

            if (inRadius)
            {
                spawnArea = GenerateRandomPointWithinRadius();
                GameObject newObject = Instantiate(prefab, spawnArea, Quaternion.identity);
                newObject.transform.parent = parent;
                newObject.name = prefab.name;
                spawnedObjects.Add(newObject);

                ParkStats.instance.TrackObject(newObject);

            }
            else
            {
                spawnArea = spawnPoint.position;
                SpawnSpecificObjectAtRandomPos(prefab, parent);
            }
            timer = Random.Range(spawnRate - fluctuationRange, spawnRate + fluctuationRange);
        }

    }

    public Vector3 SetDistanceFromGround()
    {
        RaycastHit hit;
        Vector3 floatDistance = spawnPoint.position;


        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            Vector3 pos = hit.point;

            Ray returnRay = new Ray(pos, transform.position - pos);

            floatDistance = returnRay.GetPoint(distanceFromGround);
        }

        return new Vector3(spawnArea.x, floatDistance.y, spawnArea.z);

    }

    public GameObject SpawnSpecificObjectAtRandomPos(GameObject prefab, Transform parent)
    {

        spawnPos = GenerateRandomWayPoint();
        //spawnPos = GetRandomPointOnGraph();
        GameObject newObject = Instantiate(prefab, spawnPos, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));
        newObject.transform.parent = parent;
        newObject.name = prefab.name;
        spawnedObjects.Add(newObject);

        ParkStats.instance.TrackObject(newObject);

        return newObject;

    }

    public void SpawnGameObjectsAtRandomPos(GameObject prefab, int numToSpawn)
    {

        for (int i = 0; i < numToSpawn; i++)
        {
            spawnPos = GenerateRandomWayPoint();
            //spawnPos = GetRandomPointOnGraph();
            GameObject newObject = Instantiate(prefab, spawnPos, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));
            newObject.transform.parent = parent;
            newObject.name = prefab.name;
            spawnedObjects.Add(newObject);

            ParkStats.instance.TrackObject(newObject);


        }

    }

    public Vector3 GenerateRandomPointWithinRadius()
    {
        spawnArea = spawnPoint.position + (spawnRadius * Random.insideUnitSphere);
        //spawnArea = new Vector3 (spawnArea.x, spawnPoint.position.y, spawnArea.z);
        spawnArea = SetDistanceFromGround();
        return spawnArea;
    }
    public Vector3 GenerateRandomWayPoint()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        int maxIndices = navMeshData.indices.Length - 3;

        // pick the first indice of a random triangle in the nav mesh
        int firstVertexSelected = UnityEngine.Random.Range(0, maxIndices);
        int secondVertexSelected = UnityEngine.Random.Range(0, maxIndices);

        // spawn on verticies
        Vector3 point = navMeshData.vertices[navMeshData.indices[firstVertexSelected]];

        Vector3 firstVertexPosition = navMeshData.vertices[navMeshData.indices[firstVertexSelected]];
        Vector3 secondVertexPosition = navMeshData.vertices[navMeshData.indices[secondVertexSelected]];

        // eliminate points that share a similar X or Z position to stop spawining in square grid line formations
        if ((int)firstVertexPosition.x == (int)secondVertexPosition.x || (int)firstVertexPosition.z == (int)secondVertexPosition.z)
        {
            point = GenerateRandomWayPoint(); // re-roll a position - I'm not happy with this recursion it could be better
        }
        else
        {
            // select a random point on it
            point = Vector3.Lerp(firstVertexPosition, secondVertexPosition, UnityEngine.Random.Range(0.05f, 0.95f));
        }

        point = SetDistanceFromGround();

        return point;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

    //public Vector3 GetRandomPointOnGraph()
    //{
    //    // Works for ANY graph type, however this is much slower
    //    var graph = AstarPath.active.data.graphs[0];
    //    // Add all nodes in the graph to a list
    //    List<GraphNode> nodes = new List<GraphNode>();
    //    graph.GetNodes((System.Action<GraphNode>)nodes.Add);
    //    GraphNode randomNode = nodes[Random.Range(0, nodes.Count)];

    //    // Use the center of the node as the destination for example
    //    var destination1 = (Vector3)randomNode.position;
    //    // Or use a random point on the surface of the node as the destination.
    //    // This is useful for navmesh-based graphs where the nodes are large.
    //    return randomNode.RandomPointOnSurface();
    //}
}
