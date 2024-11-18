using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeScene : Interactable
{

    //public Animator transition;

    //public float transitionTime = 2f;

    ////public GameObject menuButtons;

    //public GameObject loadingBar;
    //public Image loadingBarFill;

    //public Camera loadingCam;

    //public GameObject menuElements;
    PlayerController player;

    public bool setSpawnPoint;
    public bool goToSpawnPoint;

    public string sceneToLoad;

    private void Awake()
    {
        //loadingBar.SetActive(false);
        if (FindObjectOfType<PlayerController>())
        {
            player = FindObjectOfType<PlayerController>();
        }
    }

    public void MoveToScene(string sceneID)
    {
        //transition.SetTrigger("Start 0");
        //transition.SetBool("startTran", true);

        //SceneManager.LoadScene(sceneID);

        if (setSpawnPoint && player != null)
        {
            player.spawnPoint = player.transform.position;
        }
        
            

        StartCoroutine(LoadAsyncScene(sceneID));
    }

    //IEnumerator LoadLevel(int sceneID)
    //{

    //    yield return new WaitForSeconds(transitionTime);

    //    SceneManager.LoadScene(sceneID);
    //}

    public IEnumerator LoadAsyncScene(string sceneID)
    {
        //loadingCam.enabled = true;
        //Camera.main.enabled = false;

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneID);

        //loadingBar.SetActive(true);
        //menuElements.SetActive(false);

        while (!async.isDone)
        {

            float progressValue = Mathf.Clamp01(async.progress / 0.9f);

            //loadingBarFill.fillAmount = progressValue;

            yield return null;
        }

        if (async.isDone && player != null && goToSpawnPoint) player.transform.position = player.spawnPoint;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
