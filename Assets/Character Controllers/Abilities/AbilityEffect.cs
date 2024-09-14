using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityEffect : MonoBehaviour
{
    public Ability ability;
    private PlayerController player;
    private PlayerAbilities abilities;

    public float abilitySpeed;
    public float maxDistance;

    private void OnEnable()
    {
        player = FindObjectOfType<PlayerController>();
        abilities = FindObjectOfType<PlayerAbilities>();
    }
    private void Update()
    {
        //ability.Move(gameObject, player);

        //if ( Vector3.Distance(transform.position, player.transform.position) > ability.maxDistance);

        Move();

        if (Vector3.Distance(transform.position, player.transform.position) > maxDistance)
        {
            Destroy(gameObject);
            StartCoroutine(ability.DeactivateEffect(player, abilities));
        }
    }

    public void Move()
    {
        Vector3 direction = new Vector3(transform.position.x - player.transform.position.x, 0, transform.position.x - player.transform.position.x).normalized * abilitySpeed;

        transform.position += transform.forward * abilitySpeed  * Time.deltaTime;

    }
}
