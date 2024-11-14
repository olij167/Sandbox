using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WildernessGuide : MonoBehaviour
{
    [System.Serializable]
    public class Page
    {
        public string subjectName;
        public PageType pageType;
        public GameObject pageSubject;
        public string subType;

    }

    private QuestManager questManager;

    public GameObject journalPanel;
    public GameObject leftPage;
    public GameObject rightPage;

    public GameObject contentsPage;
    public AnimalPageUI animalPage;
    public GameObject animalIndexPage;
    public GameObject animalSubIndexPage;
    private GameObject animalIndexPageLayoutGroup;
    public PlantPageUI plantPage;
    public GameObject plantIndexPage;
    public GameObject plantSubIndexPage;
    private GameObject plantIndexPageLayoutGroup;
    public GameObject questPage;

    //public PlantPageUI plantPage;
    //public AnimalPageUI animalPage;


    public GameObject indexListButton;

    public int leftPageNum;
    public int rightPageNum;

    [field: ReadOnlyField] public List<Page> pages;

    public List<GameObject> pageSubjects;

    [field: ReadOnlyField] public List<PlantController> plantSubjects = new List<PlantController>();
    [field: ReadOnlyField] public List<EntityController> animalSubjects = new List<EntityController>();

    public bool leftKeyDown = false;
    public float leftInputTimer = 0;

    public bool rightKeyDown = false;
    public float rightInputTimer = 0;

    private void Awake()
    {
        questManager = FindObjectOfType<QuestManager>();

        SetPageSubjects();
        GoToPage(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            ToggleUI();

        if (journalPanel.activeSelf)
        {
            if (leftPageNum > 0)
            {
                if (Input.GetKeyDown(KeyCode.G))
                    leftKeyDown = true;

                if (!Input.GetKey(KeyCode.G))
                    leftKeyDown = false;

                if (leftKeyDown)
                {
                    leftInputTimer += Time.deltaTime;
                    if (leftInputTimer > 1f)
                    {
                        for (int i = leftPageNum; i > 0; i--)
                        {
                            if (pages[i].pageType == PageType.AnimalIndex)
                            {
                                GoToIndex(true, false);
                                break;
                            }
                            else if (pages[i].pageType == PageType.PlantIndex)
                            {
                                GoToIndex(false, false);
                                break;
                            }

                            if (i - 1 < 0)
                            {
                                TurnPageLeft();
                            }
                        }
                        leftInputTimer = 0;
                        leftKeyDown = false;
                    }
                }
                else if (leftInputTimer > 0)
                {
                    TurnPageLeft();
                    leftInputTimer = 0f;
                }
            }

            if (rightPageNum < pages.Count)
            {
                if (Input.GetKeyDown(KeyCode.H))
                    rightKeyDown = true;

                if (!Input.GetKey(KeyCode.H))
                    rightKeyDown = false;

                if (rightKeyDown)
                {
                    rightInputTimer += Time.deltaTime;
                    if (rightInputTimer > 1f)
                    {
                        for (int i = rightPageNum; i < pages.Count; i++)
                        {
                            if (pages[i].pageType == PageType.AnimalIndex)
                            {
                                GoToIndex(true, true);
                                break;
                            }
                            else if (pages[i].pageType == PageType.PlantIndex)
                            {
                                GoToIndex(false, true);
                                break;
                            }

                            if (i >= pages.Count)
                            {
                                TurnPageRight();
                                break;
                            }
                        }
                        rightInputTimer = 0f;
                        rightKeyDown = false;
                    }
                }
                else if (rightInputTimer > 0)
                {
                    TurnPageRight();
                    rightInputTimer = 0f;
                }
            }
        }
    }

    public void SetPageSubjects()
    {
        for (int i = 0; i < pageSubjects.Count; i++)
        {
            if (pageSubjects[i].GetComponentInChildren<PlantController>())
            {
                plantSubjects.Add(pageSubjects[i].GetComponentInChildren<PlantController>());
            }
            else if (pageSubjects[i].GetComponent<EntityController>())
            {
                animalSubjects.Add(pageSubjects[i].GetComponent<EntityController>());
            }
        }

        pages = new List<Page>(); //First Create the contents page

        Page contents = new Page();
        contents.pageType = PageType.Contents;
        contents.subjectName = "Contents Page";
        pages.Add(contents);

        if (plantSubjects.Count > 0) // create the plant index page
        {
            Page plantIndex = new Page();
            plantIndex.pageType = PageType.PlantIndex;
            plantIndex.subjectName = "Plant Index Page";
            pages.Add(plantIndex);

            List<PlantType> plantTypes = new List<PlantType>();
            SortPlantsByEnum();

            for (int i = 0; i < plantSubjects.Count; i++) // for each plant subject,
            {
                if (!plantTypes.Contains(plantSubjects[i].plantType))// if the plant type doesn't exist add a new index page
                {
                    plantTypes.Add(plantSubjects[i].plantType);

                    Page plantSubIndex = new Page();
                    plantSubIndex.pageType = PageType.PlantSubIndex;
                    plantSubIndex.subjectName = plantSubjects[i].plantType.ToString();

                    pages.Add(plantSubIndex);
                }
                
                if(plantTypes.Contains(plantSubjects[i].plantType)) // othewrwise add a new plant page of that type
                {
                    Page plantPage = new Page();
                    if (plantSubjects[i].defaultProduce != null)
                        plantPage.subjectName = plantSubjects[i].defaultProduce.name + " " + plantSubjects[i].plantType;
                    else
                        plantPage.subjectName = plantSubjects[i].name;

                    plantPage.pageType = PageType.Plant;
                    plantPage.pageSubject = plantSubjects[i].gameObject;
                    plantPage.subType = plantSubjects[i].plantType.ToString();

                    pages.Add(plantPage);
                }
            }
        }


        if (animalSubjects.Count > 0) // create the animal index page
        {
            Page animalIndex = new Page();
            animalIndex.pageType = PageType.AnimalIndex;
            animalIndex.subjectName = "Animal Index Page";
            pages.Add(animalIndex);

            for (int i = 0; i < animalSubjects.Count; i++) // for each animal subject, add a button
            {
                Page animalPage = new Page();
                animalPage.subjectName = animalSubjects[i].GetComponent<EntityController>().entityInfo.entityName;
                animalPage.pageType = PageType.Animal;
                animalPage.pageSubject = animalSubjects[i].gameObject;

                pages.Add(animalPage);
                //SpawnAnimalIndexButton(pages.Count - 1);

            }
        }

        Page questPage = new Page();
        questPage.pageType = PageType.Quest;
        questPage.subjectName = "Tasks";
        pages.Add(questPage);
    }

    public void SpawnIndexButton(int pageNum, GameObject layoutGroup)
    {
        GameObject newPageButton = Instantiate(indexListButton, layoutGroup.transform);
        newPageButton.GetComponentInChildren<TextMeshProUGUI>().text = pages[pageNum].subjectName;
        newPageButton.GetComponent<Button>().onClick.AddListener(() => GoToPage(pageNum));
    }

    //public void SpawnIndexButton(int pageNum, GameObject layoutGroup)
    //{
    //    GameObject newPageButton = Instantiate(indexListButton, layoutGroup.transform);
    //    newPageButton.GetComponentInChildren<TextMeshProUGUI>().text = pages[pageNum].pageSubject.GetComponent<EntityController>().entityInfo.entityName;
    //    newPageButton.GetComponent<Button>().onClick.AddListener(() => GoToPage(pageNum));
    //}

    public void SetAnimalPageText(int pageNum, AnimalPageUI animalPage)
    {
        animalPage.nameText.text = pages[pageNum].pageSubject.GetComponent<EntityController>().entityInfo.entityName;
        animalPage.image.sprite = pages[pageNum].pageSubject.GetComponent<EntityController>().entityInfo.icon;

        animalPage.descText.text = pages[pageNum].pageSubject.GetComponent<EntityController>().entityInfo.entityDesc; // include in entity info

        animalPage.maturityText.text = "They reach maturity at the age of " + pages[pageNum].pageSubject.GetComponent<EntityController>().entityInfo.ageOfMaturity;
        animalPage.activeHoursText.text = "They are active between " + pages[pageNum].pageSubject.GetComponent<EntityStats>().awakeHours.x.ToString("00") + "00 and " + pages[pageNum].pageSubject.GetComponent<EntityStats>().awakeHours.y.ToString("00" + "00");
        animalPage.dietText.text = "They eat: "; // add herbivore/ carnivore/ ominvore enum to entity info, Enum type for the food group of entities & produce (ie. fruits, berries, melons, roots)

        for (int i = 0; i < pages[pageNum].pageSubject.GetComponent<EntityController>().foodList.Count; i++)
        {

            if (pages[pageNum].pageSubject.GetComponent<EntityController>().foodList.Count == 1)
                animalPage.dietText.text += pages[pageNum].pageSubject.GetComponent<EntityController>().foodList[i].foodObject.name + ".";
            else if (i + 1 < pages[pageNum].pageSubject.GetComponent<EntityController>().foodList.Count)
                animalPage.dietText.text += pages[pageNum].pageSubject.GetComponent<EntityController>().foodList[i].foodObject.name + ", ";
            else
                animalPage.dietText.text += "and " + pages[pageNum].pageSubject.GetComponent<EntityController>().foodList[i].foodObject.name + ".";
        }

        animalPage.habitatText.text = "(Habitat)"; // add nesting behaviours
        animalPage.unlockText.text = "(Unlock Conditions)"; // requirements to unlock the animal
        //animalPage.pageNumText.text = pageNum.ToString();
    }

    public void SetPlantPageText(int pageNum, PlantPageUI plantPage)
    {
        plantPage.nameText.text = pages[pageNum].subjectName;
        plantPage.plantImage.sprite = pages[pageNum].pageSubject.GetComponent<PlantController>().icon;
        if (pages[pageNum].pageSubject.GetComponent<PlantController>().defaultProduce != null)
        {
            plantPage.produceImage.sprite = pages[pageNum].pageSubject.GetComponent<PlantController>().defaultProduce.icon;
            plantPage.produceText.text = "Produces " + pages[pageNum].pageSubject.GetComponent<PlantController>().defaultProduce.name + " when mature."; // include in entity info
        }
        else
        {
            plantPage.produceImage.enabled = false;
            plantPage.produceText.enabled = false;
        }

        plantPage.descText.text = pages[pageNum].pageSubject.GetComponent<PlantController>().plantDesc; // include in entity info


        plantPage.maturityText.text = "They reach maturity at the age of " + pages[pageNum].pageSubject.GetComponent<PlantController>().ageOfMaturity; // expand to include time period
        plantPage.tempText.text = "Requires temperatures between " + pages[pageNum].pageSubject.GetComponent<PlantController>().requirements.tempRange.x + " and " + pages[pageNum].pageSubject.GetComponent<PlantController>().requirements.tempRange.y + " degrees";
        plantPage.sunText.text = "Requires cloud coverage between " + pages[pageNum].pageSubject.GetComponent<PlantController>().requirements.cloudCoverageRange.x + " and " + pages[pageNum].pageSubject.GetComponent<PlantController>().requirements.cloudCoverageRange.y + " levels";
        plantPage.moistureText.text = "Requires water levels between " + pages[pageNum].pageSubject.GetComponent<PlantController>().requirements.requiredWaterRange.x + " and " + pages[pageNum].pageSubject.GetComponent<PlantController>().requirements.requiredWaterRange.y;
        plantPage.seasonText.text = "Grows in ";

        for (int i = 0; i < pages[pageNum].pageSubject.GetComponent<PlantController>().requirements.season.Count; i++)
        {
            if (pages[pageNum].pageSubject.GetComponent<PlantController>().requirements.season.Count == 1)
                plantPage.seasonText.text += pages[pageNum].pageSubject.GetComponent<PlantController>().requirements.season[i];
            else if (i < pages[pageNum].pageSubject.GetComponent<PlantController>().requirements.season.Count - 1)
                plantPage.seasonText.text += pages[pageNum].pageSubject.GetComponent<PlantController>().requirements.season[i] + ", ";
            else
                plantPage.seasonText.text += " and " + pages[pageNum].pageSubject.GetComponent<PlantController>().requirements.season[i];
        }


        plantPage.weatherText.text = "(Prefered Weather)"; // change weather conditions to improve produce quality when met rather than decrease health when not met

        //for (int i = 0; i < pages[pageNum].pageSubject.GetComponent<PlantController>().requirements.weather.Count; i++)
        //{
        //    if (pages[pageNum].pageSubject.GetComponent<PlantController>().requirements.weather.Count == 1)
        //        plantPageUI.plantWeatherText.text += pages[pageNum].pageSubject.GetComponent<PlantController>().requirements.weather[i];
        //    else if (i < pages[pageNum].pageSubject.GetComponent<PlantController>().requirements.weather.Count - 1)
        //        plantPageUI.plantWeatherText.text += pages[pageNum].pageSubject.GetComponent<PlantController>().requirements.weather[i] + ", ";
        //    else
        //        plantPageUI.plantWeatherText.text += " and " + pages[pageNum].pageSubject.GetComponent<PlantController>().requirements.weather[i];
        //}

        plantPage.unlockText.text = "(Unlock Conditions)"; // requirements to unlock the plant
        //plantPage.pageNumText.text = pageNum.ToString();
    }

    public void SpawnContentsPage(bool isLeft)
    {
        GameObject newContentsPage = Instantiate(contentsPage, isLeft ? leftPage.transform : rightPage.transform);

        foreach (Transform c in newContentsPage.transform)
        {
            if (c.gameObject.GetComponent<Button>())
            {
                if (c.GetComponentInChildren<TextMeshProUGUI>().text.Contains("Animal"))
                {
                    c.gameObject.GetComponent<Button>().onClick.AddListener(() => GoToIndex(true, true));
                    //Debug.Log("animal index listener added");

                }
                else if (c.GetComponentInChildren<TextMeshProUGUI>().text.Contains("Plant"))
                {
                    c.gameObject.GetComponent<Button>().onClick.AddListener(() => GoToIndex(false, true));
                    //Debug.Log("plant index listener added");
                } else if (c.GetComponentInChildren<TextMeshProUGUI>().text.Contains("Task"))
                {
                    for (int i = 0; i < pages.Count; i++)
                    {
                        if (pages[i].pageType == PageType.Quest)
                        {
                            c.gameObject.GetComponent<Button>().onClick.AddListener(() => GoToPage(i));
                            break;
                        }

                    }
                    //Debug.Log("plant index listener added");
                }
            }
            //else Debug.Log("Doesn't have a button");
        }
    }

    public void SpawnIndexPage(bool isLeft, bool isAnimal)
    {

        if (isAnimal) // Needs to be changed when entitytypes are added -> currently just creates a general index page of all animals
        {
            GameObject newIndexPage = Instantiate(animalIndexPage.gameObject, isLeft ? leftPage.transform : rightPage.transform);

            animalIndexPageLayoutGroup = newIndexPage.GetComponentInChildren<GridLayoutGroup>().gameObject;
            for (int i = 0; i < pages.Count; i++)
            {
                if (pages[i].pageType == PageType.Animal) // change to animalsubindex when entity types are added
                    SpawnIndexButton(i, animalIndexPageLayoutGroup);
            }
        }
        else
        {
            GameObject newIndexPage = Instantiate(plantIndexPage.gameObject, isLeft ? leftPage.transform : rightPage.transform);

            plantIndexPageLayoutGroup = newIndexPage.GetComponentInChildren<GridLayoutGroup>().gameObject;
            for (int i = 0; i < pages.Count; i++)
            {
                if (pages[i].pageType == PageType.PlantSubIndex)
                    SpawnIndexButton(i, plantIndexPageLayoutGroup);
            }
        }
    }

    public void SpawnSubIndexPage(bool isLeft, bool isAnimal, int pageIndex)
    {
        if (isAnimal) 
        {
            GameObject newIndexPage = Instantiate(animalSubIndexPage.gameObject, isLeft ? leftPage.transform : rightPage.transform);

            animalIndexPageLayoutGroup = newIndexPage.GetComponentInChildren<GridLayoutGroup>().gameObject;
            for (int i = 0; i < pages.Count; i++)
            {
                if (pages[i].pageType == PageType.Animal)
                    SpawnIndexButton(i, animalIndexPageLayoutGroup);
            }
        }
        else
        {
            GameObject newIndexPage = Instantiate(plantSubIndexPage.gameObject, isLeft ? leftPage.transform : rightPage.transform);

            newIndexPage.GetComponentInChildren<TextMeshProUGUI>().text = pages[pageIndex].subjectName;

            plantIndexPageLayoutGroup = newIndexPage.GetComponentInChildren<GridLayoutGroup>().gameObject;
            for (int i = 0; i < pages.Count; i++)
            {
                if (pages[i].pageType == PageType.Plant && pages[i].subType == pages[pageIndex].subjectName)
                    SpawnIndexButton(i, plantIndexPageLayoutGroup);
            }
        }
    }

    public void SpawnInfoPage(bool isLeft, bool isAnimal, int pageNum)
    {
        if (isAnimal)
        {
            GameObject newAnimalPage = Instantiate(animalPage.gameObject, isLeft ? leftPage.transform : rightPage.transform);

            SetAnimalPageText(pageNum, newAnimalPage.GetComponent<AnimalPageUI>());

        }
        else
        {
            GameObject newPlantPage = Instantiate(plantPage.gameObject, isLeft ? leftPage.transform : rightPage.transform);
            SetPlantPageText(pageNum, newPlantPage.GetComponent<PlantPageUI>());


        }
    }

    public void SpawnQuestPage(bool isLeft)
    {
        GameObject newPage = Instantiate(questPage.gameObject, isLeft ? leftPage.transform : rightPage.transform);

        questManager.questUIPanel = newPage.transform.GetChild(1).gameObject;
        //questManager.

    }

    public void SpawnRelevantPage(int pageNum)
    {
        switch (pages[pageNum].pageType) // spawn the desired page 
        {
            case PageType.Contents:
                SpawnContentsPage(IsEven(pageNum));
                break;
            case PageType.AnimalIndex:
                SpawnIndexPage(IsEven(pageNum), true);
                break;
            case PageType.AnimalSubIndex:
                SpawnSubIndexPage(IsEven(pageNum), true, pageNum);
                break;
            case PageType.Animal:
                SpawnInfoPage(IsEven(pageNum), true, pageNum);
                break;
            case PageType.PlantIndex:
                SpawnIndexPage(IsEven(pageNum), false);
                break;
            case PageType.PlantSubIndex:
                SpawnSubIndexPage(IsEven(pageNum), false, pageNum);
                break;
            case PageType.Plant:
                SpawnInfoPage(IsEven(pageNum), false, pageNum);
                break; 
            case PageType.Quest:
                SpawnQuestPage(IsEven(pageNum));
                break;
        }
    }

    public void GoToPage(int pageNum)
    {
        foreach (Transform c in leftPage.transform)
        { Destroy(c.gameObject); }
        foreach (Transform c in rightPage.transform)
        { Destroy(c.gameObject); }

        //Debug.Log("Going to page " + pageNum);

        if (IsEven(pageNum)) // if the desired page is on the left
        {
            leftPageNum = pageNum;
            rightPageNum = pageNum + 1;
        }
        else //if the desired page is on the right
        {
            leftPageNum = pageNum - 1;
            rightPageNum = pageNum;
        }

        if (leftPageNum >= 0 && leftPageNum < pages.Count)
            SpawnRelevantPage(leftPageNum);

        if (rightPageNum >= 0 && rightPageNum < pages.Count)
            SpawnRelevantPage(rightPageNum);

        //Debug.Log("flipped to pages " + (leftPageNum).ToString() + " & " + (rightPageNum).ToString());
        //Debug.Log("flipped to pages " + (leftPageNum).ToString() + " (" + pages[leftPageNum].subjectName + ") & " + (rightPageNum).ToString() + " (" + pages[rightPageNum].subjectName + ")");

    }

    public void TurnPageLeft()
    {

        if (leftPageNum > 0)
        {
            //Debug.Log("turning back from pages " + (leftPageNum).ToString() + " (" + pages[leftPageNum].subjectName + ") & " + (rightPageNum).ToString() + " (" + pages[rightPageNum].subjectName + ")");
            GoToPage(leftPageNum - 1);
            Debug.Log("turned back to pages " + (leftPageNum).ToString() + " (" + pages[leftPageNum].subjectName + ") & " + (rightPageNum).ToString() + " (" + pages[rightPageNum].subjectName + ")");

        }
    }
    public void TurnPageRight()
    {
        if (rightPageNum + 1 < pages.Count)
        {
            //Debug.Log("turning forward from pages " + (leftPageNum).ToString() + " & " + (rightPageNum).ToString());
            GoToPage(rightPageNum + 1);
            Debug.Log("turned forward to pages " + (leftPageNum).ToString() + " & " + (rightPageNum).ToString());
        }


    }
    public void GoToIndex(bool isAnimalIndex, bool isTurningRight)
    {
        if (isAnimalIndex)
        {
            if (isTurningRight)
            {
                for (int i = 0; i < pages.Count; i++)
                {
                    if (pages[i].pageType == PageType.AnimalIndex)
                    {
                        GoToPage(i);
                        return;
                    }
                }
            }
            else
            {
                for (int i = pages.Count - 1; i > 0; i--)
                {
                    if (pages[i].pageType == PageType.AnimalIndex)
                    {
                        GoToPage(i);
                        return;
                    }
                }

                if (pages[0].pageType == PageType.Contents)
                {
                    GoToPage(0);
                    return;
                }
            }
        }
        else
        {
            if (isTurningRight)
            {
                for (int i = 0; i < pages.Count; i++)
                {
                    if (pages[i].pageType == PageType.PlantIndex)
                    {
                        GoToPage(i);
                        return;
                    }
                }
            }
            else
            {
                for (int i = pages.Count - 1; i > 0; i--)
                {
                    if (pages[i].pageType == PageType.PlantIndex)
                    {
                        GoToPage(i);
                        return;
                    }
                }
                if (pages[0].pageType == PageType.Contents)
                {
                    GoToPage(0);
                    return;
                }
            }
        }
    }

    bool IsEven(int n)
    {
        return n % 2 == 0;
    }

    public void ToggleUI()
    {
        journalPanel.SetActive(!journalPanel.activeSelf);
        GoToPage(0);

        if (journalPanel.activeSelf)
        {
            //cam.freezeCameraRotation = true;
            //player.GetComponent<PlayerAttack>().enabled = false;

            //Cursor.lockState = CursorLockMode.Confined;
            //Cursor.visible = true;
            Pause.instance.freezeCameraRotation = true;
            Pause.instance.unlockCursor = true;
        }
        else
        {
            //cam.freezeCameraRotation = false;
            //player.GetComponent<PlayerAttack>().enabled = true;

            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
            Pause.instance.freezeCameraRotation = false;
            Pause.instance.unlockCursor = false;
        }
    }

    int ComparePlantsByEnum(PlantController a, PlantController b)
    {
        return a.plantType.CompareTo(b.plantType);
    }

    public void SortPlantsByEnum()
    {
        plantSubjects.Sort(ComparePlantsByEnum);   
    }
    // int CompareEntitiesByEnum(EntityController a, EntityController b)
    //{
    //    return a.entityInfo.entityType.CompareTo(b.entityInfo.entityType);
    //}

    //public void SortEntitiesByEnum()
    //{
    //    animalSubjects.Sort(CompareEntitiesByEnum);   
    //}

}

[System.Serializable]
public enum PageType
{
    Blank, Contents, Animal, AnimalIndex, AnimalSubIndex, Plant, PlantIndex, PlantSubIndex, Quest
}