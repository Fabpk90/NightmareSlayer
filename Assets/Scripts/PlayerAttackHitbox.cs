using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    private int damage;
    private float duration;
    
    // Start is called before the first frame update
    void Start()
    {
        damage = GetComponentInParent<Player>().attackDamage;
        StartCoroutine(SelfDestruct());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag== "Enemy")
        {
            // TODO : Remove health of enemy
            Destroy(gameObject);
        }
    }

    private IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
