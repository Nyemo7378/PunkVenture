using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public LayerMask m_enemyLayer;
    public float m_attackRange = 3;
    public float m_chainAttackRange = 3;
    public int m_maxChainCount = 3;

    public Color outLineStart;
    public Color outLineEnd;

    public Color innerLineStart;
    public Color innerLineEnd;
    public float outWidth;
    public float innerWidth;
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
        GameObject chainEffect = new GameObject("ChainEffect");

        LineRenderer outerLine = chainEffect.AddComponent<LineRenderer>();
        outerLine.positionCount = 5;
        outerLine.startWidth = outWidth; 
        outerLine.endWidth = outWidth;
        outerLine.material = new Material(Shader.Find("Sprites/Default"));
        outerLine.startColor = outLineStart;
        outerLine.endColor = outLineEnd;
        outerLine.sortingLayerName = "Effect";
        outerLine.sortingOrder = 100;

        GameObject innerEffect = new GameObject("InnerLine");
        innerEffect.transform.SetParent(chainEffect.transform); 
        LineRenderer innerLine = innerEffect.AddComponent<LineRenderer>();
        innerLine.positionCount = 5;
        innerLine.startWidth = innerWidth;
        innerLine.endWidth = innerWidth;
        innerLine.material = new Material(Shader.Find("Sprites/Default"));
        innerLine.startColor = Color.white;
        innerLine.endColor = Color.white;
        innerLine.sortingLayerName = "Effect";
        innerLine.sortingOrder = 101;

        // 지글지글
        Vector2 direction = (endPos - startPos).normalized;
        float distance = Vector2.Distance(startPos, endPos);
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);

        outerLine.SetPosition(0, startPos);
        innerLine.SetPosition(0, startPos);
        for (int i = 1; i < outerLine.positionCount - 1; i++)
        {
            float t = i / (float)(outerLine.positionCount - 1);
            Vector2 basePos = Vector2.Lerp(startPos, endPos, t);
            Vector2 offset = perpendicular * Random.Range(-0.2f, 0.2f);
            Vector2 pos = basePos + offset;
            outerLine.SetPosition(i, pos);
            innerLine.SetPosition(i, pos);
        }
        outerLine.SetPosition(outerLine.positionCount - 1, endPos);
        innerLine.SetPosition(innerLine.positionCount - 1, endPos);

        Destroy(chainEffect, 0.2f);
    }
}


