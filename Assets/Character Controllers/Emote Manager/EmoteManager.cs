using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EmoteManager : MonoBehaviour
{

    private PlayerController player;
    private ThirdPersonCam cam;
    private PlayerInventory inventory;
    private PlayerAbilities playerAbilities;

    public Emote activeEmote;
    public List<Emote> emotes;
    public List<Emote> allEmotes;

    public int selectedEmoteSlot = 1;
    [SerializeField] private KeyCode emoteWindowInput = KeyCode.U;


    [Header("UI Elements")]
    public GameObject emoteUI;
    [SerializeField] private GameObject emoteUIPrefab;
    [SerializeField] private GameObject emoteSlotPrefab;
    public int emoteSlotNum = 12;
    [SerializeField] private GameObject emoteBarPanel;
    [SerializeField] private GameObject emoteWindowPanel;
    [SerializeField] private bool emoteWindowOpen;
    [SerializeField] private TextMeshProUGUI emoteTextPrompts;
    [SerializeField] private TextPopUp textPopUp;
    [SerializeField] private EmoteSlot[] emoteSlots;

    [SerializeField] private Color selectedColour;
    private Color originalColour;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        cam = FindObjectOfType<ThirdPersonCam>();
        inventory = FindObjectOfType<PlayerInventory>();
        playerAbilities = FindObjectOfType<PlayerAbilities>();
        emoteSlots = new EmoteSlot[emoteBarPanel.transform.childCount + emoteSlotNum];

        //for (int i = 0; i < emoteSlots.Length; i++)
        //{
        //    emotes.Add(null);
        //}

        foreach (Transform child in emoteWindowPanel.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < emoteBarPanel.transform.childCount + emoteSlotNum; i++)
        {
            if (i < emoteBarPanel.transform.childCount)
            {
                emoteSlots[i] = emoteBarPanel.transform.GetChild(i).GetComponent<EmoteSlot>();
                emoteSlots[i].slot = i;
            }
            else
            {
                //emoteSlots[i] = emoteWindowPanel.transform.GetChild(i - emoteBarPanel.transform.childCount).GetComponent<EmoteSlot>();
                emoteSlots[i] = Instantiate(emoteSlotPrefab, emoteWindowPanel.transform).GetComponent<EmoteSlot>();
                emoteSlots[i].slot = i;

            }
        }
        originalColour = emoteSlots[0].GetComponent<Image>().color;

        for (int i = 0; i < emotes.Count; i++)
        {
            //AddEmoteUiToInventory(emotes[i]);
            //if ()
            SpawnNewEmoteUI(emotes[i], i);
        }

    }

    private void Update()
    {
        if (emoteUI.activeSelf)
        {
            SelectEmoteWithNumbers();
            SelectEmoteWithScroll();
        }

        if (emoteSlots[selectedEmoteSlot].emoteItem != null && activeEmote != emoteSlots[selectedEmoteSlot].emoteItem.emote)
        {
            activeEmote = emoteSlots[selectedEmoteSlot].emoteItem.emote;
        }

        if (player.emote != activeEmote)
            player.emote = activeEmote;

        if (Input.GetKeyDown(emoteWindowInput))
        {
            if (!emoteUI.activeSelf)
            {
                emoteUI.SetActive(true);
                inventory.inventoryUI.SetActive(false);
                playerAbilities.abilityUI.SetActive(false);
                emoteWindowOpen = inventory.inventoryWindowOpen;
                
            }
            else
            {
                emoteWindowOpen = !emoteWindowOpen;
            }
        }

        if (emoteUI.activeSelf)
        {
            inventory.inventoryUI.SetActive(false);
            playerAbilities.abilityUI.SetActive(false);

            if (activeEmote != null)
            {
                SetSelectedEmoteColour();
                emoteTextPrompts.gameObject.SetActive(true);
                emoteTextPrompts.text = activeEmote.itemName;
            }
            else
            {
                emoteTextPrompts.text = null;
                emoteTextPrompts.gameObject.SetActive(false);

                activeEmote = null;
                SetSelectedEmoteColour();
            }
        }
        //else
        //{
        //    //emoteTextPrompts.text = null;
        //    //emoteTextPrompts.gameObject.SetActive(false);
        //    //activeEmote = null;
        //    //inventory.inventoryUI.SetActive(true);

        //    SetSelectedEmoteColour();
        //}


        if (emoteWindowOpen && !emoteWindowPanel.activeSelf)
        {
            emoteWindowPanel.SetActive(true);
            cam.freezeCameraRotation = true;

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else if (!emoteWindowOpen && emoteWindowPanel.activeSelf)
        {
            emoteWindowPanel.SetActive(false);
            cam.freezeCameraRotation = false;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void AddEmoteToInventory(Emote emote, GameObject emoteInWorld)
    {

        if (CheckEmptySlots(emoteSlots) > 0)
        {
            for (int s = 0; s < emoteSlots.Length; s++)
            {
                if (emoteSlots[s].transform.childCount == 0) // for the first empty slot
                {
                    SpawnNewEmoteUI(emote, s);

                    if (emoteInWorld.GetComponent<EmoteInWorld>())
                    {
                        Destroy(emoteInWorld);
                        //firstPersonRaycast.selectedObject = null;
                    }

                    //emoteItem.InitialiseEmote(emote);

                    emotes.Add(emote);
                    //emotes[s] = emote;

                    textPopUp.SetAndDisplayPopUp(emote.itemName + " Emote Collected");
                    return;
                }
            }
        }
    }

    void SpawnNewEmoteUI(Emote emote, int uiSlot)
    {
        GameObject newEmoteUI = Instantiate(emoteUIPrefab, emoteSlots[uiSlot].transform);
        EmoteItem emoteItem = newEmoteUI.GetComponent<EmoteItem>();

        emoteSlots[uiSlot].emoteItem = emoteItem;

        Button button = newEmoteUI.AddComponent<Button>();
        button.onClick.AddListener(() => SelectEmoteUIAsButton(uiSlot));

        emoteItem.InitialiseEmote(emote, uiSlot);

        //emotes[uiSlot] = emoteItem;

    }


    int CheckEmptySlots(EmoteSlot[] slots)
    {
        int emptySlots = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].transform.childCount == 0)
            {
                emptySlots++;
            }
        }

        return emptySlots;
    }

    public void SelectEmoteUIAsButton(int index)
    {
        if (index <= emoteSlots.Length && emoteSlots[index] != null)
        {
            selectedEmoteSlot = index;

            activeEmote = emoteSlots[index].emoteItem.emote;
            //player.animator.SetInteger("Emote", activeEmote.itemID);

            ////animator.SetBool("isEmoting", true);
            //player.isEmoting = true;
            Debug.Log("emote set with UI Button");
        }
        //SetSelectedEmoteColour();

    }

    private void SelectEmoteWithNumbers()
    {
        if (emoteSlots.Length >= 1)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (emoteSlots.Length >= 1)
                {
                    selectedEmoteSlot = 0;
                    if (emotes[selectedEmoteSlot] != null)
                    {
                        activeEmote = emotes[0];
                    }
                }
            }
        }
        if (emoteSlots.Length >= 2)
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (emoteSlots.Length >= 2)
                {
                    selectedEmoteSlot = 1;
                    if (emotes[selectedEmoteSlot] != null)
                    {
                        activeEmote = emotes[1];
                    }
                }
            }
        }
        if (emoteSlots.Length >= 3)
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (emoteSlots.Length >= 3)
                {
                    selectedEmoteSlot = 2;
                    if (emotes[selectedEmoteSlot] != null)
                    {
                        activeEmote = emotes[2];
                    }
                }
            }
        }
        if (emoteSlots.Length >= 4)
        {
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (emoteSlots.Length >= 4)
                {
                    selectedEmoteSlot = 3;
                    if (emotes[selectedEmoteSlot] != null)
                    {
                        activeEmote = emotes[3];
                    }
                }
            }
        }
        if (emoteSlots.Length >= 5)
        {
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                if (emoteSlots.Length >= 5)
                {
                    selectedEmoteSlot = 4;
                    if (emotes[selectedEmoteSlot] != null)
                    {
                        activeEmote = emotes[4];
                    }
                }
            }
        }
        if (emoteSlots.Length >= 6)
        {
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                if (emoteSlots.Length >= 6)
                {
                    selectedEmoteSlot = 5;
                    if (emotes[selectedEmoteSlot] != null)
                    {
                        activeEmote = emotes[5];
                    }
                }
            }
        }
        if (emoteSlots.Length >= 7)
        {
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                if (emoteSlots.Length >= 7)
                {
                    selectedEmoteSlot = 6;
                    if (emotes[selectedEmoteSlot] != null)
                    {
                        activeEmote = emotes[6];
                    }
                }
            }
        }
        if (emoteSlots.Length >= 8)
        {
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                if (emoteSlots.Length >= 8)
                {
                    selectedEmoteSlot = 7;
                    if (emotes[selectedEmoteSlot] != null)
                    {
                        activeEmote = emotes[7];
                    }
                }
            }
        }
        if (emoteSlots.Length >= 9)
        {
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                if (emoteSlots.Length >= 9)
                {
                    selectedEmoteSlot = 8;

                    if (emotes[selectedEmoteSlot] != null)
                    {
                        activeEmote = emotes[8];
                    }
                }
            }
        }
        if (emoteSlots.Length == 10)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                if (emoteSlots.Length >= 10)
                {
                    selectedEmoteSlot = 9;

                    if (emotes[selectedEmoteSlot] != null)
                    {
                        activeEmote = emotes[9];
                    }
                }
            }
        }
    }

    private void SelectEmoteWithScroll()
    {
        if (Input.mouseScrollDelta != Vector2.zero)
        {
            selectedEmoteSlot += Mathf.RoundToInt(Input.mouseScrollDelta.y);
            //selectedItemSlot += Mathf.Clamp(Mathf.RoundToInt(Input.mouseScrollDelta.y), 1, inventorySlots.Length);

            if (emoteWindowOpen)
            {
                int lastFilledSlot = 0;

                for (int i = 0; i < emoteSlots.Length; i++)
                {
                    if (emoteSlots[i].transform.childCount >= 0)
                        lastFilledSlot = i;
                }

                //Debug.Log("Last filled slot = " + lastFilledSlot);

                if (selectedEmoteSlot > lastFilledSlot)
                {
                    selectedEmoteSlot = 0;
                }
                else if (selectedEmoteSlot < 0)
                {
                    selectedEmoteSlot = lastFilledSlot;
                }
            }
            else
            {
                //only scroll through filled emote slots


                if (selectedEmoteSlot > emoteSlots.Length - emoteWindowPanel.transform.childCount - 1)
                {
                    selectedEmoteSlot = 0;
                }
                else if (selectedEmoteSlot < 0)
                {
                    selectedEmoteSlot = emoteSlots.Length - emoteWindowPanel.transform.childCount - 1;
                }
            }


            if (emoteSlots[selectedEmoteSlot].emoteItem != null)
            {
                activeEmote = emoteSlots[selectedEmoteSlot].emoteItem.emote;
                //SetSelectedEmoteColour();
            }
        }
    }

    private void SetSelectedEmoteColour()
    {
        for (int i = 0; i < emoteSlots.Length; i++)
        {
            if (i == selectedEmoteSlot)
            {
                emoteSlots[i].GetComponent<Image>().color = selectedColour;
            }
            else emoteSlots[i].GetComponent<Image>().color = originalColour;
        }
    }

    public void SwapEmoteSlot(int indexA, int indexB)
    {
        // Store emote A info
        EmoteItem emoteItem = emoteSlots[indexA].emoteItem;

        //Swap Slot A Info for slot B
        emoteSlots[indexA].emoteItem = emoteSlots[indexB].emoteItem;

        // Swap slot B info for stored A info
        emoteSlots[indexB].emoteItem = emoteItem;

        //Delete & Spawn UI

        if (emoteSlots[indexA].transform.childCount > 0)
        {
            foreach (Transform child in emoteSlots[indexA].transform)
            {
                Destroy(child.gameObject);
            }
            //SpawnNewEmoteUI(emoteSlots[indexB].emoteItem.emote, indexB);

        }
        if (emoteSlots[indexB].transform.childCount > 0)
        {
            foreach (Transform child in emoteSlots[indexB].transform)
            {
                Destroy(child.gameObject);
            }
            SpawnNewEmoteUI(emoteSlots[indexA].emoteItem.emote, indexA); //a = originally B

        }

        if (emoteItem != null)
        SpawnNewEmoteUI(emoteSlots[indexB].emoteItem.emote, indexB); // originally A

        Debug.Log("Swapped " + indexA + " UI Slot with " + indexB + " UI Slot");

    }
}
