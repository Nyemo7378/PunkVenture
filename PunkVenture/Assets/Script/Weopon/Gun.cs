using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Gun : MonoBehaviour
{
    public SpriteRenderer m_shotgun;
    public GameObject m_bulletPrefab;
    public Transform m_firePoint;  
    public float m_bulletSpeed = 10f;
    public int m_poolSize = 16;
    public IPlayer m_player;
    public float cool = 0.2f;

    private Queue<Gun_Bullet> m_pool;

    void Start()
    {
        m_player = GameObject.Find("Player").GetComponent<IPlayer>();
        if (m_player == null)
        {
            Debug.LogAssertion("플레이어 없음");
        }

        // 오브젝트 풀 초기화
        m_pool = new Queue<Gun_Bullet>();
        for (int i = 0; i < m_poolSize; i++)
        {
            GameObject bulletObj = Instantiate(m_bulletPrefab);
            bulletObj.SetActive(false);

            Gun_Bullet bullet = bulletObj.GetComponent<Gun_Bullet>();

            bullet.m_parentPool = m_pool;
            bullet.m_firepos = m_firePoint;
            m_pool.Enqueue(bullet);
        }
    }

    void Update()
    {
        cool -= Time.deltaTime;
        if (!Input.GetMouseButtonDown(0))
        {
            if(cool <= 0.0f)
            {
                Vector3 currentRotation0 = m_shotgun.transform.eulerAngles;
                currentRotation0.z = 0;
                m_shotgun.transform.eulerAngles = currentRotation0;
                cool = 0.2f;
            }
            return;
        }

        if (m_pool.Count <= 0)
            return;

        Vector3 currentRotation = m_shotgun.transform.eulerAngles;
        currentRotation.z = 25;
        m_shotgun.transform.eulerAngles = currentRotation;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        Vector2 direction = (mouseWorldPos - m_firePoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        Gun_Bullet bullet = m_pool.Dequeue();
        bullet.Shoot(m_firePoint.position, direction, rotation, m_bulletSpeed);
    }
}
