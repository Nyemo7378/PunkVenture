using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHitEffect : MonoBehaviour
{
    float m_activeTime = 0.3f;
    float m_reset = 0.3f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        m_activeTime -= Time.deltaTime;
        if (m_activeTime <= 0.0f)
        {
            m_activeTime = m_reset;
            gameObject.SetActive(false);
        }
    }
}
