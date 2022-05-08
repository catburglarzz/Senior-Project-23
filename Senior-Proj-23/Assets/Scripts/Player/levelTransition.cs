using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelTransition : MonoBehaviour
{


    [SerializeField] Transform spawnPoint;
    [SerializeField] Transform background1;
    [SerializeField] Transform background1extra;
    [SerializeField] Transform background2;
    [SerializeField] Transform background2extra;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.CompareTag("Player"))
        {

            background1.gameObject.SetActive(false);
            background1extra.gameObject.SetActive(false);
            background2.gameObject.SetActive(true);
            background2extra.gameObject.SetActive(true);
            collision.transform.position = spawnPoint.position;
            MovementScript.instance.grabPower = true;
            
        }
    }
}
