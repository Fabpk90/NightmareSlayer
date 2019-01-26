using UnityEngine;

public abstract class Actor : Deathable
{
    public Projectile projectile;
    protected abstract void Shoot();
}
