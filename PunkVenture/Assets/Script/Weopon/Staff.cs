using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : MonoBehaviour
{
    public GameObject bulletPrefab; // 총알 프리팹
    public Transform firePoint;     // 총알이 발사될 위치
    public float bulletSpeed = 10f; // 총알 속도
    public int poolSize = 16;       // 오브젝트 풀 크기

    private Queue<Staff_Projectile> bulletPool;

    void Start()
    {
        // 오브젝트 풀 초기화
        bulletPool = new Queue<Staff_Projectile>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bulletObj = Instantiate(bulletPrefab);
            bulletObj.SetActive(false);

            Staff_Projectile bullet = bulletObj.GetComponent<Staff_Projectile>();
            bullet.SetParentPool(bulletPool);
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

            Vector2 direction = (mouseWorldPos - firePoint.position).normalized;

            Staff_Projectile bullet = bulletPool.Dequeue();
            bullet.SetFire(firePoint.position, direction, Quaternion.identity, bulletSpeed);
        }
    }

}
