using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelTransition : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            collision.transform.position = spawnPoint.position;
            MovementScript.instance.grabPower = true;
        }
    }
}
