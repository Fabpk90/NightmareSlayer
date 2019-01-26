
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    
    [Range(1, 300)]
    public float speed;
    
    private Deathable owner;

    public void Shoot(Vector3 destination, Deathable owner)
    {
        this.owner = owner;

        GetComponent<Rigidbody2D>()
            .AddForce((destination - transform.position).normalized * speed);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var go = other.gameObject;
        var actor = go.GetComponent<Deathable>();
        
        if (actor && actor != owner)
        {
            actor.TakeDamage(damage);
        }
        
        Destroy(gameObject);
    }
}
