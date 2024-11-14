using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RightClickMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //right click on inventory items
    //spawn a menu at the mouse position
    //fill with item specific buttons

    public GameObject menuPanel;
    //public Vector3 
    public GameObject currentPanel;

    public bool isMousedOver;
    public bool itemIsMousedOver;
    public float maxDistanceFromMenu;

    private void LateUpdate()
    {
        if (currentPanel != null)
        {
            //Debug.DrawLine(currentPanel.transform.position, new Vector3(maxDistanceFromMenu, currentPanel.transform.position.y, currentPanel.transform.position.z), Color.yellow);
            //Debug.DrawLine(currentPanel.transform.position, new Vector3(-maxDistanceFromMenu, currentPanel.transform.position.y, currentPanel.transform.position.z), Color.white);
            //Debug.DrawLine(currentPanel.transform.position, new Vector3(currentPanel.transform.position.x, maxDistanceFromMenu, currentPanel.transform.position.z), Color.yellow);
            //Debug.DrawLine(currentPanel.transform.position, new Vector3( currentPanel.transform.position.x, -maxDistanceFromMenu, currentPanel.transform.position.z), Color.white);
            if (!isMousedOver && !itemIsMousedOver)
            {
                CheckMouseDistanceFromMenu();
            }
        }
    }
    public void CreateMenu()
    {
        if (currentPanel != null) DestroyMenu();

       currentPanel = Instantiate(menuPanel, Input.mousePosition, Quaternion.identity, transform);

    }

    public void DestroyMenu()
    {
        Destroy(currentPanel);
    }

    //Detect if the Cursor starts to pass over the GameObject
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        isMousedOver = true;


    }

    //Detect when Cursor leaves the GameObject
    public void OnPointerExit(PointerEventData pointerEventData)
    {
        isMousedOver = false;
    }

    public void CheckMouseDistanceFromMenu()
    {
        if (Vector3.Distance(Input.mousePosition, currentPanel.transform.position) > maxDistanceFromMenu)
            DestroyMenu();
    }

    public void OnDrawGizmos()
    {
        if (currentPanel != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(currentPanel.transform.position, Vector3.one * maxDistanceFromMenu);
        }
    }
}
