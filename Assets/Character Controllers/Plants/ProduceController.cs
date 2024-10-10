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

    public Vector2 birthSizeRange = new Vector2(0.2f, 0.4f);
    public Vector2 maturitySizeRange = new Vector2(0.8f, 1.6f);

    public float growthSpeed = 0.5f;
    public float age;
    public float ageOfMaturity = 60f;
    public float ageOfDecline = 120f;
    public float ageOfSpoilage = 180f;
    public bool stillPlanted;

    public MeshRenderer produceRenderer;

    public Color unRipeColour = Color.green;
    [field: ReadOnlyField] public Color ripeColour;
    public Color spoiledColour;

    private void Awake()
    {
        ripeColour = produceRenderer.material.color;

        age = 0f;

        lifeSizeRange.x = Random.Range(birthSizeRange.x, birthSizeRange.y);
        minSize = new Vector3(lifeSizeRange.x, lifeSizeRange.x, lifeSizeRange.x);
        lifeSizeRange.y = Random.Range(maturitySizeRange.x, maturitySizeRange.y);
        maxSize = new Vector3(lifeSizeRange.y, lifeSizeRange.y, lifeSizeRange.y);

        spoiledSize = minSize + (maxSize / 2);
    }

    private void Update()
    {
        age += Time.deltaTime * growthSpeed;

        if (age < ageOfMaturity)
        {
            Grow();
        }
        else if (stillPlanted)
        {
            FallFromPlant();
        }
        else
        {
            Spoil();
        }
    }

    public void Grow()
    {
        transform.localScale = Vector3.Lerp(minSize, maxSize, age / ageOfMaturity);
        produceRenderer.material.color = Color.Lerp(unRipeColour, ripeColour, age / ageOfMaturity);

    }

    public void FallFromPlant()
    {
        transform.parent = null;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;

        parentPlant.ResetProduce(this);
        stillPlanted = false;
    }

    public void Spoil()
    {
        if (age > ageOfDecline)
        {
            float declineAge = age - ageOfDecline;
            transform.localScale = Vector3.Lerp(maxSize, spoiledSize, (age - ageOfDecline) / (ageOfSpoilage - ageOfDecline));
            produceRenderer.material.color = Color.Lerp(ripeColour, spoiledColour, (age - ageOfDecline) / (ageOfSpoilage - ageOfDecline));
        }
    }
}
