
using UnityEngine;

public class Deathable : MonoBehaviour
{
    public int maxHealth;
    protected int health;

    private void Start()
    {
        OnStart();
    }

    protected virtual void OnStart()
    {
        health = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (health - amount <= 0)
            OnDie();
    }

    protected virtual void OnDie()
    {
        Destroy(gameObject);
    }
}
