using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CMonster : MonoBehaviour, IMonster
{
    [SerializeField] float m_curHp = 100.0f;
    [SerializeField] float m_maxHp = 100.0f;
    [SerializeField] MonsterSpawner m_parentSpawner;
    [SerializeField] MonsterUI m_UI;
 
    void Start()
    {
    
    }
    void Update()
    {
    }

    public void ResetHp()
    {
        m_curHp = 100.0f;
    }

    public void ApplyDamage(float damage, IPlayer player)
    {
        m_curHp -= damage;
        if(m_curHp <= 0.0f)
        {
            player.AddExp(5.0f);
            transform.gameObject.SetActive(false);
        }
        float hpRatio = m_curHp / m_maxHp;
        float result = 1.0f - hpRatio;
        Math.Clamp(result, 0.0f, 1.0f);
        m_UI.UpdateUI(result);
    }
}
