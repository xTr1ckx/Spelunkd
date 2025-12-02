using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Player Stats")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("References")]
    public Animator animator;
    public PlayerMovement playerMovement;
    public CharacterController2D controller;
    public Collider2D playerCollider;
    public GameObject drillObject;
    public GameObject[] smokeParticles;
    public MonoBehaviour cameraFollowScript;

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        if (playerMovement == null) playerMovement = GetComponent<PlayerMovement>();
        if (controller == null) controller = GetComponent<CharacterController2D>();
        if (playerCollider == null) playerCollider = GetComponent<Collider2D>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        currentHealth -= amount;
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        currentHealth = 0;

        if (animator != null)
            animator.SetBool("isDeadCrushed", true);

        if(playerMovement != null)
            playerMovement.enabled = false;

        if (controller != null)
            controller.enabled = false;

        if (playerCollider != null)
            playerCollider.enabled = false;

        if(drillObject != null)
            drillObject.SetActive(false);

        if(smokeParticles != null)
        {
            foreach (GameObject smoke in smokeParticles)
            {
                if (smoke != null)
                    smoke.SetActive(false);
            }
        }

        if (cameraFollowScript != null)
            cameraFollowScript.enabled = false;

        Debug.Log("Unit down");
    }
}
