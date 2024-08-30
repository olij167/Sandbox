using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//using Pathfinding;

// requires a nav mesh in the scene
public class SpawnObjects : MonoBehaviour
{
    public Transform parent;
    public List<GameObject> prefabs;

    public bool randomSpawnPoint;

    Vector3 spawnPos;
    public int spawnNumPerPrefab, totalSpawnNum;

    public bool randomPrefabs;

    public Transform spawnPoint;
    private Vector3 spawnArea;
    public float distanceFromGround;


    public bool spawnInRadius;
    public float spawnRadius = 10;

    public bool spawnConstant;

    public int maxSpawnAmount = 10;
    public List<GameObject> spawnedObjects;

    public float spawnRate = 30f;
    public bool fluctuateSpawnRate;
    public float fluctuationRange = 30f;

    private float timer;

    public void Update()
    {
        SpawnPrefabs(randomPrefabs, randomSpawnPoint);

        CheckIfDestroyed();
    }

    public void SpawnPrefabs(bool randomPrefabs, bool randomPos)
    {
        if (!randomPrefabs)
        {
            for (int i = 0; i < prefabs.Count; i++)
            {
                if (spawnConstant)
                {
                    if (fluctuateSpawnRate)
                    {
                        SpawnSporatic(prefabs[i], parent, spawnRate, randomPos);
                    }
                    else
                    {
                        SpawnConsistent(prefabs[i], parent, spawnRate, randomPos);
                    }
                }
                else
                {
                    SpawnGameObjectsAtRandomPos(prefabs[i], spawnNumPerPrefab);
                }

            }

        }
        else
        {
            for (int i = 0; i < totalSpawnNum; i++)
            {

                if (spawnConstant)
                {
                    if (fluctuateSpawnRate)
                    {
                        SpawnSporatic(prefabs[Random.Range(0, prefabs.Count)], parent, spawnRate, randomPos);
                    }
                    else
                    {
                        SpawnConsistent(prefabs[Random.Range(0, prefabs.Count)], parent, spawnRate, randomPos);
                    }
                }
                else
                {
                    SpawnGameObjectsAtRandomPos(prefabs[i], spawnNumPerPrefab);
                }

            }
        }
    }

    public void CheckIfDestroyed()
    {
        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            if (spawnedObjects[i] == null)
            {
                spawnedObjects.RemoveAt(i);

                Debug.Log("Spawned Item " + i + " has been destroyed");
            }
        }
    }

    public void SpawnConsistent(GameObject prefab, Transform parent, float spawnRate, bool randomSpawnPos)
    {
        timer -= Time.deltaTime;
        if (spawnedObjects.Count < maxSpawnAmount)
        {
            if (timer <= 0f)
            {
                if (spawnInRadius)
                    GenerateRandomPointWithinRadius();
                else spawnArea = spawnPoint.position;

                if (!randomSpawnPos)
                {
                    GameObject newObject = Instantiate(prefab, spawnArea, Quaternion.identity);
                    newObject.transform.parent = parent;
                    spawnedObjects.Add(newObject);
                }
                else
                {
                    SpawnSpecificObjectAtRandomPos(prefab, parent);
                }
                timer = spawnRate;
            }
        }
        //else
        //{
        //    //Debug.Log("Max Objects Reached");
        //    //return null;
        //}

    }

    public void SpawnSporatic(GameObject prefab, Transform parent, float spawnRate, bool randomSpawnPos)
    {
        timer -= Time.deltaTime;
        if (spawnedObjects.Count < maxSpawnAmount)
        {
            if (timer <= 0f)
            {
                if (spawnInRadius)
                    GenerateRandomPointWithinRadius();
                else spawnArea = spawnPoint.position;

                if (!randomSpawnPos)
                {
                    GameObject newObject = Instantiate(prefab, spawnArea, Quaternion.identity);
                    newObject.transform.parent = parent;
                    spawnedObjects.Add(newObject);

                }
                else
                {
                    SpawnSpecificObjectAtRandomPos(prefab, parent);
                }
                timer = Random.Range(spawnRate - fluctuationRange, spawnRate + fluctuationRange);
            }
        }
        else
        {
            //Debug.Log("Max Objects Reached");
            //return null;
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
        if (spawnedObjects.Count < maxSpawnAmount)
        {
            spawnPos = GenerateRandomWayPoint();
            //spawnPos = GetRandomPointOnGraph();
            GameObject newObject = Instantiate(prefab, spawnPos, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));
            newObject.transform.parent = parent;
            spawnedObjects.Add(newObject);


            return newObject;
        }
        else
        {
            Debug.Log("Max Objects Reached");
            return null;
        }
    }

    public void SpawnGameObjectsAtRandomPos(GameObject prefab, int numToSpawn)
    {
        if (spawnedObjects.Count < maxSpawnAmount)
        {
            for (int i = 0; i < numToSpawn; i++)
        {
            spawnPos = GenerateRandomWayPoint();
            //spawnPos = GetRandomPointOnGraph();
            GameObject newObject = Instantiate(prefab, spawnPos, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));
            newObject.transform.parent = parent;
            spawnedObjects.Add(newObject);

        }
        }
        else
        {
            Debug.Log("Max Objects Reached");
            //return null;
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
