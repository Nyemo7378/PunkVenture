using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] GameObject m_sparkLinePrefab;
    [SerializeField] LayerMask m_enemyLayer;
    [SerializeField] float m_attackRange = 3;
    [SerializeField] float m_chainAttackRange = 3;
    [SerializeField] int m_maxChainCount = 3;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 attackPoint = transform.position; 
            Collider2D[] monsters = Physics2D.OverlapCircleAll(attackPoint, m_attackRange, m_enemyLayer);
            if(monsters.Length != 0)
            {
                monsters[0].GetComponent<IMonster>().TakeHit(10);
                ChainAttack(monsters[0].transform);
            }
        }
    }
    // ¾ÈÂÊ¼± 
    void ChainAttack(Transform originEnemy)
    {
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(originEnemy.position, m_chainAttackRange, m_enemyLayer);
        int chainCount = 0;
        foreach (Collider2D enemy in nearbyEnemies)
        {
            if (enemy.transform != originEnemy && chainCount < m_maxChainCount)
            {
                enemy.GetComponent<IMonster>().TakeHit(5);
                chainCount++;
                DrawChainEffect(originEnemy.position, enemy.transform.position); 
            }
        }
    }

    void DrawChainEffect(Vector2 startPos, Vector2 endPos)
    {
        GameObject sparkObject = Instantiate(m_sparkLinePrefab, startPos, Quaternion.identity);
        SparkLine sparkScript = sparkObject.GetComponent<SparkLine>();
        sparkScript.SetPositions(startPos, endPos);
        Destroy(sparkObject, 0.2f);
    }
}


