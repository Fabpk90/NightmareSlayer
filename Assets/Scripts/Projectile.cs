
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    
    public float speed;
    
    private Deathable owner;

    public float rotationRate;

    private void FixedUpdate()
    {
        transform.Rotate(0, 0, rotationRate);
    }

    public void Shoot(Vector3 destination, Deathable owner)
    {
        this.owner = owner;

        GetComponent<Rigidbody2D>()
            .AddForce((destination - transform.position).normalized * speed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var go = other.gameObject;
        var actor = go.GetComponent<Deathable>();
        
        if (actor)
        {
            if (actor != owner)
            {
                var Player = go.GetComponent<Player>();

                if (Player)
                {
                    if (Player.canTakeDamage)
                    {
                        Player.TakeDamage(damage);
                        Destroy(gameObject);
                    }
                        
                }
                else
                {
                    if (actor.canTakeDamage)
                    {
                        actor.TakeDamage(damage);
                        Destroy(gameObject);
                    }
                }
            }
        }
        else if(!go.GetComponent<Projectile>())
            Destroy(gameObject);
    }
}
