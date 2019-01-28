
using UnityEngine;

public class Deathable : MonoBehaviour
{
    public int maxHealth;
    public int health;
    public bool canTakeDamage = true;

    private void Start()
    {
        OnStart();
    }

    protected virtual void OnStart()
    {
        health = maxHealth;
    }

    public virtual void TakeDamage(int amount)
    {
        if (canTakeDamage)
        {
            if (health - amount <= 0)
            {
                health = 0;
                OnDie();
            }
            else
            {
                health -= amount;
            }
        }
    }

    protected virtual void OnDie()
    {
        Destroy(gameObject);
    }
}
