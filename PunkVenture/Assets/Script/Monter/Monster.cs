using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Monster : MonoBehaviour, IMonster
{
    [SerializeField] float m_curHp = 100.0f;
    [SerializeField] float m_maxHp = 100.0f;
    [SerializeField] MonsterSpawner m_parentSpawner;
    [SerializeField] MonsterUI m_UI;
    IPlayer m_player;

    void Start()
    {
        m_player = GameObject.Find("Player").GetComponent<IPlayer>();
    }
    void Update()
    {
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Damageable"))
        {
            IDamageable damageObject = collision.gameObject.GetComponent<IDamageable>();
            ApplyDamage(damageObject.GetDamage());
        }
    }

    public void ResetHp()
    {
        m_curHp = 100.0f;
    }

    public void ApplyDamage(float damage)
    {
        m_curHp -= damage;
        if(m_curHp <= 0.0f)
        {
            m_player.AddExp(5.0f);
            transform.gameObject.SetActive(false);
        }
        float hpRatio = m_curHp / m_maxHp;
        float result = 1.0f - hpRatio;
        Math.Clamp(result, 0.0f, 1.0f);
        m_UI.UpdateUI(result);
    }
}
