
using System.Collections;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Boss : Actor
{
    public Player player;
    private Vector3 playerPosition;

    [Header("Projectile Wave")] 
    public int nbProjectile;

    public float cooldownBetween;
    
    protected override void OnStart()
    {
        base.OnStart();
        StartCoroutine(ProjectileWave());
    }

    protected override void OnDie()
    {
        print("The player has defeated the boss");
    }

    protected override void Shoot()
    {
        print("The boss is shooting");

        if (player != null)
        {
            var position = player.transform.position;
            
            playerPosition = position;
            Instantiate(projectile, transform.position, Quaternion.identity)
                .GetComponent<Projectile>()
                .Shoot(position, this);
        }
        else
        {
            Instantiate(projectile, transform.position, Quaternion.identity)
                .GetComponent<Projectile>()
                .Shoot(playerPosition, this);
        }

        
    }

    IEnumerator ProjectileWave()
    {
        for (int i = 0; i < nbProjectile; i++)
        {
            Shoot();
            yield return new WaitForSeconds(cooldownBetween);
        }
    }
}