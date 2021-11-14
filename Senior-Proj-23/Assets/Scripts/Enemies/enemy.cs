using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    public int maxHealth = 100;
    int currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Play hurt animation

        if(currentHealth <= 0)
        {
            Die();
        }
        // Play die animation
    }
    
    void Die()
    {
        Debug.Log("Enemy died!");

        // Disable the enemy
        GetComponent<Collider2D>().enabled = false;
        //foreach (Collider2D c in this.GetComponents<Collider2D>())
        //{
        //    c.enabled = false;
        //    //Physics2D.IgnoreCollision(CharacterController2D.GetComponent<Collider2D>(), c);
        //}

        this.GetComponent<Animator>().enabled = false; // jerry rigged way to make enemy "appear" dead, due to no dedicated death animation
        this.GetComponent<AIPatrol>().enabled = false;


        this.enabled = false;
    }
}
