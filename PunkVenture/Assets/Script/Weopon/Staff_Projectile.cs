using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
public class Staff_Projectile : MonoBehaviour
{
    [SerializeField] float m_damage = 5.0f;
    [SerializeField] float m_lifeTimeReset = 3.0f; 
    [SerializeField] Rigidbody2D m_rigidbody2D;
    [SerializeField] bool m_destroyOnGround = false;

    public Transform m_firepos;
    float m_lifeTime;
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
        if (collision.gameObject.CompareTag("Monster"))
        {
            IDamageable monster = collision.gameObject.GetComponent<IDamageable>();
            if (monster != null)
            {
                monster.TakeHit(m_damage);

                Death();
                return;
            }
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            if (m_destroyOnGround)
            {
                Death();
            }
        }
    }

    public void SetFire(Vector2 startPos, Vector2 dir, Quaternion rot, float speed)
    {
        transform.SetParent(null, false);
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
        transform.parent = m_firepos;
        transform.position = m_firepos.position;
        m_parentPool.Enqueue(this);
    }
}
