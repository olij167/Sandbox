using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCamera : MonoBehaviour
{
    public List<Camera> cameraList;
    public Camera currentCam;
    public int count;

    private List<SetParent> setParent;
    public Billboard moonBillboard;

    private SetCamera setCamera;

    private void Start()
    {
        setParent = new List<SetParent>();
        foreach (SetParent s in FindObjectsOfType<SetParent>()) 
        { 
            if (s.notACamera)
                setParent.Add(s); 
        }

        setCamera = GetComponent<SetCamera>();

        foreach (Camera cam in cameraList)
            if (cam != currentCam) cam.gameObject.SetActive(false);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            currentCam.gameObject.SetActive(false);

            if (count + 1 < cameraList.Count) count++;
            else count = 0;

            currentCam = cameraList[count];

            foreach (SetParent s in setParent)
            {
                s.parentTransform = currentCam.transform;
                s.SetNewParent();
            }

            setCamera.isFirstPerson = !setCamera.isFirstPerson;
            setCamera.SetActiveCamera();

            currentCam.gameObject.SetActive(true);

            moonBillboard.camTransform = currentCam.transform;
        }
    }
}
