using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvent : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != null && other.gameObject.tag == "Player")
        {
            GameManager.instance.LockBossRoom();
            Destroy(gameObject);
        }
    }
}
