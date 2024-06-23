using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class HealthBar : MonoBehaviour
{
    protected float _maxHealth = 100f;
    protected float _currentHealth;
    protected float fillSpeed = 0.5f;
    [SerializeField] protected Image healthBarFill;
    [SerializeField] protected Gradient colorGradient;
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void UpdateHealth(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if (currentHealth <= 0)
        {
            OnDeath();
        }
        UpdateHealthBar();
    }

    protected void UpdateHealthBar()
    {
        float targetFillAmount = currentHealth / maxHealth;
        healthBarFill.DOFillAmount(targetFillAmount, fillSpeed);
        healthBarFill.DOColor(colorGradient.Evaluate(targetFillAmount), fillSpeed);
    }

    protected abstract void OnDeath();
    protected abstract void OnTriggerEnter2D(Collider2D collision);


    //Properties
    public float currentHealth
    {
        get { return _currentHealth; }
        set { _currentHealth = value; }
    }

    public float maxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }
}
