using System;
using UnityEngine;
using UnityEngine.Events;

public class Damager : MonoBehaviour
{
    [Serializable] public class DamageableEvent : UnityEvent<Damager, Damageable> { }

    [Serializable] public class NonDamageableEvent : UnityEvent<Damager> { }

    //call that from inside the onDamageableHit or OnNonDamageableHit to get what was hit.
    public Collider2D LastHitCollider => lastHitCollider;

    public int damage = 1;
    public Vector2 offset = new Vector2(1.5f, 1f);
    public Vector2 size = new Vector2(2.5f, 1f);
    [Tooltip("If this is set, the offset x will be changed base on the sprite flipX setting. e.g. Allow to make the damager always forward in the direction of sprite")]
    public bool offsetBasedOnSpriteFacing = true;
    [Tooltip("SpriteRenderer used to read the flipX value used by offset Based OnSprite Facing")]
    public SpriteRenderer spriteRenderer;
    [Tooltip("If disabled, damager ignore trigger when casting for damage")]
    public bool canHitTriggers;
    public bool disableDamageAfterHit = false;
    [Tooltip("If set, the player will be forced to respawn to latest checkpoint in addition to loosing life")]
    public bool forceRespawn = false;
    [Tooltip("If set, an invincible damageable hit will still get the onHit message (but won't loose any life)")]
    public bool ignoreInvincibility = false;
    public LayerMask hittableLayers;
    public DamageableEvent OnDamageableHit;
    public NonDamageableEvent OnNonDamageableHit;

    protected bool SpriteOriginallyFlipped;
    protected bool CanDamage = true;
    protected ContactFilter2D AttackContactFilter;
    protected readonly Collider2D[] AttackOverlapResults = new Collider2D[10];
    protected Transform DamagerTransform;
    protected Collider2D lastHitCollider;

    private void Awake()
    {
        AttackContactFilter.layerMask = hittableLayers;
        AttackContactFilter.useLayerMask = true;
        AttackContactFilter.useTriggers = canHitTriggers;

        if (offsetBasedOnSpriteFacing && spriteRenderer != null)
            SpriteOriginallyFlipped = spriteRenderer.flipX;

        DamagerTransform = transform;
    }

    public void EnableDamage()
    {
        CanDamage = true;
    }

    public void DisableDamage()
    {
        CanDamage = false;
    }

    private void FixedUpdate()
    {
        if (!CanDamage)
            return;

        Vector2 scale = DamagerTransform.lossyScale;

        Vector2 facingOffset = Vector2.Scale(offset, scale);
        
        if (offsetBasedOnSpriteFacing && spriteRenderer != null && spriteRenderer.flipX != SpriteOriginallyFlipped)
            facingOffset = new Vector2(-offset.x * scale.x, offset.y * scale.y);

        Vector2 scaledSize = Vector2.Scale(size, scale);

        Vector2 pointA = (Vector2)DamagerTransform.position + facingOffset - scaledSize * 0.5f;
        Vector2 pointB = pointA + scaledSize;

        int hitCount = Physics2D.OverlapArea(pointA, pointB, AttackContactFilter, AttackOverlapResults);

        for (int i = 0; i < hitCount; i++)
        {
            lastHitCollider = AttackOverlapResults[i];
            Damageable damageable = lastHitCollider.GetComponent<Damageable>();

            if (damageable)
            {
                OnDamageableHit.Invoke(this, damageable);
                damageable.TakeDamage(this, ignoreInvincibility);
                
                if (disableDamageAfterHit)
                    DisableDamage();
            }
            else
            {
                OnNonDamageableHit.Invoke(this);
            }
        }
    }
}
