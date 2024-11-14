public class EntityHome : SpawnObjects
{
    public float homePriority;
    public HomeType homeType;
    public bool isVacant = true;

    private void Start()
    {
        ParkStats.instance.TrackObject(gameObject);

    }
    public override void SpawnPrefabs(bool randomPrefabs, bool inRadius)
    {
        base.SpawnPrefabs(randomPrefabs, inRadius);

        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            if (spawnedObjects[i].GetComponent<EntityController>())
            {
                if (spawnedObjects[i].GetComponent<EntityController>().homeType == homeType)
                {
                    spawnedObjects[i].GetComponent<EntityController>().spawnedByHome = true;
                    spawnedObjects[i].GetComponent<EntityController>().SetHome(this.transform, homePriority);
                }
                else
                {
                    spawnedObjects.RemoveAt(i);
                    i -= 1;
                }
            }
        }
    }
}

[System.Serializable]
public enum HomeType
{
    TallGrass, Rocks, Cave, Nest, Burrow, Den, Hive, Thicket
}