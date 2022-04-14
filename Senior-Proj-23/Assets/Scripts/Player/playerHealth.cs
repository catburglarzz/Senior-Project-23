using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerHealth : MonoBehaviour
{
    public static playerHealth instance;
    public float health = 0f;
    private float maxHealth = 100f;

    // Start is called before the first frame update
    private void Start()
    {
        health = maxHealth;
        if(instance == null){
            instance = this;
        }
    }

    public void UpdateHealth(float mod)
    {
        health += mod;

        if (health > maxHealth){
            health = maxHealth;
        }
        else if(health <= 0){
            health = 0; 
            Debug.Log("Player Respawn");
        }
    }


    public void ResetHealth(){
        health = maxHealth;
    }

}
