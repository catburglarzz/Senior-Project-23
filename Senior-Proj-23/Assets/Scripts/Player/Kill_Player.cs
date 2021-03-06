using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kill_Player : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            collision.transform.position = spawnPoint.position;
            playerHealth.instance.ResetHealth();
        }
    }
}
