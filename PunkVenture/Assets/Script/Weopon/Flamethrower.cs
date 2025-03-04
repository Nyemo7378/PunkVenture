using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : MonoBehaviour
{
    private ParticleSystem m_particle; // 파티클 시스템 참조
    private bool m_isAttack = false;

    void Start()
    {
        // 현재 오브젝트에서 ParticleSystem 컴포넌트 가져오기
        m_particle = GetComponent<ParticleSystem>();
        m_particle.Stop();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            m_isAttack = false;
            if (m_particle.isPlaying)
            {
                m_particle.Stop();
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            m_isAttack = true;
            m_particle.Play();
        }

        // 마우스 방향으로 회전
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(!m_isAttack)
        {
            return;
        }

        if(collision.CompareTag("Monster"))
        {
            var a = collision.GetComponent<IDamageable>();
            a.TakeHit(1);
        }
    }
}