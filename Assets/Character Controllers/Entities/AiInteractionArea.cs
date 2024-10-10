using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiInteractionArea : MonoBehaviour
{

    private EntityController entityController;
    private EntityInfo entityInfo;

    public List<GameObject> objectsInTrigger;

    private EntityStats entityToEat;

    private void Awake()
    {
        entityController = transform.parent.root.GetComponent<EntityController>();
        entityInfo = entityController.entityInfo;
    }

    private void Update()
    {
        if (objectsInTrigger != null && objectsInTrigger.Count > 0)
        {
            //for (int objectsInTrigger[oIT] = 0; objectsInTrigger[oIT] < objectsInTrigger.Count; objectsInTrigger[oIT]++)
            //{

            //}
            //foreach(GameObject objectsInTrigger[oIT] in objectsInTrigger)
            for (int oIT = 0; oIT < objectsInTrigger.Count; oIT++)
            {
                if (objectsInTrigger[oIT] == null)
                {
                    objectsInTrigger.Remove(objectsInTrigger[oIT]);
                    break;
                }


                if (entityController.target != null && entityController.target.interestingObject == objectsInTrigger[oIT].transform)
                {


                    for (int i = 0; i < entityController.pointsOfInterest.Count; i++)
                    {
                        if (objectsInTrigger[oIT].name.Contains(entityController.pointsOfInterest[i].interestingObject.name))
                        {
                            switch (entityController.pointsOfInterest[i].focusType)
                            {
                                case EntityController.Focus.Food:
                                    for (int f = 0; f < entityController.foodList.Count; f++)
                                    {
                                        if (objectsInTrigger[oIT].name.Contains(entityController.foodList[f].foodObject.name))// if you can eat the other object
                                        {
                                            // Debug.Log(entityController.name + " is going after " + objectsInTrigger[oIT].name + " (food)");
                                            if (objectsInTrigger[oIT].GetComponent<EntityStats>()) // if it is another enity
                                            {
                                                entityToEat = objectsInTrigger[oIT].GetComponent<EntityStats>();
                                                entityController.isEating = true;
                                                if (entityToEat.isDead)
                                                {
                                                    EatOtherEntity(objectsInTrigger[oIT], f);
                                                    //GetComponent<EntityStats>().IncreaseHealth(healthRecovery);
                                                    //Debug.Log(entityController.name + " has eaten " + objName + " for " + entityController.foodList[i].healthRecovery + " health recovery");
                                                    //objectsInTrigger.Remove(objectsInTrigger[oIT]);
                                                }
                                            }
                                            else if (!entityController.isEating)
                                            {
                                                //consume non-living object
                                                entityToEat = null;
                                                StartCoroutine(EatInanimateFood(entityController.foodList[f].eatingTime, objectsInTrigger[oIT], f));
                                                entityController.isEating = true;
                                                break;
                                            }
                                            else entityToEat = null;

                                        }
                                    }
                                    break;

                                case EntityController.Focus.Water:

                                    break;

                               
                                case EntityController.Focus.Companion:
                                    if (objectsInTrigger[oIT].GetComponent<EntityController>() && objectsInTrigger[oIT].GetComponent<EntityController>().entityInfo.entityID == entityInfo.entityID) //if they are the same species
                                    {
                                        EntityController entity = objectsInTrigger[oIT].GetComponent<EntityController>();
                                        EntityInfo entInfo = entity.entityInfo;

                                        if (entityController.canReproduce && entity.canReproduce
                                                && entInfo.mother != entityController && entityInfo.father != entity
                                                && !entityInfo.children.Contains(entity) && !entityInfo.siblings.Contains(entity)
                                                && !entInfo.children.Contains(entityController) && !entInfo.siblings.Contains(entityController)) // Make sure they have both reached maturity & arent relatives
                                        {
                                            if (entityInfo.gender == EntityInfo.Gender.female && entInfo.gender == EntityInfo.Gender.male) // female perspective
                                            {

                                                entityController.canReproduce = false;
                                                entity.canReproduce = false;
                                                entityController.isMating = true;
                                                entity.isMating = true;
                                                entityController.stats.sexDrive = 0f;
                                                entity.stats.sexDrive = 0f;
                                                StartCoroutine(Reproduce(entityInfo.reproductionTime, entityController, entity)); // start incubation
                                            }
                                        }
                                        else if (entityInfo.gender == EntityInfo.Gender.male && entInfo.gender == EntityInfo.Gender.female) // male perspective
                                        {
                                            entityController.canReproduce = false;
                                            entity.canReproduce = false;
                                            entityController.isMating = true;
                                            entity.isMating = true;
                                            entityController.stats.sexDrive = 0f;
                                            entity.stats.sexDrive = 0f;

                                            StartCoroutine(ReproductionCooldown(entityInfo.reproductionTime));

                                        }

                                    }
                                    else if (!objectsInTrigger[oIT].GetComponent<EntityController>() || (objectsInTrigger[oIT].GetComponent<EntityController>() && objectsInTrigger[oIT].GetComponent<EntityController>().entityInfo.entityID != entityInfo.entityID)) // otherwise just follow
                                        entityController.followBehind = true;
                                    break;
                                case EntityController.Focus.Attack:
                                    entityController.isAttacking = true;
                                    break;
                                case EntityController.Focus.Avoid:

                                    break;
                            }
                            break;
                        }


                        //if (objectsInTrigger[oIT].GetComponent<PlayerController>() || (objectsInTrigger[oIT].GetComponent<EntityController>() && objectsInTrigger[oIT].GetComponent<EntityController>().entityID != entityController.entityID)) //if it's the player or another creature - attack
                        //{
                        //    entityController.isAttacking = true;
                        //}



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
        //Debug.Log(gameObject.name + " has begun eating " + food.name );

        yield return new WaitForSeconds(eatingTime);

        GetComponentInParent<EntityStats>().IncreaseStatInstant(entityController.stats.currentHealth, entityController.stats.maxHealth.GetValue(), entityController.foodList[foodListIndex].healthRecovery);
        GetComponentInParent<EntityStats>().IncreaseStatInstant(entityController.stats.hunger, entityController.stats.maxHunger.GetValue(), entityController.foodList[foodListIndex].healthRecovery * 2);

        Debug.Log(entityController.name + " has eaten " + entityController.foodList[foodListIndex].foodObject.name + " for " + entityController.foodList[foodListIndex].healthRecovery + " health recovery");


        if (food != null)
        {
            objectsInTrigger.Remove(food);

            Destroy(food);
        }
        else Debug.Log("Food disappeared before eating was finished " );

        entityController.isEating = false;
    }

    public void EatOtherEntity(GameObject entity, int foodListIndex)
    {
        //GetComponentInParent<EntityStats>().IncreaseHealth(entityController.foodList[foodListIndex].healthRecovery);
        GetComponentInParent<EntityStats>().IncreaseStatInstant(entityController.stats.currentHealth, entityController.stats.maxHealth.GetValue(), entityController.foodList[foodListIndex].healthRecovery);
        GetComponentInParent<EntityStats>().IncreaseStatInstant(entityController.stats.hunger, entityController.stats.maxHunger.GetValue(), entityController.foodList[foodListIndex].healthRecovery * 2);
        //GetComponentInParent<EntityStats>().IncreaseStatInstant(entityController.stats.energy, entityController.stats.maxEnergy.GetValue(), entityController.foodList[foodListIndex].healthRecovery / 2);

        if (entity != null)
        {
            Debug.Log(entityController.name + " has eaten " + entity.name + " for " + entityController.foodList[foodListIndex].healthRecovery + " health recovery");

            objectsInTrigger.Remove(entity);
        }
        else
            Debug.Log(entityController.name + " has eaten " + entityController.foodList[foodListIndex].foodObject.name + " for " + entityController.foodList[foodListIndex].healthRecovery + " health recovery");

        entityController.isEating = false;
    }

    public IEnumerator Reproduce(float incubationTime, EntityController mother, EntityController father)
    {
        Debug.Log("Incubation Begun");
        mother.canReproduce = false;

        yield return new WaitForSeconds(incubationTime);

        Debug.Log("Reproduction Complete");

        EntityController baby = Instantiate(entityInfo.childPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z - 1), Quaternion.identity).GetComponent<EntityController>();
        baby.entityInfo.mother = mother;
        baby.entityInfo.father = father;


        if (mother.entityInfo.children == null) mother.entityInfo.children = new List<EntityController>();
        if (father.entityInfo.children == null) father.entityInfo.children = new List<EntityController>();

        mother.entityInfo.children.Add(baby);
        father.entityInfo.children.Add(baby);

        for (int i = 0; i < mother.entityInfo.children.Count; i++)
        {
            if (mother.entityInfo.children[i].entityInfo.siblings == null) mother.entityInfo.children[i].entityInfo.siblings = new List<EntityController>();

            if (mother.entityInfo.children[i] == baby)
            {
                foreach (EntityController child in mother.entityInfo.children)
                    if (!baby.entityInfo.siblings.Contains(child) && baby != child)
                        baby.entityInfo.siblings.Add(child);
            }
            else if (!mother.entityInfo.children[i].entityInfo.siblings.Contains(baby))
                mother.entityInfo.children[i].entityInfo.siblings.Add(baby);
        }

        for (int i = 0; i < father.entityInfo.children.Count; i++)
        {
            if (father.entityInfo.children[i].entityInfo.siblings == null) father.entityInfo.children[i].entityInfo.siblings = new List<EntityController>();

            if (father.entityInfo.children[i] == baby)
            {
                foreach (EntityController child in father.entityInfo.children)
                    if (!baby.entityInfo.siblings.Contains(child) && baby != child)
                        baby.entityInfo.siblings.Add(child);
            }
            else if (!father.entityInfo.children[i].entityInfo.siblings.Contains(baby)) father.entityInfo.children[i].entityInfo.siblings.Add(baby);
        }

        mother.canReproduce = true;
    }

    public IEnumerator ReproductionCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);

        entityController.canReproduce = true;
    }

    private void OnTriggerExit(Collider other)
    {

        if (entityController.target != null && entityController.target.interestingObject == other.transform)
        {
            if (other.gameObject.GetComponent<PlayerController>() || (other.GetComponent<EntityController>() && other.GetComponent<EntityController>().entityInfo.entityID != entityInfo.entityID))
            {
                entityController.isAttacking = true;
            }
        }
        else if (entityController.target == null || entityController.target.focusType != EntityController.Focus.Food)
        {
            entityController.isAttacking = false;

        }

        if (objectsInTrigger.Contains(other.gameObject))
            objectsInTrigger.Remove(other.gameObject);
    }
}
