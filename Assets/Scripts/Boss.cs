
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
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

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

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

    [Range(0.05f, 1f)]
    public float probabilityMovementPhase;
    
    private float lerpIncrement;

    private bool nextWaveIsToBeActivated;

    private int waveIndex;
    private Animator _animator;
    protected override void OnStart()
    {
        base.OnStart();

        lerpIncrement = 0;
        nextWaveIsToBeActivated = true;
        waveIndex = 0;
        
        StartCoroutine(WaveManager());
    }
    
    public override void TakeDamage(int amount) 
    {
        base.TakeDamage(amount);
        float nightmareRatio = (float)health / maxHealth;
        GameManager.instance.SetNightmareAmount(nightmareRatio);
    }

    protected override void OnDie()
    {
        enabled = false;
        GetComponent<Animator>().SetBool("Dead", true);
        gameObject.SetActive(false);
        GameManager.instance.OnWin();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void ActivateBoss()
    {
        enabled = true;
        GameManager.instance.GiveControls(true);
    }

    protected override void Shoot()
    {
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
            print("Player hit boss "+damageWhenCollide+" hp");
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

                /*if (Random.Range(0f,1f) < probabilityMovementPhase)
                {
                    nextWaveIsToBeActivated = false;
                    StartCoroutine(MovingBoss());
                }*/
            }
            
            yield return  new WaitForSeconds(.5f);
        }
    }

    IEnumerator ProjectileWave()
    {
        _animator.SetBool("Attack", true);
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < nbProjectile; i++)
        {
            Shoot();
            yield return new WaitForSeconds(cooldownBetween);
        }
        
        yield return  new WaitForSeconds(cooldownBetweenPhase);
        _animator.SetBool("Attack", false);
        nextWaveIsToBeActivated = true;
    }

    IEnumerator MovingBoss()
    {
        float startX = transform.position.x;
        float endX = 0;
        if (isOnTheLeft)
        {
            endX = right.transform.position.x;
        }
        else
        {
            endX = left.transform.position.x;
        }
        
        _animator.SetBool("Dash", true);
       
        while (lerpIncrement < 1)
        {
            transform.position = 
                (new Vector3(Mathf.Lerp(startX, endX, lerpIncrement), transform.position.y, 0));
            lerpIncrement += Time.deltaTime / movementDuration;
            yield return null;
        }

        isOnTheLeft = !isOnTheLeft;
        lerpIncrement = 0;
        
        _animator.SetBool("Dash", false);
        transform.localScale = new Vector3(
            Mathf.Abs(transform.localScale.x) * (isOnTheLeft ? -1 : 1), transform.localScale.y, transform.localScale.z);
        
        yield return  new WaitForSeconds(cooldownBetweenPhase);
        nextWaveIsToBeActivated = true;
        
        
        
    }
}