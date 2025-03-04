using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBlink : MonoBehaviour
{
    float m_blinkTime = 0.3f;
    float m_reset = 0.3f;

    void Update()
    {
        m_blinkTime -= Time.deltaTime;
        if (m_blinkTime <= 0.0f)
        {
            m_blinkTime = m_reset;
            gameObject.SetActive(false);
        }
    }
}