using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterUI : MonoBehaviour
{
    public RectTransform m_mask;
    public float value = 1.0f;
    private Vector2 m_initSize;

    void Start()
    {
        m_initSize = m_mask.sizeDelta;
    }

    void Update()
    {
        FixHpBar(value);
    }

    void FixHpBar(float curHpRate)
    {
        curHpRate = Mathf.Clamp(curHpRate, 0.0f, 1.0f);
        m_mask.sizeDelta = new Vector2(m_initSize.x * curHpRate, m_initSize.y);
    }
}
