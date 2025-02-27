using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] GameObject m_monsterPrefab;
    [SerializeField] float m_coolTime;
    [SerializeField] float m_coolTimeReset = 5.0f;
    [SerializeField] uint m_maxCount = 8;
    [SerializeField] bool m_isSpawning = false;
    [SerializeField] List<Transform> m_spawnPositionList;
    private int m_lastRenderOrder;
    private List<GameObject> m_monsterPool;

    // Start is called before the first frame update
    void Start()
    {
        m_monsterPool = new List<GameObject>();
        for (int i = 0; i < m_maxCount; i++)
        {
            GameObject monsterObject = Instantiate(m_monsterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            monsterObject.SetActive(false);
            m_monsterPool.Add(monsterObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_isSpawning)
        {
            return;
        }

        m_coolTime -= Time.deltaTime;
        if (m_coolTime > 0.0f)
        {
            return;
        }
        m_coolTime = m_coolTimeReset;

        for (int i = 0; i < m_spawnPositionList.Count; i++)
        {
            GameObject monsterObject = GetFreeMonster();
            if(monsterObject == null)
            {
                continue;
            }

            monsterObject.SetActive(true);
            monsterObject.transform.position = m_spawnPositionList[i].position;

            Monster monsterScript = monsterObject.GetComponent<Monster>();
            monsterScript.ResetHp();

            MonsterMove moveScript = monsterObject.GetComponentInChildren<MonsterMove>();
            moveScript.m_speed += Random.Range(-0.5f, 0.3f);

            MonsterUI uiScript = monsterObject.GetComponentInChildren<MonsterUI>();
            uiScript.SetUIRenderOrder(m_lastRenderOrder);

            m_lastRenderOrder++;
        }
    }

    private GameObject GetFreeMonster()
    {
        for (int i = 0; i < m_monsterPool.Count; i++)
        {
            if (false == m_monsterPool[i].activeSelf)
            {
                return m_monsterPool[i];
            }
        }
        return null;
    }
}
