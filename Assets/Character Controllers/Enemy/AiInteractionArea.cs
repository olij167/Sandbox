using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiInteractionArea : MonoBehaviour
{

    private EntityController entityController;

    private void Awake()
    {
        entityController = transform.parent.root.GetComponent<EntityController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (entityController.target != null && entityController.target.interestingObject == other.transform)
        {
            if (other.GetComponent<EntityController>() && other.GetComponent<EntityController>().entityID == entityController.entityID)
            {
                EntityController entity = other.GetComponent<EntityController>();

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
            if (other.gameObject.GetComponent<PlayerController>() || (other.GetComponent<EntityController>() && other.GetComponent<EntityController>().entityID != entityController.entityID))
            {
                for (int i = 0; i < entityController.foodList.Count; i++)
                {
                    if (entityController.foodList[i].foodObject == other.gameObject)
                    {
                        //if the other npc dies, eat it
                        if (other.GetComponent<EntityStats>() && other.GetComponent<EntityStats>().isDead)
                        {
                            GetComponent<EntityStats>().IncreaseHealth(entityController.foodList[i].healthRecovery);
                            Debug.Log(gameObject.name + " has eaten " + other.name + " for " + entityController.foodList[i].healthRecovery + " health recovery");
                        }
                    }
                }
                entityController.animator.SetBool("isAttacking", true);
            }
            //else
            //{
            //    for (int i = 0; i < entityController.foodList.Count; i++)
            //    {
            //        if (entityController.foodList[i].foodObject == other.gameObject)
            //        {
            //            //consume non-living object
            //        }
            //    }
            //}
        }
    }

    public IEnumerator Reproduce(float incubationTime, EntityController mother, EntityController father)
    {
        Debug.Log("Incubation Begun");

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
    }
}
