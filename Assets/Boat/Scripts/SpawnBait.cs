using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBait : MonoBehaviour
{
    public delegate void SpawnBaitAction();

    public SpawnBaitAction onSpawnBait;

    public Transform sharkBait;

    public Vector3 spawnOffset;

    private Vector3 spawnPosition;

    public int baitLimit = 2;


    /// <summary>
    /// For testing without a VR headset.
    /// Set to true to spawn bait based on the Spawn Timer.
    /// </summary>
    [SerializeField] private bool debugMode;

    public float spawnTimer = 3f;
    private float timerReset;

    [SerializeField] private List<GameObject> baitObjects;

    private void Awake()
    {

        if (debugMode)
        {
            timerReset = spawnTimer;
        }

        CheckBaitNumber();
    }

    private void Update()
    {
        spawnPosition = new Vector3(transform.position.x + spawnOffset.x, transform.position.y + spawnOffset.y, transform.position.z + spawnOffset.z);


        if (debugMode)
        {
            spawnTimer -= Time.deltaTime;

            if (spawnTimer <= 0f)
            {
                SpawnBaitObject();
                spawnTimer = timerReset;
            }
        }
    }

    private void OnEnable()
    {
        onSpawnBait += SpawnBaitObject;
    }

    private void OnDisable()
    {
        onSpawnBait -= SpawnBaitObject;
    }

    public void SpawnBaitObject()
    {
        CheckBaitNumber();
        if (debugMode)
            Debug.Log("Checking bait num...");

        if (baitObjects.Count < baitLimit)
        {
            Transform newBait = Instantiate(sharkBait, spawnPosition, Quaternion.identity);
            newBait.parent = transform;
            if (debugMode)
                Debug.Log("New Bait Spawned");
        }
        else if(debugMode)
        {
            Debug.Log("Couldnt Spawn New Bait, the limit has been reached");
        }
    }

    public void CheckBaitNumber()
    {
        baitObjects = new List<GameObject>();

        foreach(GameObject bait in GameObject.FindGameObjectsWithTag("SharkBait"))
        {
            baitObjects.Add(bait);
        }
    }

}
