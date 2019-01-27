
using System.Collections;
using UnityEngine;

public class Platform : Deathable
{
    public float cooldownRespawn;

    private BoxCollider2D collider;
    private SpriteRenderer render;

    protected override void OnStart()
    {
        base.OnStart();

        collider = GetComponent<BoxCollider2D>();
        render = GetComponent<SpriteRenderer>();
    }

    protected override void OnDie()
    {
        collider.enabled = false;
        render.enabled = false;
        
        StartCoroutine(EnableAfterSeconds());
    }

    IEnumerator EnableAfterSeconds()
    {
        yield return new WaitForSeconds(cooldownRespawn);
        health = maxHealth;
        
        collider.enabled = true;
        render.enabled = true;
    }
}