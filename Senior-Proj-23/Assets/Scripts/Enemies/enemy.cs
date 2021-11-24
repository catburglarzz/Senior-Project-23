using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    public Animator animator;

    public float maxHealth = 100f;
    private float attackDamage = 10f;
    [SerializeField] private float attackRate = .5f;
    private float canAttack;
    float currentHealth;
    [SerializeField] Transform playerSpawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    private void OnCollisionStay2D(Collision2D other){
        
        if(other.gameObject.tag == "Player"){
            if(other.gameObject.GetComponent<playerHealth>().health <= 0f){
                other.transform.position = playerSpawnPoint.position;
                other.gameObject.GetComponent<playerHealth>().ResetHealth();
            }
            if(attackRate <= canAttack){
                other.gameObject.GetComponent<playerHealth>().UpdateHealth(-attackDamage);
                canAttack = 0f;
            }
            else{
                canAttack += Time.deltaTime;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Play hurt animation
        animator.SetTrigger("Hurt");

        if(currentHealth <= 0f)
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
