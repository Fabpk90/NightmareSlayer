
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Boss : Actor
{
    public enum EWaveType
    {
        PROJECTILE,
        MOVEMENT
    }

    public List<EWaveType> listWave;
    
    public Player player;
    private Vector3 playerPosition;

    public GameObject left;
    public GameObject right;
    public bool isOnTheLeft;

    [Header("Projectile Wave")] 
    public int nbProjectile;

    public float cooldownBetween;

    [Header("Movement Wave")]
    public float movementDuration;

    [Header("General")]
    [Range(0.1f, 10f)]
    public float cooldownBetweenPhase;

    public int damageWhenCollide;

    private Random random;
    private float lerpIncrement;

    private bool nextWaveIsToBeActivated;

    private int waveIndex;
    
    protected override void OnStart()
    {
        base.OnStart();
        
        random = new Random();

        lerpIncrement = 0;
        nextWaveIsToBeActivated = true;
        waveIndex = 0;
        
        StartCoroutine(WaveManager());
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.gameObject.GetComponent<Player>();

        if (player)
        {
            player.TakeDamage(damageWhenCollide);
        }
    }

    IEnumerator WaveManager()
    {
        while (true)
        {
            if (nextWaveIsToBeActivated)
            {
                nextWaveIsToBeActivated = false;
                switch (listWave[(waveIndex++) % listWave.Count])
                {
                    case EWaveType.PROJECTILE:
                        StartCoroutine(ProjectileWave());                      
                        break;
                
                    case EWaveType.MOVEMENT:
                        StartCoroutine(MovingBoss());
                        break;
                }   
                //TODO: random pick between the phase
            }
            
            yield return  new WaitForSeconds(.5f);
        }
    }

    IEnumerator ProjectileWave()
    {
        for (int i = 0; i < nbProjectile; i++)
        {
            Shoot();
            yield return new WaitForSeconds(cooldownBetween);
        }
        
        yield return  new WaitForSeconds(cooldownBetweenPhase);
        nextWaveIsToBeActivated = true;
    }

    IEnumerator MovingBoss()
    {
        float startX = transform.position.x;
        float endX = isOnTheLeft ? right.transform.position.x : left.transform.position.x;
            
            
        while (lerpIncrement < 1)
        {
            transform.position = 
                (new Vector3(Mathf.Lerp(startX, endX, lerpIncrement), transform.position.y, 0));
            lerpIncrement += Time.deltaTime / movementDuration;
            yield return null;
        }

        isOnTheLeft = !isOnTheLeft;
        lerpIncrement = 0;
        
        yield return  new WaitForSeconds(cooldownBetweenPhase);
        nextWaveIsToBeActivated = true;
    }
}