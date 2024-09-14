using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Flamethrower")]

public class FlameThrowerWeapon : Ability
{
    public ParticleSystem flamethrowerParticles;
    public ParticleSystem spawnedParticles;
    //public float fireballSpeed;
    //public float fireballDamage;
    public override void ActivateEffect(PlayerController player, PlayerAbilities abilities)
    {
        if (spawnedParticles == null)
            spawnedParticles = Instantiate(flamethrowerParticles, player.orientation.transform.position, Quaternion.identity);

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
