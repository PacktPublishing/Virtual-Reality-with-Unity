using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : MonoBehaviour
{
    public int health = 100;
    private int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = health;
    }

    void TakeDamage(int damageToDeal)
    {
        currentHealth -= damageToDeal;

        if(currentHealth <= 0)
        {
            Debug.Log("Game Over");

            // Restart level
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
