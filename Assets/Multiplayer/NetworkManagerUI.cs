using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    public GameObject menuCamera;

    [SerializeField] private Button serverButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    private void Awake()
    {
        serverButton.onClick.AddListener(()=> { NetworkManager.Singleton.StartServer();
            menuCamera.SetActive(false); });
        hostButton.onClick.AddListener(()=> { NetworkManager.Singleton.StartHost(); menuCamera.SetActive(false); });
        clientButton.onClick.AddListener(()=> { NetworkManager.Singleton.StartClient(); menuCamera.SetActive(false); });
    }
}
