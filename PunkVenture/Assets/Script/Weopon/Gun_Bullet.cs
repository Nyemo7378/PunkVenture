using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
public class Gun_Bullet : MonoBehaviour
{
    [SerializeField] float m_damage = 5.0f;
    [SerializeField] float m_lifeTimeReset = 3.0f;
    [SerializeField] Rigidbody2D m_rigid;
    [SerializeField] bool m_destroyOnGround = false;
    [SerializeField] TrailRenderer m_trailRenderer;

    public Transform m_firepos;
    float m_lifeTime;
    public Queue<Gun_Bullet> m_parentPool;

    void Start()
    {
        m_lifeTime = m_lifeTimeReset;
    }

    private void Update()
    {
        m_lifeTime -= Time.deltaTime;
        if (m_lifeTime <= 0.0f)
        {
            ReturnToStaff();
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

                ReturnToStaff();
                return;
            }
        }

        if (!m_destroyOnGround)
            return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            ReturnToStaff();
        }
    }

    public void Shoot(Vector2 startPos, Vector2 dir, Quaternion rot, float speed)
    {
        m_trailRenderer.Clear();
        gameObject.SetActive(true);
        transform.SetParent(null, false);
        transform.SetPositionAndRotation(startPos, rot);
        m_rigid.velocity = dir * speed;
    }

    void ReturnToStaff()
    {
        m_trailRenderer.Clear();
        gameObject.SetActive(false);
        m_parentPool.Enqueue(this);
        m_lifeTime = m_lifeTimeReset;
        transform.parent = m_firepos;
        transform.position = m_firepos.position;
    }
}
