//using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHome : SpawnObjects
{
    public float homePriority;
    public HomeType homeType;
    public bool isVacant = true;
    public EntityController resident;

    [SerializeField] private bool setColour;
    [SerializeField] private Color debugColour;

    private void Start()
    {
        ParkStats.instance.TrackObject(gameObject);

        if (setColour) GetComponent<MeshRenderer>().material.color = debugColour;

    }

    private void LateUpdate()
    {
        if (!isVacant && resident == false)
            isVacant = true;
    }

    public override void SpawnPrefabs(bool randomPrefabs, bool inRadius)
    {
        base.SpawnPrefabs(randomPrefabs, inRadius);

        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            if (spawnedObjects[i] != null)
            {
                if (spawnedObjects[i].GetComponent<EntityController>())
                {
                    spawnedObjects[i].GetComponent<EntityController>().spawnedByHome = true;

                    if (spawnedObjects[i].GetComponent<EntityController>().homeType == homeType)
                    {
                        spawnedObjects[i].GetComponent<EntityController>().SetHome(this.transform, homePriority);
                    }
                    else
                    {
                        spawnedObjects.RemoveAt(i);
                        i -= 1;
                    }
                }
            }
            else
            {
                spawnedObjects.RemoveAt(i);
                i -= 1;
            }
        }
    }
}

[System.Serializable]
public enum HomeType
{
    TallGrass, Rocks, Cave, Nest, Burrow, Den, Hive, Thicket
}