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
            // TODO : Remove health of enemy
            Debug.Log("Boss collision");
            OnDisable();
        }
    }

    private void OnDisable()
    {
        gameObject.SetActive(false);
    }
}
