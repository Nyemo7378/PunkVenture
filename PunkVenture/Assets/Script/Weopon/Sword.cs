using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] GameObject m_SparkLinePrefab;
    [SerializeField] LayerMask m_enemyLayer;
    [SerializeField] float m_attackRange = 3;
    [SerializeField] float m_chainAttackRange = 3;
    [SerializeField] int m_maxChainCount = 3;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 attackPoint = transform.position; // 공격 중심점 (검의 위치 기준)
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint, m_attackRange, m_enemyLayer);
            if(hitEnemies.Length != 0)
            {
                hitEnemies[0].GetComponent<IMonster>().ApplyDamage(10);
                ChainAttack(hitEnemies[0].transform);
            }
        }
    }
    // 안쪽선 
    void ChainAttack(Transform originEnemy)
    {
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(originEnemy.position, m_chainAttackRange, m_enemyLayer);
        int chainCount = 0;
        foreach (Collider2D enemy in nearbyEnemies)
        {
            if (enemy.transform != originEnemy && chainCount < m_maxChainCount)
            {
                enemy.GetComponent<IMonster>().ApplyDamage(5);
                chainCount++;
                DrawChainEffect(originEnemy.position, enemy.transform.position); 
            }
        }
    }

    void DrawChainEffect(Vector2 startPos, Vector2 endPos)
    {
        GameObject chainEffect = Instantiate(m_SparkLinePrefab, startPos, Quaternion.identity);
        SparkLine outerLine = chainEffect.GetComponent<SparkLine>();
        outerLine.SetPositions(startPos, endPos);
        Destroy(chainEffect, 0.2f);
    }
}


