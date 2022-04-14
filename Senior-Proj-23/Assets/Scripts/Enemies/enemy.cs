using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    public Animator animator;
    public MovementScript move;
    public Rigidbody2D rb;

    public float maxHealth = 100f;
    private float attackDamage = 10f;
    [SerializeField] private float attackRate = .5f;
    private float canAttack;
    [SerializeField] private float knockbackForce = 2f;
    public float currentHealth;
    [SerializeField] Transform playerSpawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        move = GetComponent<MovementScript>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionStay2D(Collision2D other){
        
        if(other.gameObject.tag == "Player"){
            if(other.gameObject.GetComponent<playerHealth>().health <= 0f){
                other.transform.position = playerSpawnPoint.position;
                other.gameObject.GetComponent<playerHealth>().ResetHealth();
            }
            if(attackRate <= canAttack){
                other.gameObject.GetComponent<MovementScript>().UpdateKBDistance(knockbackForce);
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

        //Play death animation
        animator.SetBool("isDead", true);

        // Disable the enemy
        this.GetComponent<AIPatrol>().moveSpeed = 0f;
        rb.velocity = new Vector2(0, 0);
        this.GetComponent<Collider2D>().enabled = false;
        this.GetComponent<AIPatrol>().enabled = false;
        Destroy(gameObject, 1);
        this.enabled = false;
    }
}
