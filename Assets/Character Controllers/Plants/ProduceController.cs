using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProduceController : MonoBehaviour
{
    [field: ReadOnlyField] public Vector2 lifeSizeRange;
    [field: ReadOnlyField] public Vector3 minSize, maxSize, spoiledSize;

    [field: ReadOnlyField] public Transform producePosition;

    [field: ReadOnlyField] public PlantController parentPlant;
    //public GameObject produceObject;
    public Sprite icon;

    public InventoryItem seedItem;


    public ProduceQuality produceQuality;

    public Vector2 birthSizeRange = new Vector2(0.2f, 0.4f);
    public Vector2 maturitySizeRange = new Vector2(0.8f, 1.6f);

    public float growthSpeed = 0.5f;
    public float ripeningSpeed = 0.5f;
    public float age;
    public float ageOfMaturity = 60f;

    public float ageOfDecline = 120f;
    public float ageOfSpoilage = 180f;

    public bool continueToRipenWhenFallen;

    public bool stillPlanted;
    public bool fallWhenMature = true;

    public MeshRenderer produceRenderer;

    [field: ReadOnlyField] public Color currentColour;


    public Color unRipeColour = Color.green;
    [field: ReadOnlyField] public Color ripeColour;
    public Color spoiledColour;

    private void Awake()
    {
        if (produceRenderer == null) produceRenderer = GetComponent<MeshRenderer>();

        ripeColour = produceRenderer.material.color;

        age = 0f;

        lifeSizeRange.x = Random.Range(birthSizeRange.x, birthSizeRange.y);
        minSize = new Vector3(lifeSizeRange.x, lifeSizeRange.x, lifeSizeRange.x);
        lifeSizeRange.y = Random.Range(maturitySizeRange.x, maturitySizeRange.y);
        maxSize = new Vector3(lifeSizeRange.y, lifeSizeRange.y, lifeSizeRange.y);

        spoiledSize = minSize + (maxSize / 2);

        if (parentPlant == null)
        {
            if (transform.parent != null && transform.parent.GetComponentInParent<PlantController>())
            {
                parentPlant = transform.parent.GetComponentInParent<PlantController>();
                stillPlanted = true;
            }
            else
                PreGrown();
        }
        else stillPlanted = true;


    }

    private void Start()
    {
        //ParkStats.instance.TrackObject(gameObject);

    }

    private void Update()
    {

        AssessRipeness();

    }

    public void Grow()
    {
        transform.localScale = Vector3.Lerp(minSize, maxSize, age / ageOfMaturity);
        produceRenderer.material.color = Color.Lerp(unRipeColour, ripeColour, age / ageOfMaturity);
        produceQuality = ProduceQuality.Growing;
    }

    public void PreGrown()
    {
        age = ageOfMaturity;

        transform.localScale = Vector3.Lerp(minSize, maxSize, age / ageOfMaturity);
        produceRenderer.material.color = Color.Lerp(unRipeColour, ripeColour, age / ageOfMaturity);
        AssessRipeness();
    }

    public void AssessRipeness()
    {

        if (age < ageOfMaturity)
        {
            if (stillPlanted)
            {
                produceQuality = ProduceQuality.Growing;

                if (parentPlant.requirementsMet) // only grow if the parent plant is healthy
                {
                    age += Time.deltaTime * growthSpeed;
                    transform.localScale = Vector3.Lerp(minSize, maxSize, age / ageOfMaturity);
                    produceRenderer.material.color = Color.Lerp(unRipeColour, ripeColour, age / ageOfMaturity);
                }

            }
            else
            {
                produceQuality = ProduceQuality.Unripe;

                if (continueToRipenWhenFallen)
                    produceRenderer.material.color = Color.Lerp(unRipeColour, ripeColour, age / ageOfMaturity);
            }

        }
        else if (age > ageOfSpoilage)
        {
            produceQuality = ProduceQuality.Spoiled;
        }
        else if (age > ageOfDecline)
        {
            produceQuality = ProduceQuality.Overripe;

            transform.localScale = Vector3.Lerp(maxSize, spoiledSize, (age - ageOfDecline) / (ageOfSpoilage - ageOfDecline));
            produceRenderer.material.color = Color.Lerp(ripeColour, spoiledColour, (age - ageOfDecline) / (ageOfSpoilage - ageOfDecline));
        }
        else if (age > ageOfMaturity && (produceQuality == ProduceQuality.Growing || produceQuality == ProduceQuality.Unripe))
        {

            if (fallWhenMature)
                FallFromPlant();

            maxSize = transform.localScale;
            spoiledSize = minSize + (maxSize / 2);

            produceQuality = ProduceQuality.Ripe;
        }
        else produceQuality = ProduceQuality.Ripe;

        if (!stillPlanted) // age continuosly when not planted
            age += Time.deltaTime * growthSpeed;

        if (currentColour != produceRenderer.material.color)
            currentColour = produceRenderer.material.color;

    }

    public void FallFromPlant()
    {

        transform.parent = null;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;

        parentPlant.ResetProduce(this);
        stillPlanted = false;
    }

    public void Harvest()
    {
        PlayerInventory inventory = GetComponent<PlayerInventory>();
        inventory.AddItemToInventory(GetComponent<ItemInWorld>().item, inventory.inventory, inventory.inventorySlots, gameObject, this);
    }

}

[System.Serializable]
public enum ProduceQuality
{
   Growing, Unripe, Ripe, Overripe, Spoiled
}
