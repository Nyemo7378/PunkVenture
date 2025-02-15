using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] GameObject m_monster;
    [SerializeField] float m_coolTime;
    [SerializeField] float m_coolTimeReset = 5.0f;
    [SerializeField] uint m_preSpawnedMonsterCount = 8;
    [SerializeField] bool m_isSpawning = false;
    [SerializeField] List<Transform> m_spawnPositionList;
    int m_lastRenderOrder;
    private List<GameObject> m_monsterPool;

    // Start is called before the first frame update
    void Start()
    {
        m_monsterPool = new List<GameObject>();
        for (int i = 0; i < m_preSpawnedMonsterCount; i++)
        {
            GameObject monster = Instantiate(m_monster, new Vector3(0, 0, 0), Quaternion.identity);
            monster.SetActive(false);
            m_monsterPool.Add(monster);
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
            GameObject monster = GetFreeMonster();
            monster.SetActive(true);
            monster.transform.position = m_spawnPositionList[i].position;
            CMonster cmon = monster.GetComponent<CMonster>();
            cmon.ResetHp();

            MonsterUI ui = monster.GetComponentInChildren<MonsterUI>();
            ui.SetCoverRenderOrder(m_lastRenderOrder + 1);
            ui.SetGaugeRenderOrder(m_lastRenderOrder);
            m_lastRenderOrder += 2;
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

        Debug.LogAssertion("풀에 스폰가능한 몬스터가 없습니다. preSpawnedMonsterCount를 늘려주세요");
        return null;
    }
}
