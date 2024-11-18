using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public GameObject menuButtons;
    public GameObject settingsMenuPanel;

    public void ToggleSettingsMenu()
    {
        settingsMenuPanel.SetActive(!settingsMenuPanel.activeSelf);
        menuButtons.SetActive(!settingsMenuPanel.activeSelf);
    }
}
