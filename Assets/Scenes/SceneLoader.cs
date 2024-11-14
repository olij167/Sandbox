using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CheckMethod
{
    Distance, Trigger, Interaction
}

public class SceneLoader : Interactable
{
    private PlayerController player;
    //public CheckMethod checkMethod;
    //public float loadDistance;
    public string mainScene = "ParkDemo";
    public string rootScene;
    public string sceneToLoad;

    [field: ReadOnlyField] public  bool isLoaded;
    [field: ReadOnlyField] public  bool shouldLoad;

    public bool setSpawnPoint;
    public bool goToSpawnPoint;
    public bool setSkybox;
    public Material newSkybox;
    //public bool hideBackgroundElements;
    //public List<GameObject> backgroundElements;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();

        if (sceneToLoad == mainScene) isLoaded = true;

    }

    private void FixedUpdate()
    {
        if (!SceneManager.GetSceneByName(sceneToLoad).isLoaded) isLoaded = false;
    }

    //private void Update()
    //{
    //    if (checkMethod == CheckMethod.Distance)
    //    {
    //        DistanceCheck();
    //    }
    //    else if (checkMethod == CheckMethod.Trigger)
    //    {
    //        TriggerCheck();
    //    }
    //}

    //void DistanceCheck()
    //{
    //    if (Vector3.Distance(player.transform.position, transform.position) < loadDistance)
    //    {
    //        LoadScene();
    //    }
    //    else UnloadScene();

    //}

    public void LoadScene()
    {
        if (!isLoaded)
        {

            StartCoroutine(BackgroundLoad());
            //isLoaded = true;

            if (setSkybox && newSkybox != null) RenderSettings.skybox = newSkybox; 

            if (setSpawnPoint) player.spawnPoint = player.transform.position;

           

        }
        else
        {
            if (goToSpawnPoint) player.transform.position = player.spawnPoint;

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoad));

        }
    }

    IEnumerator BackgroundLoad()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);


        while (!async.isDone)
        {
            // float progressValue = Mathf.Clamp01(async.progress / 0.9f);

            yield return null;
        }

        if (async.isDone)
        {
            isLoaded = true;
        }
    }


    public void UnloadScene()
    {
        if (goToSpawnPoint)
            player.returnToSpawn = true;


        if (isLoaded)
            StartCoroutine(BackgroundUnload());

    }

    IEnumerator BackgroundUnload()
    {
        AsyncOperation async = SceneManager.UnloadSceneAsync(rootScene);

        while (!async.isDone)
        {

            yield return null;
        }

        if (async.isDone)
        {
            isLoaded = false;
        }
    }

    void TriggerCheck()
    {
        if (shouldLoad)
        {
            LoadScene();
        }
        else
        {
            UnloadScene();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shouldLoad = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            shouldLoad = false;
        }
    }

}
