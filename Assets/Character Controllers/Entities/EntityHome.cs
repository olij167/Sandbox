public class EntityHome : SpawnObjects
{
    public float homePriority;

    public override void SpawnPrefabs(bool randomPrefabs, bool inRadius)
    {
        base.SpawnPrefabs(randomPrefabs, inRadius);

        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            if (spawnedObjects[i].GetComponent<EntityController>())
            {
                spawnedObjects[i].GetComponent<EntityController>().SetHome(this.transform, homePriority); 
            }
        }
    }
}
