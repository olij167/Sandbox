using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Fireball")]

public class FireballAbility : Ability
{
    public ParticleSystem fireballParticles;
    public ParticleSystem spawnedParticles;
    //public float fireballSpeed;
    //public float fireballDamage;
    public override void ActivateEffect(PlayerController player)
    {
        if (spawnedParticles == null)
            spawnedParticles = Instantiate(fireballParticles, player.transform.position, Quaternion.identity);

        spawnedParticles.Play();
        //Vector3 dir = player.orientation.forward * fireballSpeed * Time.deltaTime;

    }

    public override IEnumerator DeactivateEffect(PlayerController player, PlayerAbilities abilities)
    {
        yield return new WaitUntil(() => spawnedParticles.isStopped);


         Destroy(spawnedParticles.gameObject);
        abilities.effectActive = false;


        yield return null;
    }
}
