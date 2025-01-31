using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D m_rigid;
    float m_coolTime = 3.0f;
    float m_reset = 3.0f;
    [SerializeField]
    float m_speed = 30.0f;
    int m_dir = 1;

    // Start is called before the first frame update
    void Start()
    {
        m_rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        m_coolTime -= Time.deltaTime;
        if (m_coolTime < 0.0f)
        {
            m_coolTime = m_reset;
            m_dir = Random.Range(-1, 1);
        }
        m_rigid.velocity = new Vector2(m_dir * m_speed, m_rigid.velocity.y);
    }
}
