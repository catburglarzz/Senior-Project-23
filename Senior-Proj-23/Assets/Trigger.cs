using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{

    [SerializeField] Collider2D col; 

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Disappear")
        {
            Destroy(col.gameObject);
        }
    }

}
