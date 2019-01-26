
using UnityEngine;

public class Platform : Deathable
{
    public bool hasToMove;
    
    public GameObject pointA;
    public GameObject pointB;

    private bool goingToA;
    
    private void FixedUpdate()
    {
        if (hasToMove)
        {
            var moving = Vector3.MoveTowards(transform.position
                             , goingToA
                                 ? pointA.transform.position
                                 : pointB.transform.position, 0f) * Time.deltaTime;

            if (moving == transform.position)
                goingToA = !goingToA;
            else
                transform.position = moving;
        }
    }
}