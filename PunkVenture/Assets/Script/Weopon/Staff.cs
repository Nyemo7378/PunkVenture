using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : MonoBehaviour
{
    public GameObject m_bulletPrefab;
    public Transform m_firePoint;  
    public float m_bulletSpeed = 10f;
    public int m_poolSize = 16;       

    private Queue<Staff_Projectile> bulletPool;

    void Start()
    {
        // 오브젝트 풀 초기화
        bulletPool = new Queue<Staff_Projectile>();
        for (int i = 0; i < m_poolSize; i++)
        {
            GameObject bulletObj = Instantiate(m_bulletPrefab);
            bulletObj.SetActive(false);

            Staff_Projectile bullet = bulletObj.GetComponent<Staff_Projectile>();
       
            bullet.SetParentPool(bulletPool);
            bullet.m_firepos = m_firePoint;
            bulletPool.Enqueue(bullet);
        }
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (bulletPool.Count > 0)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;

            Vector2 direction = (mouseWorldPos - m_firePoint.position).normalized;

            Staff_Projectile bullet = bulletPool.Dequeue();
            bullet.SetFire(m_firePoint.position, direction, Quaternion.identity, m_bulletSpeed);
        }
    }

}
