
using System.Collections;
using UnityEngine;

public class Platform : Deathable
{
    public float cooldownRespawn;

    protected override void OnDie()
    {
        gameObject.SetActive(false);
        StartCoroutine(EnableAfterSeconds());
    }

    IEnumerator EnableAfterSeconds()
    {
        yield return new WaitForSeconds(cooldownRespawn);
        health = maxHealth;
        gameObject.SetActive(true);
    }
}