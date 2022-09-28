using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maximumHealth = 100;
    private int currentHealth = 100;

    private void Start()
    {
        currentHealth = maximumHealth;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"Health is currently at {GetHealthPerentage()}");
        }
    }

    public float GetHealthPerentage()
    { 
        return (float)currentHealth / maximumHealth;
    }
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaximumHealth()
    {
        return maximumHealth;
    }

    public void RemoveHealth(int Amount)
    {
        currentHealth -= Amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maximumHealth);
    }

    public void AddHealth(int Amount)
    {
        currentHealth += Amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maximumHealth);
    }
    public void FullHealth()
    {
        currentHealth = maximumHealth;
    }
}
