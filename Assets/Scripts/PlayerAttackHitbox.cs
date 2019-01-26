using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    private int damage;
    
    void Start()
    {
        damage = GetComponentInParent<Player>().attackDamage;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag== "Boss")
        {
            var boss = other.gameObject.GetComponent<Boss>();
            boss.TakeDamage(damage);
            OnDisable();
        }
    }

    private void OnDisable()
    {
        gameObject.SetActive(false);
    }
}
