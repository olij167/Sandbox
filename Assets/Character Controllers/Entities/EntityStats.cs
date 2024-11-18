using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats : CharacterStats
{

    //public float energy;
    //public Stat maxEnergy;
    //public Stat energyIncreaseRate;
    //public Stat energyDecreaseRate;

    public Vector2 awakeHours;
    //public bool isNocturnal;

    public float hunger;
    public Stat maxHunger;
    //public Stat hungerIncreaseRate;
    public Stat hungerDecreaseRate;

    public float thirst;
    public Stat maxThirst;
    //public Stat thirstIncreaseRate;
    public Stat thirstDecreaseRate;

    public float sexDrive;
    public Stat maxSexDrive;
    public Stat sexDriveIncreaseRate;
    //public Stat sexDriveDecreaseRate;

    Animator animator;

    public Vector2 lootRange = new Vector2(1, 3);
    public List<LootDrop> lootDrops;

    [System.Serializable]
    public class LootDrop
    {
        public GameObject loot;
        [Tooltip("0 = impossible, 100 = highly likely")]
        public float rarity;
    }

    public GameObject experienceItem;

    public bool isDead;

    private void Awake()
    {
        animator = GetComponent<EntityController>().animator;
    }
    public override void Die()
    {
        base.Die();

        isDead = true;

        ParkStats.instance.StopTrackingObject(gameObject);
        // death animation
        if (FindObjectOfType<PlayerAttack>())
            FindObjectOfType<PlayerAttack>().lookTargets.RemoveMember(transform);
        animator.SetBool("isDead", true);
        GetComponent<Collider>().enabled = false;
        StartCoroutine(DeathAnimationDelay());

        //spawn loot
        if (lootDrops != null && lootDrops.Count > 0)
            SpawnLoot();
    }

    public void SpawnLoot()
    {
        int r = (int)Random.Range(lootRange.x, lootRange.y);

        List<GameObject> lootSpawns = new List<GameObject>();

        for (int i = 0; i < r; i++)
        {

            float luckPool = Random.Range(0f, 100f);

            List<GameObject> possibleLoot = new List<GameObject>();

            for (int j = 0; j < lootDrops.Count; j++)
            {
                if (lootDrops[j].rarity >= luckPool)
                    possibleLoot.Add(lootDrops[j].loot);
            }
            if (possibleLoot.Count > 0)
                lootSpawns.Add(possibleLoot[Random.Range(0, possibleLoot.Count)]);
            else
            {
                Debug.Log("No loot to spawn for " + gameObject.name);
                return;
            }
        }

        for (int i = 0;i < lootSpawns.Count;i++)
        {
            Instantiate(lootSpawns[i], transform.position, Quaternion.identity);
        }

        Debug.Log("Loot has been spawned for " + gameObject.name);
    }

    IEnumerator DeathAnimationDelay()
    {
        if (animator.GetBool("isDead"))
        {
            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);


            if (clipInfo.Length > 0)
            {
                //Debug.Log(clipInfo[0].clip.name + " Length: " + clipInfo[0].clip.length);

                yield return new WaitForSeconds(clipInfo[0].clip.length);

                Instantiate(experienceItem, transform.position, Quaternion.identity);

                Debug.Log(gameObject.name + " has died");
                Destroy(gameObject);


            }
        }

    }

}
