using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    [SerializeField] Transform m_target;
    [SerializeField] Rigidbody2D m_rigid;
    [SerializeField] float m_speed = 30.0f;
    [SerializeField] bool m_detected = false;
    float m_coolTime = 3.0f;
    [SerializeField] float m_coolTimeReset = 3.0f;
    int m_dir = 1;

    // Start is called before the first frame update
    void Start()
    {
        m_rigid = GetComponentInParent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_detected)
        {
            FollowPlayer();
        }
        else
        {
            FollowNothing();
        }

        m_rigid.velocity = new Vector2(m_dir * m_speed, m_rigid.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            m_target = collision.transform;
            m_detected = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_detected = false;
        }
    }

    void FollowPlayer()
    {
        float targetDir = m_target.position.x - transform.position.x;
        if(targetDir < 0.0f)
        {
            m_dir = -1;
        }
        else if(targetDir == 0.0f)
        {
            m_dir = 0;
        }
        else
        {
            m_dir = 1;
        }
    }
    void FollowNothing()
    {
        m_coolTime -= Time.deltaTime;
        if (m_coolTime < 0.0f)
        {
            m_coolTime = m_coolTimeReset;
            m_dir = Random.Range(-1, 1);
        }
    }
}
