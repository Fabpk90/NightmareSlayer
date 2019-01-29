
using System.Collections;
using UnityEngine;

public class Platform : Deathable
{
    public float cooldownRespawn;

    private BoxCollider2D myCollider;
    private SpriteRenderer render;

    protected override void OnStart()
    {
        base.OnStart();

        myCollider = GetComponent<BoxCollider2D>();
        render = GetComponent<SpriteRenderer>();
    }

    protected override void OnDie()
    {
        myCollider.enabled = false;
        render.enabled = false;
        
        StartCoroutine(EnableAfterSeconds());
    }

    IEnumerator EnableAfterSeconds()
    {
        yield return new WaitForSeconds(cooldownRespawn);
        health = maxHealth;
        
        myCollider.enabled = true;
        render.enabled = true;
    }
}