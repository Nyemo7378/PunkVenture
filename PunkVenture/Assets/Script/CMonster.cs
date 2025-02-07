using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMonster : MonoBehaviour, IMonster
{
    [SerializeField] float m_curHp = 100.0f;
    [SerializeField] float m_maxHp = 100.0f;
    [SerializeField] Material m_hpBarMaterial;
    [SerializeField] MonsterSpawner m_parentSpawner;
    void Start()
    {
    }
    void Update()
    {
    }
    public void ApplyDamage(float damage)
    {
        m_curHp -= damage;
        if(m_curHp <= 0.0f)
        {
            m_curHp = 0.0f;
            transform.gameObject.SetActive(false);
        }
        float hpRatio = m_curHp / m_maxHp;
        float result = 1.0f - hpRatio;
        Math.Clamp(result, 0.0f, 1.0f);
        UpdateUI(result);
    }

    private void UpdateUI(float cutOffValue)
    {
        m_hpBarMaterial.SetFloat("_CutOff", cutOffValue);
    }
}
