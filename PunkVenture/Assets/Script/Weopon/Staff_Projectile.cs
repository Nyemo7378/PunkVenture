using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Staff_Projectile : MonoBehaviour, IDamageable
{
    float m_lifeTime;
    [SerializeField] float m_lifeTimeReset = 3.0f; 
    [SerializeField] Rigidbody2D m_rigidbody2D;
    [SerializeField] bool IsDestroyOnCollision = false;
    Queue<Staff_Projectile> m_parentPool;
    void Start()
    {
        m_lifeTime = m_lifeTimeReset;
    }

    private void Update()
    {
        m_lifeTime -= Time.deltaTime;
        if (m_lifeTime <= 0.0f)
        {
            m_lifeTime = m_lifeTimeReset;
            Death();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsDestroyOnCollision)
            return;

        Death();
    }

    public void SetFire(Vector2 startPos, Vector2 dir, Quaternion rot, float speed)
    {
        transform.SetPositionAndRotation(startPos, rot);
        transform.gameObject.SetActive(true);
        m_rigidbody2D.velocity = dir * speed;
    }

    public void SetParentPool(Queue<Staff_Projectile> pool)
    {
        m_parentPool = pool;
    }

    void Death()
    {
        transform.gameObject.SetActive(false);
        m_parentPool.Enqueue(this);
    }

    public float GetDamage()
    {
        return 5.0f;
    }

}
