using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantController : MonoBehaviour
{
    public float age = 0f;
    public float ageOfMaturity = 60f;

    [field: ReadOnlyField, SerializeField] private Vector2 lifeSizeRange;
    private Vector3 minSize, maxSize;

    public float growthSpeed = 0.5f;
    public Vector2 birthSizeRange = new Vector2(0.2f, 0.4f);
    public Vector2 maturitySizeRange = new Vector2(0.8f, 1.6f);

    public bool canProduce;
    public ProduceController defaultProduce;
    public List<Transform> producePositions;
    public List<ProduceController> produceList;

    [System.Serializable]
    public class PlantRequirements
    {
        public Vector2 tempRange;
        public Vector2 cloudCoverageRange;
        public Vector2 requiredWaterRange;
        public float weatherToleranceTime;
        public float tempToleranceTime;
        public float waterToleranceTime;
        public float deathTime;
        //amount of sunlight
    }

    [System.Serializable]
    public class PlantProduce
    {
        public List<GameObject> produceStages;

        public int currentProduceStage;

        public float produceTempQualityEffect;
        public float produceWeatherQualityEffect;
        public float produceWaterQualityEffect;

        public Vector2 produceQualityRange;
    }

    //[System.Serializable]
    //public class Produce
    //{
    //    [field: ReadOnlyField] public Vector2 lifeSizeRange;
    //    [field: ReadOnlyField] public Vector3 minSize, maxSize;

    //    [field: ReadOnlyField] public Transform producePosition;
    //    public GameObject produceObject;

    //    public Vector2 birthSizeRange = new Vector2(0.2f, 0.4f);
    //    public Vector2 maturitySizeRange = new Vector2(0.8f, 1.6f);

    //    public float growthSpeed = 0.5f;
    //    public float age;
    //    public float ageOfMaturity = 60f;

    //    public MeshRenderer produceRenderer;

    //    public Color unRipeColour = Color.green;
    //    public Color ripeColour;

    //}

    private void Awake()
    {
        age = 0f;

        lifeSizeRange.x = Random.Range(birthSizeRange.x, birthSizeRange.y);
        minSize = new Vector3(lifeSizeRange.x, lifeSizeRange.x, lifeSizeRange.x);
        lifeSizeRange.y = Random.Range(maturitySizeRange.x, maturitySizeRange.y);
        maxSize = new Vector3(lifeSizeRange.y, lifeSizeRange.y, lifeSizeRange.y);

        produceList = new List<ProduceController>();
        for (int i = 0; i < producePositions.Count; i++)
        {
            produceList.Add(null);
        }

    }

    private void Update()
    {
        age += Time.deltaTime * growthSpeed;
        if (age < ageOfMaturity) // grow
        {
            canProduce = false;

            transform.localScale = Vector3.Lerp(minSize, maxSize, age / ageOfMaturity);
        }
        else
        {
            SpawnProduce();
        }
    }

    public void InitializeProduce(ProduceController produce)
    {

        if (FindEmptyPosition())
        {
            produce.producePosition = FindEmptyPosition();

           

            //newProduce.growthSpeed = defaultProduce.growthSpeed;
            //newProduce.ageOfMaturity = defaultProduce.ageOfMaturity;
            //newProduce.ageOfDecline = defaultProduce.ageOfDecline;
            //newProduce.ageOfSpoilage = defaultProduce.ageOfSpoilage;


            produceList.Add(produce);
        
        }
    }

    public Transform FindEmptyPosition()
    {
        for (int p = 0; p < producePositions.Count; p++)
        {
            for (int i = 0; i < produceList.Count; i++)
            {

                if (produceList[i] != null && produceList[i].producePosition == producePositions[p])
                {
                    break;
                }
                else if (i + 1 >= produceList.Count)
                    return producePositions[p];
            }
        }

        return null;
    }

    public void ResetProduce(ProduceController produce)
    {
        produceList.Remove(produce);
        produceList.Add(null);
        //produce.age = 0f;

        //produce.lifeSizeRange.x = Random.Range(defaultProduce.birthSizeRange.x, defaultProduce.birthSizeRange.y);
        //produce.minSize = new Vector3(lifeSizeRange.x, lifeSizeRange.x, lifeSizeRange.x);
        //produce.lifeSizeRange.y = Random.Range(defaultProduce.maturitySizeRange.x, defaultProduce.maturitySizeRange.y);
        //produce.maxSize = new Vector3(lifeSizeRange.y, lifeSizeRange.y, lifeSizeRange.y);

        //produce.growthSpeed = defaultProduce.growthSpeed;
        //produce.ageOfMaturity = defaultProduce.ageOfMaturity;

        ////produce.gameObject = null;
        //produce.ripeColour = Color.white;

    }

    public void SpawnProduce()
    {

        for (int i = 0; i < produceList.Count; i++)
        {
            Transform emptyPos = null;


            if (FindEmptyPosition() != null)
            {
                emptyPos = FindEmptyPosition();
            }
            else Debug.Log("No Empty Positions Found");

            if ((produceList[i] == null || produceList.Count < producePositions.Count) && emptyPos != null)
            {
                ProduceController produce = Instantiate(defaultProduce, emptyPos.position, Quaternion.identity);
                produce.transform.localScale = produce.minSize;

                produce.producePosition = emptyPos;


                produce.stillPlanted = true;

                produce.parentPlant = this;

                produce.GetComponent<Rigidbody>().isKinematic = true;
                produce.GetComponent<Rigidbody>().useGravity = false;

                produceList[i] = produce;

            }
        }


    }

    
}
