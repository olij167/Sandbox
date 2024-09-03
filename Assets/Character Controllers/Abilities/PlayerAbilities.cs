using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerAbilities : MonoBehaviour
{
    [HideInInspector] public PlayerController player;
    private ThirdPersonCam cam;
    private PlayerInventory inventory;
    private EmoteManager emoteManager;

    public bool effectActive;

    public Ability activeAbility;
    public int selectedAbilitySlot = 0;
    public List<Ability> abilities;
    public List<Ability> allAbilities;
    private AbilitySlot[] abilitySlots;

    [SerializeField] private bool abilityWindowOpen;
    [SerializeField] private TextMeshProUGUI abilityTextPrompts;
    [SerializeField] private TextPopUp textPopUp;

    public GameObject abilityUI;
    [SerializeField] private GameObject abilityItemPrefab;
    [SerializeField] private GameObject abilitySlotPrefab;
    public int abilitySlotNum = 12;
    [SerializeField] private GameObject abilityBarPanel;
    [SerializeField] private GameObject abilityWindowPanel;

    [SerializeField] private KeyCode abilityWindowInput = KeyCode.Y;
    [SerializeField] private KeyCode abilityInput = KeyCode.E;

    [SerializeField] private Color selectedColour;
    private Color originalColour;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        cam = FindObjectOfType<ThirdPersonCam>();
        inventory = FindObjectOfType<PlayerInventory>();
        emoteManager = FindObjectOfType<EmoteManager>();
        abilitySlots = new AbilitySlot[abilityBarPanel.transform.childCount + abilitySlotNum];

        //for (int i = 0; i < abilitySlots.Length; i++)
        //{
        //    abilitys.Add(null);
        //}

        foreach (Transform child in abilityWindowPanel.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < abilityBarPanel.transform.childCount + abilitySlotNum; i++)
        {
            if (i < abilityBarPanel.transform.childCount)
            {
                abilitySlots[i] = abilityBarPanel.transform.GetChild(i).GetComponent<AbilitySlot>();
                abilitySlots[i].slot = i;
            }
            else
            {
                //abilitySlots[i] = abilityWindowPanel.transform.GetChild(i - abilityBarPanel.transform.childCount).GetComponent<AbilitySlot>();
                abilitySlots[i] = Instantiate(abilitySlotPrefab, abilityWindowPanel.transform).GetComponent<AbilitySlot>();
                abilitySlots[i].slot = i;

            }
        }
        originalColour = abilitySlots[0].GetComponent<Image>().color;

        for (int i = 0; i < abilities.Count; i++)
        {
            //AddabilityUiToInventory(abilitys[i]);
            //if ()
            SpawnNewAbilityUI(abilities[i], i);
        }

    }

    private void Update()
    {
        if (activeAbility != null && !effectActive && player.stats.power > activeAbility.energyCost)
        {
            if (Input.GetKeyDown(abilityInput))
            {
                activeAbility.ActivateEffect(player);
                effectActive = true;
                player.stats.DecreasePower(activeAbility.energyCost);
                StartCoroutine(activeAbility.DeactivateEffect(player, this));
            }
        }

        if (!effectActive)
            player.stats.RegeneratePower();

        if (abilityUI.activeSelf)
        {
            SelectAbilityWithNumbers();
            SelectAbilityWithScroll();
        }

        if (abilitySlots[selectedAbilitySlot].abilityItem != null && activeAbility != abilitySlots[selectedAbilitySlot].abilityItem.ability)
        {
            activeAbility = abilitySlots[selectedAbilitySlot].abilityItem.ability;
        }

        if (player.ability != activeAbility)
            player.ability = activeAbility;

        if (Input.GetKeyDown(abilityWindowInput))
        {
            if (!abilityUI.activeSelf)
            {
                abilityUI.SetActive(true);
                inventory.inventoryUI.SetActive(false);
                emoteManager.emoteUI.SetActive(false);
                abilityWindowOpen = inventory.inventoryWindowOpen;

            }
            else
            {
                abilityWindowOpen = !abilityWindowOpen;
            }
        }

        if (abilityUI.activeSelf)
        {
            inventory.inventoryUI.SetActive(false);
            emoteManager.emoteUI.SetActive(false);


            if (activeAbility != null)
            {
                SetSelectedAbilityColour();
                abilityTextPrompts.gameObject.SetActive(true);
                abilityTextPrompts.text = activeAbility.itemName;
            }
            else
            {
                abilityTextPrompts.text = null;
                abilityTextPrompts.gameObject.SetActive(false);

                activeAbility = null;
                SetSelectedAbilityColour();
            }
        }
        //else
        //{
        //    //abilityTextPrompts.text = null;
        //    //abilityTextPrompts.gameObject.SetActive(false);
        //    //activeAbility = null;
        //    //inventory.inventoryUI.SetActive(true);

        //    SetSelectedAbilityColour();
        //}


        if (abilityWindowOpen && !abilityWindowPanel.activeSelf)
        {
            abilityWindowPanel.SetActive(true);
            cam.freezeCameraRotation = true;

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else if (!abilityWindowOpen && abilityWindowPanel.activeSelf)
        {
            abilityWindowPanel.SetActive(false);
            cam.freezeCameraRotation = false;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void AddAbilityToInventory(Ability ability, GameObject abilityInWorld)
    {

        if (CheckEmptySlots(abilitySlots) > 0)
        {
            for (int s = 0; s < abilitySlots.Length; s++)
            {
                if (abilitySlots[s].transform.childCount == 0) // for the first empty slot
                {
                    SpawnNewAbilityUI(ability, s);

                    if (abilityInWorld.GetComponent<AbilityInWorld>())
                    {
                        Destroy(abilityInWorld);
                        //firstPersonRaycast.selectedObject = null;
                    }

                    //abilityItem.Initialiseability(ability);

                    abilities.Add(ability);
                    //abilitys[s] = ability;

                    textPopUp.SetAndDisplayPopUp(ability.itemName + " Ability Collected");
                    return;
                }
            }
        }
    }
    void SpawnNewAbilityUI(Ability ability, int abilitySlot)
    {
        GameObject newAbilityUI = Instantiate(abilityItemPrefab, abilitySlots[abilitySlot].transform);
        AbilityUI abilityUIItem = newAbilityUI.GetComponent<AbilityUI>();
        //abilityUIItem.isEquipped = false;
        abilitySlots[abilitySlot].abilityItem = abilityUIItem;

        Button button = newAbilityUI.AddComponent<Button>();
        button.onClick.AddListener(() => SelectAbilityUIAsButton(abilitySlot));

        abilityUIItem.InitialiseAbility(ability, abilitySlot);

        //abilities[abilitySlot] = abilityUIItem;

    }
    public void SwapSlot(int indexA, int indexB)
    {

        // Store ability A info
        AbilityUI abilityItem = abilitySlots[indexA].abilityItem;

        //Swap Slot A Info for slot B
        abilitySlots[indexA].abilityItem = abilitySlots[indexB].abilityItem;

        // Swap slot B info for stored A info
        abilitySlots[indexB].abilityItem = abilityItem;

        //Delete & Spawn UI

        if (abilitySlots[indexA].transform.childCount > 0)
        {
            foreach (Transform child in abilitySlots[indexA].transform)
            {
                Destroy(child.gameObject);
            }
            //SpawnNewabilityUI(inventorySlots[indexB].inventoryItem.ability, indexB);

        }

        if (abilitySlots[indexB].transform.childCount > 0)
        {
            foreach (Transform child in abilitySlots[indexB].transform)
            {
                Destroy(child.gameObject);
            }
            SpawnNewAbilityUI(abilitySlots[indexA].abilityItem.ability, indexA); //a = originally B

        }

        if (abilityItem != null)
            SpawnNewAbilityUI(abilitySlots[indexB].abilityItem.ability, indexB); // originally A


        Debug.Log("Swapped " + indexA + " ability slot with " + indexB + " ability slot");


    }

    //public IEnumerator DelaySettingFalseVariables(float delay)
    //{
    //    yield return new WaitForSeconds(delay);

    //    StartCoroutine(activeAbility.DeactivateEffect(player, effectActive));
    //    //effectActive = false;
    //}

    int CheckEmptySlots(AbilitySlot[] slots)
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

    public void SelectAbilityUIAsButton(int index)
    {
        if (index <= abilitySlots.Length && abilitySlots[index] != null)
        {
            selectedAbilitySlot = index;

            activeAbility = abilitySlots[index].abilityItem.ability;
            Debug.Log("ability set with UI Button");
        }
        //SetSelectedabilityColour();

    }

    private void SelectAbilityWithNumbers()
    {
        if (abilitySlots.Length >= 1)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (abilitySlots.Length >= 1)
                {
                    selectedAbilitySlot = 0;
                    if (abilities[selectedAbilitySlot] != null)
                    {
                        activeAbility = abilities[0];
                    }
                }
            }
        }
        if (abilitySlots.Length >= 2)
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (abilitySlots.Length >= 2)
                {
                    selectedAbilitySlot = 1;
                    if (abilities[selectedAbilitySlot] != null)
                    {
                        activeAbility = abilities[1];
                    }
                }
            }
        }
        if (abilitySlots.Length >= 3)
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (abilitySlots.Length >= 3)
                {
                    selectedAbilitySlot = 2;
                    if (abilities[selectedAbilitySlot] != null)
                    {
                        activeAbility = abilities[2];
                    }
                }
            }
        }
        if (abilitySlots.Length >= 4)
        {
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (abilitySlots.Length >= 4)
                {
                    selectedAbilitySlot = 3;
                    if (abilities[selectedAbilitySlot] != null)
                    {
                        activeAbility = abilities[3];
                    }
                }
            }
        }
        if (abilitySlots.Length >= 5)
        {
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                if (abilitySlots.Length >= 5)
                {
                    selectedAbilitySlot = 4;
                    if (abilities[selectedAbilitySlot] != null)
                    {
                        activeAbility = abilities[4];
                    }
                }
            }
        }
        if (abilitySlots.Length >= 6)
        {
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                if (abilitySlots.Length >= 6)
                {
                    selectedAbilitySlot = 5;
                    if (abilities[selectedAbilitySlot] != null)
                    {
                        activeAbility = abilities[5];
                    }
                }
            }
        }
        if (abilitySlots.Length >= 7)
        {
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                if (abilitySlots.Length >= 7)
                {
                    selectedAbilitySlot = 6;
                    if (abilities[selectedAbilitySlot] != null)
                    {
                        activeAbility = abilities[6];
                    }
                }
            }
        }
        if (abilitySlots.Length >= 8)
        {
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                if (abilitySlots.Length >= 8)
                {
                    selectedAbilitySlot = 7;
                    if (abilities[selectedAbilitySlot] != null)
                    {
                        activeAbility = abilities[7];
                    }
                }
            }
        }
        if (abilitySlots.Length >= 9)
        {
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                if (abilitySlots.Length >= 9)
                {
                    selectedAbilitySlot = 8;

                    if (abilities[selectedAbilitySlot] != null)
                    {
                        activeAbility = abilities[8];
                    }
                }
            }
        }
        if (abilitySlots.Length == 10)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                if (abilitySlots.Length >= 10)
                {
                    selectedAbilitySlot = 9;

                    if (abilities[selectedAbilitySlot] != null)
                    {
                        activeAbility = abilities[9];
                    }
                }
            }
        }
    }

    private void SelectAbilityWithScroll()
    {
        if (Input.mouseScrollDelta != Vector2.zero)
        {
            selectedAbilitySlot += Mathf.RoundToInt(Input.mouseScrollDelta.y);
            //selectedItemSlot += Mathf.Clamp(Mathf.RoundToInt(Input.mouseScrollDelta.y), 1, inventorySlots.Length);

            if (abilityWindowOpen)
            {
                int lastFilledSlot = 0;

                for (int i = 0; i < abilitySlots.Length; i++)
                {
                    if (abilitySlots[i].transform.childCount >= 0)
                        lastFilledSlot = i;
                }

                //Debug.Log("Last filled slot = " + lastFilledSlot);

                if (selectedAbilitySlot > lastFilledSlot)
                {
                    selectedAbilitySlot = 0;
                }
                else if (selectedAbilitySlot < 0)
                {
                    selectedAbilitySlot = lastFilledSlot;
                }
            }
            else
            {
                //only scroll through filled ability slots


                if (selectedAbilitySlot > abilitySlots.Length - abilityWindowPanel.transform.childCount - 1)
                {
                    selectedAbilitySlot = 0;
                }
                else if (selectedAbilitySlot < 0)
                {
                    selectedAbilitySlot = abilitySlots.Length - abilityWindowPanel.transform.childCount - 1;
                }
            }


            if (abilitySlots[selectedAbilitySlot].abilityItem != null)
            {
                activeAbility = abilitySlots[selectedAbilitySlot].abilityItem.ability;
                //SetSelectedAbilityColour();
            }
        }
    }

    private void SetSelectedAbilityColour()
    {
        for (int i = 0; i < abilitySlots.Length; i++)
        {
            if (i == selectedAbilitySlot)
            {
                abilitySlots[i].GetComponent<Image>().color = selectedColour;
            }
            else abilitySlots[i].GetComponent<Image>().color = originalColour;
        }
    }
}
