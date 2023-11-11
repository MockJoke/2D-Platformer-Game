using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody2D rb;
    [SerializeField] 
    private bool destroyWhenOutOfView = true;
    [SerializeField, Tooltip("If -1 never auto destroy, otherwise bullet is destroyed or return to pool when that time is reached")]
    private float timeBeforeAutodestruct = -1.0f;
    [SerializeField]
    private Camera mainCamera;
    private float m_Timer;
    private const float OffScreenError = 0.01f;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = FindObjectOfType<Camera>();
    }

    void FixedUpdate()
    {
        if (destroyWhenOutOfView)
        {
            Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
            bool onScreen = screenPoint is { z: > 0, x: > -OffScreenError } and { x: < 1 + OffScreenError, y: > -OffScreenError and < 1 + OffScreenError };
            
            if (!onScreen)
                Destroy(this.gameObject);
        }

        if (timeBeforeAutodestruct > 0)
        {
            m_Timer += Time.deltaTime;
            if (m_Timer > timeBeforeAutodestruct)
            {
                Destroy(this.gameObject);
            }
        }
    }
    
    public void AfterDamage()
    {
        Destroy(this.gameObject);
    }
}
