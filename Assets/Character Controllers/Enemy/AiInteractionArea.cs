using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiInteractionArea : MonoBehaviour
{

    private EntityController entityController;

    public List<GameObject> objectsInTrigger;

    private void Awake()
    {
        entityController = transform.parent.root.GetComponent<EntityController>();
    }

    private void Update()
    {
        if (objectsInTrigger != null && objectsInTrigger.Count > 0)
        {
            foreach(GameObject obj in objectsInTrigger)
            //for (int oIT = 0; oIT < objectsInTrigger.Count; oIT++)
            {
                if (obj == null)
                {
                    objectsInTrigger.Remove(obj);
                    break;
                }


                if (entityController.target != null && entityController.target.interestingObject == obj.transform)
                {
                    if (obj.GetComponent<EntityController>() && obj.GetComponent<EntityController>().entityID == entityController.entityID)
                    {
                        EntityController entity = obj.GetComponent<EntityController>();

                        if (entityController.gender == EntityController.Gender.female && entity.gender == EntityController.Gender.male)
                        {
                            if (entityController.canReproduce && entity.canReproduce
                                && entity.mother != entityController && entityController.father != entity
                                && !entityController.children.Contains(entity) && !entityController.siblings.Contains(entity)
                                && !entity.children.Contains(entityController) && !entity.siblings.Contains(entityController)) // Prevent pedophilia and incest 
                            {
                                entityController.canReproduce = false;
                                StartCoroutine(Reproduce(entityController.reproductionTime, entityController, entity));
                            }
                        }

                    }
                    else
                    {
                        if (obj.gameObject.GetComponent<PlayerController>() || (obj.GetComponent<EntityController>() && obj.GetComponent<EntityController>().entityID != entityController.entityID))
                        {
                            entityController.animator.SetBool("isAttacking", true);
                        }
                        else
                        {
                            for (int i = 0; i < entityController.foodList.Count; i++)
                            {
                                if (obj.name.Contains(entityController.foodList[i].foodObject.name))
                                {
                                    if (obj.GetComponent<EntityStats>())
                                    {
                                        if (obj.GetComponent<EntityStats>().isDead)
                                        {
                                            GetComponent<EntityStats>().IncreaseHealth(entityController.foodList[i].healthRecovery);
                                            Debug.Log(gameObject.name + " has eaten " + obj.name + " for " + entityController.foodList[i].healthRecovery + " health recovery");
                                            objectsInTrigger.Remove(obj);
                                        }
                                    }
                                    else if (!entityController.isEating)
                                    {
                                        //consume non-living object

                                        StartCoroutine(EatInanimateFood(entityController.foodList[i].eatingTime, obj, i));
                                        entityController.isEating = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (objectsInTrigger == null) objectsInTrigger = new List<GameObject>();

        for (int i = 0; i < entityController.pointsOfInterest.Count; i++)
        {
            if (other.name.Contains(entityController.pointsOfInterest[i].interestingObject.name))
                if (!objectsInTrigger.Contains(other.gameObject))
                    objectsInTrigger.Add(other.gameObject);
        }
    }

    public IEnumerator EatInanimateFood(float eatingTime, GameObject food, int foodListIndex)
    {
        Debug.Log(gameObject.name + " has begun eating " + food.name );

        yield return new WaitForSeconds(eatingTime);

        GetComponent<EntityStats>().IncreaseHealth(entityController.foodList[foodListIndex].healthRecovery);
        Debug.Log(gameObject.name + " has eaten " + food.name + " for " + entityController.foodList[foodListIndex].healthRecovery + " health recovery");
        
        objectsInTrigger.Remove(food);

        Destroy(food);
        entityController.isEating = false;
    }

    public IEnumerator Reproduce(float incubationTime, EntityController mother, EntityController father)
    {
        Debug.Log("Incubation Begun");
        mother.canReproduce = false;

        yield return new WaitForSeconds(incubationTime);

        Debug.Log("Reproduction Complete");

        EntityController baby = Instantiate(entityController.childPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z - 1), Quaternion.identity).GetComponent<EntityController>();
        baby.mother = mother;
        baby.father = father;


        if (mother.children == null) mother.children = new List<EntityController>();
        if (father.children == null) father.children = new List<EntityController>();

        mother.children.Add(baby);
        father.children.Add(baby);

        for (int i = 0; i < mother.children.Count; i++)
        {
            if (mother.children[i].siblings == null) mother.children[i].siblings = new List<EntityController>();

            if (mother.children[i] == baby)
            {
                foreach (EntityController child in mother.children)
                    if (!baby.siblings.Contains(child) && baby != child)
                        baby.siblings.Add(child);
            }
            else if (!mother.children[i].siblings.Contains(baby))
                mother.children[i].siblings.Add(baby);
        }

        for (int i = 0; i < father.children.Count; i++)
        {
            if (father.children[i].siblings == null) father.children[i].siblings = new List<EntityController>();

            if (father.children[i] == baby)
            {
                foreach (EntityController child in father.children)
                    if (!baby.siblings.Contains(child) && baby != child)
                        baby.siblings.Add(child);
            }
            else if (!father.children[i].siblings.Contains(baby)) father.children[i].siblings.Add(baby);
        }

        mother.canReproduce = true;
    }

    private void OnTriggerExit(Collider other)
    {

        if (entityController.target != null && entityController.target.interestingObject == other.transform)
        {
            if (other.gameObject.GetComponent<PlayerController>() || (other.GetComponent<EntityController>() && other.GetComponent<EntityController>().entityID != entityController.entityID))
            {
                entityController.animator.SetBool("isAttacking", false);
            }
        }

        if (objectsInTrigger.Contains(other.gameObject))
            objectsInTrigger.Remove(other.gameObject);
    }
}
