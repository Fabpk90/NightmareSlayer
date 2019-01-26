using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    private int damage;
    private float attackHitboxDuration;
    private Coroutine coroutine;
    
    void Start()
    {
        damage = GetComponentInParent<Player>().attackDamage;
        attackHitboxDuration = GetComponentInParent<Player>().attackHitboxDuration;

        coroutine = StartCoroutine(DisableAfterDuration());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag== "Boss")
        {
            // TODO : Remove health of enemy
            Debug.Log("Boss collision");
            OnDisable();
        }
    }

    private IEnumerator DisableAfterDuration()
    {
        yield return new WaitForSeconds(attackHitboxDuration);
        Debug.Log("MaxDuration");
        OnDisable();
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable");
        gameObject.SetActive(false);
    }
}
