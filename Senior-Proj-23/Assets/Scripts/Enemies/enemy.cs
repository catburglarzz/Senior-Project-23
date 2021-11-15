using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    public Animator animator;

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
        animator.SetTrigger("Hurt");

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
        this.GetComponent<AIPatrol>().enabled = false;
        Destroy(gameObject, 1);
        this.enabled = false;
    }
}
