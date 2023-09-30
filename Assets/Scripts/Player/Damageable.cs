using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    [Serializable] public class HealthEvent : UnityEvent<Damageable> { }
    [Serializable] public class DamageEvent : UnityEvent<Damager, Damageable> { }
    [Serializable] public class HealEvent : UnityEvent<int, Damageable> { }

    public int startingHealth = 5;
    public bool invulnerableAfterDamage = true;
    public float invulnerabilityDuration = 3f;
    public bool disableOnDeath = false;
    [Tooltip("An offset from the object position used to set from where the distance to the damager is computed")]
    public Vector2 centreOffset = new Vector2(0f, 1f);
    public HealthEvent OnHealthSet;
    public DamageEvent OnTakeDamage;
    public DamageEvent OnDeath;
    public HealEvent OnGainHealth;

    protected bool isInvulnerable;
    protected float invulnerabilityTimer;
    protected int currentHealth;
    protected Vector2 damageDirection;
    protected bool ResetHealthOnSceneReload;

    public int CurrentHealth => currentHealth;

    private void Start()
    {
        currentHealth = startingHealth;

        OnHealthSet.Invoke(this);

        DisableInvulnerability();
    }

    private void Update()
    {
        if (isInvulnerable)
        {
            invulnerabilityTimer -= Time.deltaTime;

            if (invulnerabilityTimer <= 0f)
            {
                isInvulnerable = false;
            }
        }
    }

    public void EnableInvulnerability(bool ignoreTimer = false)
    {
        isInvulnerable = true;
        //technically don't ignore timer, just set it to an insanely big number. Allow to avoid to add more test & special case.
        invulnerabilityTimer = ignoreTimer ? float.MaxValue : invulnerabilityDuration;
    }

    public void DisableInvulnerability()
    {
        isInvulnerable = false;
    }

    public Vector2 GetDamageDirection()
    {
        return damageDirection;
    }

    public void TakeDamage(Damager damager, bool ignoreInvincible = false)
    {
        if ((isInvulnerable && !ignoreInvincible) || currentHealth <= 0)
            return;

        //we can reach that point if the damager was one that was ignoring invincible state.
        //We still want the callback that we were hit, but not the damage to be removed from health.
        if (!isInvulnerable)
        {
            currentHealth -= damager.damage;
            OnHealthSet.Invoke(this);
        }

        damageDirection = transform.position + (Vector3)centreOffset - damager.transform.position;

        OnTakeDamage.Invoke(damager, this);

        if (currentHealth <= 0)
        {
            OnDeath.Invoke(damager, this);
            ResetHealthOnSceneReload = true;
            EnableInvulnerability();
            if (disableOnDeath) gameObject.SetActive(false);
        }
    }

    public void GainHealth(int amount)
    {
        currentHealth += amount;

        if (currentHealth > startingHealth)
            currentHealth = startingHealth;

        OnHealthSet.Invoke(this);

        OnGainHealth.Invoke(amount, this);
    }

    public void SetHealth(int amount)
    {
        currentHealth = amount;

        if (currentHealth <= 0)
        {
            OnDeath.Invoke(null, this);
            ResetHealthOnSceneReload = true;
            EnableInvulnerability();
            if (disableOnDeath) gameObject.SetActive(false);
        }

        OnHealthSet.Invoke(this);
    }
}
