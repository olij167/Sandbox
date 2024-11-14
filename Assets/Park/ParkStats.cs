using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ParkStats;

public class ParkStats : MonoBehaviour
{
    public static ParkStats instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else instance = this;
    }

    public List<GameObject> objectsToTrack;

    public List<TrackedObject> currentTrackedObjects;

    public List<Unlockable> unlockableObjects;

    public List<Unlockable> unlockedObjects;

    private SpawnObjects spawnObjects;

    public TextPopUp popUpText;

    [System.Serializable]
    public class TrackedObject
    {
        [HideInInspector] public string elementName;
        public List<GameObject> objectInstances;
    }

    [System.Serializable]
    public class Unlockable
    {
        public GameObject unlockableObject;
        public bool isUnlocked;
        public List<UnlockRequirements> requirements;
        public List<UnlockGroupRequirements> groupRequirements;
    }

    [System.Serializable]
    public class UnlockRequirements
    {
        public GameObject requiredObject;
        public int requiredAmount;

        public AboveOrBelowValue aboveOrBelow;
        public bool requiredAmountMet;

       
    }

    [System.Serializable]
    public class UnlockGroupRequirements
    {
        public string groupName;
        public List<GameObject> requiredObjectsGroup;
        public int requiredAmount;

        public AboveOrBelowValue aboveOrBelow;
        public bool requiredAmountMet;

        public int cumulativeCount = 0;

    }

    private void Start()
    {
        spawnObjects = GetComponent<SpawnObjects>();

        CheckAllUnlocks();


    }

    public void TrackObject(GameObject trackedObject)
    {
        Debug.Log("tracking " + trackedObject.name);
        for (int i = 0; i < currentTrackedObjects.Count; i++)
        {
            //if (currentTrackedObjects[i].trackedObject == null)
            //{
            //    currentTrackedObjects.RemoveAt(i);

            //    TrackObject(trackedObject);
            //    return;
            //}

            if (currentTrackedObjects[i].objectInstances != null && currentTrackedObjects[i].objectInstances.Count > 0 && currentTrackedObjects[i].objectInstances[0] != null)
            {
                if (currentTrackedObjects[i].objectInstances[0].name == trackedObject.name)
                {
                    //currentTrackedObjects[i].count += 1;
                    currentTrackedObjects[i].objectInstances.Add(trackedObject);
                    CheckRelatedUnlocks(trackedObject);

                    return;
                }
            }
            else
            {
                currentTrackedObjects.RemoveAt(i);

                TrackObject(trackedObject);
                return;
            }
        }

        TrackedObject newObj = new TrackedObject();
        newObj.objectInstances = new List<GameObject>();
        newObj.elementName = trackedObject.name;
        //newObj.count = 1;
        newObj.objectInstances.Add(trackedObject);

        currentTrackedObjects.Add(newObj);

        CheckRelatedUnlocks(trackedObject);
    }
    
    public void StopTrackingObject(GameObject trackedObject)
    {
        for (int i = 0; i < currentTrackedObjects.Count; i++)
        {
            if (currentTrackedObjects[i].objectInstances.Contains(trackedObject))
            {

                currentTrackedObjects[i].objectInstances.Remove(trackedObject);

                if (currentTrackedObjects[i].objectInstances == null || currentTrackedObjects[i].objectInstances.Count <= 0)
                {
                    currentTrackedObjects.RemoveAt(i);
                }

            } 
        }
    }

    public void CheckAllUnlocks()
    {
        for (int i = 0; i < unlockableObjects.Count; i++) // for each unlockable
        {
            if (CheckUnlockRequirements(unlockableObjects[i])) // check if it's requirements have been met
            {
                UnlockObject(unlockableObjects[i]); // if so then unlock :D
            }
        }

        for (int i = 0; i < unlockedObjects.Count; i++)
        {
            if (!CheckUnlockRequirements(unlockedObjects[i]))
            {
                LockObject(unlockedObjects[i]);
            }
        }
    }

    public void CheckRelatedUnlocks(GameObject newTrackedObj) // only check the status of unlockable objects which require the newly tracked object
    {
        Debug.Log("Checking related unlocks for " + newTrackedObj.name);
        for (int i = 0; i < unlockableObjects.Count; i++) // for each unlockable
        {
            for (int u = 0; u < unlockableObjects[i].requirements.Count; u++) // for each of it's requirement
            {
                if (newTrackedObj.name.Contains(unlockableObjects[i].requirements[u].requiredObject.name)) // if the newly tracked object is required
                {
                    if (!unlockedObjects.Contains(unlockableObjects[i]))
                    {
                        if (CheckUnlockRequirement(unlockableObjects[i].requirements[u])) // check if it's requirements have been met
                        {
                            UnlockObject(unlockableObjects[i]); // if so then unlock :D
                        }
                    }
                    else
                    {
                        if (!CheckUnlockRequirement(unlockableObjects[i].requirements[u])) // check if it's requirements have been met
                        {
                            LockObject(unlockableObjects[i]); // if so then unlock :D
                        }
                    }
                }
            }

            for (int u = 0; u < unlockableObjects[i].groupRequirements.Count; u++)
            {
                for (int t = 0; t < unlockableObjects[i].groupRequirements[u].requiredObjectsGroup.Count; t++)
                {

                    if (newTrackedObj.name.Contains(unlockableObjects[i].groupRequirements[u].requiredObjectsGroup[t].name)) // if the newly tracked object is required
                    {
                        if (!unlockedObjects.Contains(unlockableObjects[i]))
                        {
                            if (CheckUnlockGroupRequirements(unlockableObjects[i].groupRequirements[u])) // check if it's requirements have been met
                            {
                                UnlockObject(unlockableObjects[i]); // if so then unlock :D
                            }
                        }
                        else
                        {
                            if (!CheckUnlockGroupRequirements(unlockableObjects[i].groupRequirements[u])) // check if it's requirements have been met
                            {
                                LockObject(unlockableObjects[i]); // if so then unlock :D
                            }
                        }
                    }
                }
            }
        }
    }

    public bool CheckUnlockRequirements(Unlockable unlockable)
    {

        for (int u = 0; u < unlockable.requirements.Count; u++) // for each requirement of the unlockable
        {

            unlockable.requirements[u].requiredAmountMet = CheckUnlockRequirement(unlockable.requirements[u]);

            if (CheckUnlockRequirement(unlockable.requirements[u])) return true;
        }


        for (int u = 0; u < unlockable.groupRequirements.Count; u++)
        {

            unlockable.groupRequirements[u].requiredAmountMet = CheckUnlockGroupRequirements(unlockable.groupRequirements[u]);

            if (CheckUnlockGroupRequirements(unlockable.groupRequirements[u])) return true;

        }

        return false;
    }

    public bool CheckUnlockRequirement(UnlockRequirements requirement)
    {
        for (int i = 0; i < currentTrackedObjects.Count; i++) // for each tracked object in the scene
        {
            if (currentTrackedObjects[i].objectInstances[0].name.Contains(requirement.requiredObject.name)) // check if the object exists
            {
                if (requirement.aboveOrBelow == AboveOrBelowValue.Above)
                {
                    if (requirement.requiredAmount <= currentTrackedObjects[i].objectInstances.Count) // check if there is enough of it
                    {
                        requirement.requiredAmountMet = true;
                    }
                    else
                    {
                        requirement.requiredAmountMet = false;
                        Debug.Log(requirement.requiredObject.name + " requires more " + requirement.requiredObject.name + " to unlock ~ " + "[" + currentTrackedObjects[i].objectInstances.Count + "/" + requirement.requiredAmount + "]");
                        return false; // the requirements arent met
                    }
                }
                else
                {
                    if (requirement.requiredAmount > currentTrackedObjects[i].objectInstances.Count) // check if there is few enough of it
                    {
                        requirement.requiredAmountMet = true;
                    }
                    else
                    {
                        requirement.requiredAmountMet = false;

                        Debug.Log(requirement.requiredObject.name + " requires less " + requirement.requiredObject.name + " to unlock ~ " + "[" + currentTrackedObjects[i].objectInstances.Count + "/" + requirement.requiredAmount + "]");
                        return false; // the requirements arent met
                    }
                }
            }
        }


        if (requirement.aboveOrBelow == AboveOrBelowValue.Below)
        {
            for (int i = 0; i < currentTrackedObjects.Count; i++)
            {
                if (currentTrackedObjects[i].objectInstances[0].name == requirement.requiredObject.name) break;

                if (i == currentTrackedObjects.Count - 1)
                    requirement.requiredAmountMet = true;

            }

        }

        if (!requirement.requiredAmountMet)
        {
            //Debug.Log(unlockable.unlockableObject.name + " cannot be unlocked because " + unlockable.requirements[i].requiredObject.trackedObject.name + " unlock requirements aren't met");
            return false; // the requirements arent met
        }

        return true;
    }

    public bool CheckUnlockGroupRequirements(UnlockGroupRequirements requirement)
    {
        requirement.cumulativeCount = 0;

        for (int i = 0; i < currentTrackedObjects.Count; i++) // for each tracked object in the scene
        {
            for (int t = 0; t < requirement.requiredObjectsGroup.Count; t++) // for each tracked object in the scene
            {
                if (currentTrackedObjects[i].objectInstances[0].name.Contains(requirement.requiredObjectsGroup[t].name)) // check if the object exists
                {
                    requirement.cumulativeCount += currentTrackedObjects[i].objectInstances.Count;
                }
            }
        }

        if (requirement.aboveOrBelow == AboveOrBelowValue.Above)
        {
            if (requirement.requiredAmount <= requirement.cumulativeCount) // check if there is enough of it
            {
                requirement.requiredAmountMet = true;
            }
            else
            {
                requirement.requiredAmountMet = false;
                Debug.Log(requirement.groupName + " requires more objects to unlock ~ " + "[" + requirement.cumulativeCount + "/" + requirement.requiredAmount + "]");
                return false; // the requirements arent met
            }
        }
        else
        {
            if (requirement.requiredAmount > requirement.cumulativeCount) // check if there is few enough of it
            {
                requirement.requiredAmountMet = true;
            }
            else
            {
                requirement.requiredAmountMet = false;

                Debug.Log(requirement.groupName + " requires less objects to unlock ~ " + "[" + requirement.cumulativeCount + "/" + requirement.requiredAmount + "]");
                return false; // the requirements arent met
            }
        }

        return true;
    }

    public void UnlockObject(Unlockable unlockable)
    {
        for (int i = 0; i < unlockedObjects.Count; i++)
        {
            if (unlockedObjects[i].unlockableObject.name == unlockable.unlockableObject.name)
                return; // already unlocked
        }

        if (CheckUnlockRequirements(unlockable))
        {
            unlockedObjects.Add(unlockable); // unlock

            popUpText.SetAndDisplayPopUp(unlockable.unlockableObject.name + " has appeared in the park");

            spawnObjects.prefabs.Add(unlockable.unlockableObject);
            unlockable.isUnlocked = true;
        }
    }

    public void LockObject(Unlockable unlockable)
    {
        if (!CheckUnlockRequirements(unlockable))
        {
            unlockable.isUnlocked = false;

            unlockableObjects.Remove(unlockable);
            spawnObjects.prefabs.Remove(unlockable.unlockableObject);

            popUpText.SetAndDisplayPopUp(unlockable.unlockableObject.name + " has disappeared from the park");

        }


    }

}

[System.Serializable]
public enum AboveOrBelowValue
{
    Above, Below
}
