using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeWeather;
using JetBrains.Annotations;
using UnityEngine.UI;
using TMPro;

public class Sleep : MonoBehaviour
{
    public static Sleep instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else instance = this;
    }

    TimeController timeController;
    public GameObject sleepPanel;

    public float sleepDuration;
    public Slider sleepTimeSlider;

    public TextMeshProUGUI sleepDurationText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dateText;

    Pause pause;

    private void Start()
    {
        timeController = TimeController.instance;
        pause = Pause.instance;
    }

    private void Update()
    {
        if (sleepPanel.activeSelf)
        {
            timeText.text = timeController.timeString;
            dateText.text = timeController.dayText.text;

            sleepDuration = sleepTimeSlider.value;

            sleepDurationText.text = sleepDuration.ToString("00.00") + " Hours";
        }
    }

    [ContextMenu("Toggle Sleep Panel")]
    public void ToggleSleepPanel()
    {
        sleepPanel.SetActive(!sleepPanel.activeSelf);

        //pause.freezeMovement = sleepPanel.activeSelf;
        pause.freezeCameraRotation = sleepPanel.activeSelf;
        pause.unlockCursor = sleepPanel.activeSelf;
    }

    //public float SetSleepDuration()
    //{
    //    return sleepDuration = sleepTimeSlider.value;
    //}

    public void StartSleep()
    {
        StartCoroutine(GoToSleep(sleepDuration));
    }

    public IEnumerator GoToSleep(float sleepDuration)
    {
        //fade to black
        //progress time
        if (timeController.timeOfDay + sleepDuration < 24)
        {
            timeController.timeOfDay += sleepDuration;
            yield return null;
        }
        else
        {
            TimeController.Day today = timeController.currentDay;

            float timeUntilNewDay = 23.99f - timeController.timeOfDay;
            timeController.timeOfDay += timeUntilNewDay;

            yield return new WaitUntil(() => timeController.currentDay != today);

            Debug.Log("New Day, continue progressing time");

            if (timeController.currentDay != today)
            {
                timeController.timeOfDay += sleepDuration - timeUntilNewDay;
            }

            Debug.Log("Time to wake up");
            yield return null;
        }
        //fade back
    }
}
