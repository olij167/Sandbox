using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeWeather;

public class PlantController : MonoBehaviour
{
    public string plantName;
    [TextArea(2,6)]public string plantDesc;
    public Sprite icon;
    public PlantType plantType;
    public Vector2 seedsToSpawn = new Vector2(0,3);
    public int seedsToSpawnNum;
    public float age = 0f;
    public float ageOfMaturity = 60f;

    public float health = 100f; // health stat for growing plants
    public float maxHealth = 100f;
     
    public float damage = 0f; //health stat for destroying plants
    public float maxDamage = 100f;
    public AttackType[] attackTypeToDamage;

    public bool planted = true;
    public bool isDead;
    private bool destroyStarted;

    public bool destroyWhenNotPlanted;
    public float destroyDelay = 10f;

    public PlantRequirements requirements;

    public bool requirementsMet;

    public PlantDiagnostic plantDiagnostic;

    public float water;

    [field: ReadOnlyField] public Vector3 originalSize;
    [field: ReadOnlyField, SerializeField] private Vector2 lifeSizeRange;
    private Vector3 minSize, maxSize;

    public float growthSpeed = 0.5f;
    public Vector2 birthSizeRange = new Vector2(0.2f, 0.4f);
    public Vector2 maturitySizeRange = new Vector2(0.8f, 1.6f);

    public bool canProduce;
    [field: ReadOnlyField, SerializeField] private float produceSpawnCountdown;
    public ProduceController defaultProduce;
    public float produceFrequency;
    public List<Transform> producePositions;
    public List<ProduceController> produceList;
    public List<int> takenProducePositionList;

    public MeshRenderer[] foliageMeshes;
    public int currentHealthPhase;
    public List<Color> healthColours;
    public List<Color> additionalHealthColours;

    private TimeController timeController;
    private WeatherController weatherController;
    //[field: ReadOnlyField] GridBuildingSystem gridSystem;

    [System.Serializable]
    public class PlantRequirements
    {
        public Vector2 tempRange;
        public Vector2 cloudCoverageRange;
        public Vector2 requiredWaterRange;
        public List<string> season;
        public List<string> weather;
        //amount of sunlight
    }

    [System.Serializable]
    public class PlantDiagnostic
    {
        public float totalDisparity;
        [Header("Temperature")]
        public bool goodTemp;
        public bool tooHot;
        public bool tooCold;
        public float tempDisparity;
        [Header("Moisture")]
        public bool goodMoisture;
        public bool tooWet;
        public bool tooDry;
        public float moistureDisparity;
        [Header("Sunlight")]
        public bool goodSunlight;
        public bool tooCloudy;
        public bool tooSunny;
        public float sunDisparity;
        [Header("Season")]
        public bool wrongSeason;
        [Header("Weather")]
        public bool badWeather;
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
        timeController = TimeController.instance;
        weatherController = FindObjectOfType<WeatherController>();

        switch (plantType)
        {
            case PlantType.Tree:
                healthColours[0] = foliageMeshes[0].materials[1].GetColor("_Top_Color");
                additionalHealthColours[0] = foliageMeshes[0].materials[1].GetColor("_Bottom_Color");
                break;
            case PlantType.Bush:
                healthColours[0] = foliageMeshes[0].material.GetColor("_Top_Color");
                additionalHealthColours[0] = foliageMeshes[0].material.GetColor("_Bottom_Color");
                break;

            case PlantType.Root:
                healthColours[0] = foliageMeshes[0].material.color;

                break;

            case PlantType.Flower:
                healthColours[0] = foliageMeshes[0].material.GetColor("_Top_Color");
                additionalHealthColours[0] = foliageMeshes[0].material.GetColor("_Bottom_Color");
                break;

            case PlantType.Weed:

                break;

            case PlantType.Mushroom:

                break;
            case PlantType.Grass:

                break;
            case PlantType.Grain: 

                break;
        }
       

        age = 0f;

        water = (requirements.requiredWaterRange.y - requirements.requiredWaterRange.x) / 2 + requirements.requiredWaterRange.x;

        originalSize = transform.localScale;

        lifeSizeRange.x = Random.Range(birthSizeRange.x, birthSizeRange.y);
        minSize = new Vector3(lifeSizeRange.x * originalSize.x, lifeSizeRange.x * originalSize.y, lifeSizeRange.x * originalSize.z) ;
        lifeSizeRange.y = Random.Range(maturitySizeRange.x, maturitySizeRange.y);
        maxSize = new Vector3(lifeSizeRange.y * originalSize.x, lifeSizeRange.y * originalSize.y, lifeSizeRange.y * originalSize.z);

        transform.localScale = Vector3.Lerp(minSize, maxSize, age / ageOfMaturity);

        if (Chance.CoinFlip())
        {
            seedsToSpawnNum = (int)Random.Range(seedsToSpawn.x, seedsToSpawn.y);
        }
        else seedsToSpawnNum = 0;
        
        //Debug.Log("Seeds to spawn = " +  seedsToSpawnNum);

        if (defaultProduce != null && producePositions.Count > 0)
        {
            produceFrequency = defaultProduce.ageOfMaturity;

            produceList = new List<ProduceController>();
            for (int i = 0; i < producePositions.Count; i++)
            {
                produceList.Add(null);
            }

            takenProducePositionList = new List<int>();
        }
    }

    private void Update()
    {
        if (!isDead && planted)
        {
            CheckDiagnostics();

            requirementsMet = CheckRequirements();


            if (requirementsMet)
            {
                age += Time.deltaTime * growthSpeed;
                if (age < ageOfMaturity) // grow
                {
                    canProduce = false;

                    transform.localScale = Vector3.Lerp(minSize, maxSize, age / ageOfMaturity);

                    if (health < maxHealth)
                    {
                        health += Time.deltaTime * growthSpeed;
                    }
                }
                else if (health < maxHealth)
                {
                    health += Time.deltaTime * growthSpeed;
                }
                else if (defaultProduce != null && producePositions.Count > 0)
                {
                    GenerateNewProduce();
                }
            }
            else if (health > 0)
            {
                health -= Time.deltaTime * plantDiagnostic.totalDisparity;
            }
            else
            {
                isDead = true;
            }


            currentHealthPhase = (int)Mathf.Clamp((maxHealth - health) / (maxHealth / healthColours.Count), 0f, healthColours.Count - 1f);
            //Debug.Log((maxHealth - health) / (maxHealth / healthColours.Count));
            foreach (MeshRenderer mesh in foliageMeshes)
            {
                

                switch (plantType)
                {
                    case PlantType.Tree:
                        if (currentHealthPhase >= 0)
                        {
                            mesh.materials[1].SetColor("_Top_Color", healthColours[currentHealthPhase]);
                            mesh.materials[1].SetColor("_Bottom_Color", additionalHealthColours[currentHealthPhase]);
                        }
                        else
                        {
                            mesh.materials[1].SetColor("_Top_Color", healthColours[healthColours.Count - 1]);
                            mesh.materials[1].SetColor("_Bottom_Color", additionalHealthColours[additionalHealthColours.Count - 1]);
                        }
                        break;
                    case PlantType.Bush:
                        if (currentHealthPhase >= 0)
                        {
                            mesh.material.SetColor("_Top_Color", healthColours[currentHealthPhase]);
                            mesh.material.SetColor("_Bottom_Color", additionalHealthColours[currentHealthPhase]);
                        }
                        else
                        {
                            mesh.material.SetColor("_Top_Color", healthColours[healthColours.Count - 1]);
                            mesh.material.SetColor("_Bottom_Color", additionalHealthColours[additionalHealthColours.Count - 1]);
                        }
                            break;

                    case PlantType.Root:

                        break;

                    case PlantType.Flower:
                        if (currentHealthPhase >= 0 && currentHealthPhase < additionalHealthColours.Count)
                        {
                            mesh.material.SetColor("_Top_Color", healthColours[currentHealthPhase]);
                            mesh.material.SetColor("_Bottom_Color", additionalHealthColours[currentHealthPhase]);
                        }
                        else
                        {
                            mesh.material.SetColor("_Top_Color", healthColours[healthColours.Count - 1]);
                            mesh.material.SetColor("_Bottom_Color", additionalHealthColours[additionalHealthColours.Count - 1]);
                        }
                        break;

                    case PlantType.Weed:

                        break;

                    case PlantType.Mushroom:

                        break;
                    case PlantType.Grass:

                        break;
                    case PlantType.Grain:

                        break;
                }
            }

            if (weatherController.currentWeatherPreset.isRaining) water += (weatherController.rainChance / 100) * Time.deltaTime;
            else if (water > 0) water -= (weatherController.temperature / 100) * Time.deltaTime;
        }

        if (damage >= maxDamage && planted)
        {
            // destroy & spawn resources
            if (transform.parent != null && transform.parent.GetComponent<PlacedObject>())
            {
                PlacedObject placedObject = transform.parent.GetComponent<PlacedObject>();
                //PlacedObjectTypeSO item = placedObject.refItem;
                placedObject.gridSystem.grid.GetGridObject(placedObject.origin.x, placedObject.origin.y).ClearPlacedObject();

                transform.parent = null;

                if (produceList != null && produceList.Count > 0)
                {
                    for (int i = 0; i < produceList.Count; i++)
                    {
                        if (produceList[i] != null)
                        {
                            produceList[i].FallFromPlant();
                        }
                    }
                }

                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<Rigidbody>().useGravity = true;

                ParkStats.instance.StopTrackingObject(placedObject.gameObject);

                Destroy(placedObject.gameObject);

                planted = false;
            }

            //if (placedObject != null && inventory.inventorySlotNum > inventory.inventory.Count)
            //{
            //    PlacedObjectTypeSO item = placedObject.refItem;
            //    gridSystem.GetGridObject(x, z).ClearPlacedObject();
            //    inventory.AddItemToInventory(item.itemData, placedObject.gameObject);
            //    Destroy(placedObject.gameObject);
            //    //item.refItem.PickUpItem();

            //}
        }

        if (destroyWhenNotPlanted && !planted && !destroyStarted)
            StartCoroutine(DestroyAfterDelay(destroyDelay));

       
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        destroyStarted = true;
        int numSpawned = 0;

        if (numSpawned < seedsToSpawnNum && !isDead) // dont spawn seeds if it's dead
        {
            for (int i = 0; i < seedsToSpawnNum; i++)
            {
                Instantiate(defaultProduce.seedItem.prefab, new Vector3(transform.position.x + Random.Range(-0.5f, 0.5f), transform.position.y, transform.position.z + Random.Range(-0.5f, 0.5f)), Quaternion.identity);
                numSpawned++;
            }
        }

        yield return new WaitForSeconds(delay);

        Destroy(gameObject);

    }

    public bool CheckRequirements()
    {
        if (plantDiagnostic.goodMoisture && plantDiagnostic.goodSunlight && plantDiagnostic.goodTemp && !plantDiagnostic.wrongSeason && !plantDiagnostic.badWeather)
            return true;
        else return false;
    }

    public void CheckDiagnostics()
    {
        //check temperature
        plantDiagnostic.tooCold = weatherController.temperature < requirements.tempRange.x ? true : false;
        plantDiagnostic.tooHot = weatherController.temperature > requirements.tempRange.y ? true : false;

        plantDiagnostic.goodTemp = (!plantDiagnostic.tooCold && !plantDiagnostic.tooHot) ? true : false;

        if (!plantDiagnostic.goodTemp)
        {
            if (plantDiagnostic.tooCold)
            {
                plantDiagnostic.tempDisparity = requirements.tempRange.x - weatherController.temperature;
            }

            if (plantDiagnostic.tooHot)
            {
                plantDiagnostic.tempDisparity = weatherController.temperature - requirements.tempRange.y;
            }
        }
        else plantDiagnostic.tempDisparity = 0;

        //check cloud cover
        plantDiagnostic.tooCloudy = weatherController.hourlyWeather[timeController.timeHours].cloudPower < requirements.cloudCoverageRange.x ? true : false;
        plantDiagnostic.tooSunny = weatherController.hourlyWeather[timeController.timeHours].cloudPower > requirements.cloudCoverageRange.y ? true : false;

        plantDiagnostic.goodSunlight = (!plantDiagnostic.tooCloudy && !plantDiagnostic.tooSunny) ? true : false;

        if (!plantDiagnostic.goodSunlight)
        {
            if (plantDiagnostic.tooCloudy)
            {
                plantDiagnostic.sunDisparity = requirements.cloudCoverageRange.x - weatherController.hourlyWeather[timeController.timeHours].cloudPower;
            }
            if (plantDiagnostic.tooSunny)
            {
                plantDiagnostic.sunDisparity = weatherController.hourlyWeather[timeController.timeHours].cloudPower - requirements.cloudCoverageRange.y;
            }
        }
        else plantDiagnostic.sunDisparity = 0;

        //check water levels
        plantDiagnostic.tooDry = water < requirements.requiredWaterRange.x ? true : false;
        plantDiagnostic.tooWet = water > requirements.requiredWaterRange.y ? true : false;

        plantDiagnostic.goodMoisture = (!plantDiagnostic.tooWet && !plantDiagnostic.tooDry) ? true : false;
        if (!plantDiagnostic.goodMoisture)
        {
            if (plantDiagnostic.tooDry)
            {
                plantDiagnostic.moistureDisparity = requirements.requiredWaterRange.y - water;
            }

            if (plantDiagnostic.tooWet)
            {
                plantDiagnostic.moistureDisparity = water - requirements.requiredWaterRange.x;
            }
        }
        else plantDiagnostic.moistureDisparity = 0;

        //check season
        plantDiagnostic.wrongSeason = requirements.season.Contains(timeController.currentMonthData.season) ? false : true;

        //check weather
        if (requirements.weather != null && requirements.weather.Count >= 0)
        {
            for (int i = 0; i < requirements.weather.Count; i++)
            {
                for (int w = 0; w < weatherController.weatherDataPresets.Length; w++)
                {
                    if (requirements.weather[i] == weatherController.weatherDataPresets[w].weatherCondition)
                        plantDiagnostic.badWeather = false;
                    break;
                }

                if (i + 1 >= requirements.weather.Count)
                    plantDiagnostic.badWeather = true;
            }
        }
        else plantDiagnostic.badWeather = false;

        plantDiagnostic.totalDisparity = plantDiagnostic.tempDisparity + plantDiagnostic.moistureDisparity + plantDiagnostic.sunDisparity;

    }


    public Transform FindEmptyPosition()
    {
        for (int p = 0; p < producePositions.Count; p++)
        {
            for (int i = 0; i < produceList.Count; i++)
            {
                if (takenProducePositionList.Count > 0)
                {
                    for (int tP = 0; tP < takenProducePositionList.Count; tP++)
                    {
                        if (produceList[i] != null && produceList[i].producePosition == producePositions[p] || takenProducePositionList[tP] == p)
                        {
                            break;
                        }
                        else if (tP + 1 >= takenProducePositionList.Count)
                        {
                            return producePositions[p];
                        }
                    }
                }
                else
                {
                    return producePositions[p];
                }
            }
        }

        return null;
    }

    public void ResetProduce(ProduceController produce)
    {
        for (int i = 0; i < produceList.Count; i++)
        {
            if (produceList[i] == produce)
            {
                takenProducePositionList.Remove(i);
            }
        }

        produceList.Remove(produce);
        produceList.Add(null);
    }

    public void GenerateNewProduce()
    {

        for (int i = 0; i < produceList.Count; i++)
        {
            Transform emptyPos = null;


            if (FindEmptyPosition() != null)
            {
                emptyPos = FindEmptyPosition();
            }
            //else Debug.Log("No Empty Positions Found");

            produceSpawnCountdown = Random.Range(1f, produceFrequency);

            if (takenProducePositionList.Count > 0)
            {
                for (int tP = 0; tP < takenProducePositionList.Count; tP++)
                {
                    if (!takenProducePositionList.Contains(i))
                    {
                        StartCoroutine(SpawnProduceDelayed(produceSpawnCountdown, emptyPos, i));

                    }
                }
            }
            else StartCoroutine(SpawnProduceDelayed(produceSpawnCountdown, emptyPos, i));
        }
    }

    public void SpawnNewProduce(Transform emptyPos, int produceIndex)
    {
        if ((produceList[produceIndex] == null || produceList.Count < producePositions.Count) && emptyPos != null)
        {
            ProduceController produce = Instantiate(defaultProduce, emptyPos.position, Quaternion.identity, emptyPos);
            produce.parentPlant = this;
            produce.gameObject.name = defaultProduce.name;
            produce.transform.localScale = produce.minSize;


            produce.producePosition = emptyPos;


            produce.stillPlanted = true;


            produce.GetComponent<Rigidbody>().isKinematic = true;
            produce.GetComponent<Rigidbody>().useGravity = false;

            produceList[produceIndex] = produce;

            ParkStats.instance.TrackObject(produce.gameObject);
        }
    }

    public IEnumerator SpawnProduceDelayed(float delay, Transform emptyPos, int produceIndex)
    {
        takenProducePositionList.Add(produceIndex);


        //Debug.Log("Produce will spawn in " + delay + " second");
        yield return new WaitForSeconds(delay);

        SpawnNewProduce(emptyPos, produceIndex);

        yield return null;
    }
    
}

[System.Serializable]
public enum PlantType
{
    Tree, Bush, Root, Flower, Weed, Mushroom, Grass, Grain
}
