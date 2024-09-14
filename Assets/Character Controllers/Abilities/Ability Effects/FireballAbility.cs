using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Fireball")]

public class FireballAbility : Ability
{
    //public ParticleSystem fireballParticles;
    //public ParticleSystem spawnedParticles;
    public GameObject fireball;
    //public GameObject spawnedFireball;
    //public int spawnedNum;
   // public int maxSpawnedNum;

    //public float fireballSpeed;
    public float fireballDamage;

    //public float maxDistance;
    public override void ActivateEffect(PlayerController player, PlayerAbilities abilities)
    {
        //if (spawnedNum < maxSpawnedNum)
        abilities.abilityObject = Instantiate(fireball, new Vector3(player.model.transform.position.x, player.model.transform.position.y + 1f, player.model.transform.position.z), Quaternion.identity);
        abilities.abilityObject.transform.forward = player.model.transform.forward;
        //spawned.GetComponent<AbilityEffect>()
        // spawnedFireball.GetComponent<Rigidbody>().AddForce(spawnedFireball.transform.forward * fireballSpeed);

        //spawnedFireball.Play();

    }



    public override IEnumerator DeactivateEffect(PlayerController player, PlayerAbilities abilities)
    {

        yield return new WaitForSeconds(effectDuration);


        yield return base.DeactivateEffect(player, abilities);
    }
}
