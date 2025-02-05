using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    [SerializeField] private float attackDamage = 20f; // 공격력 (퍼센트로 적용됨)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            MonsterUI monsterUI = collision.GetComponent<MonsterUI>();
            if (monsterUI != null)
            {
                // 체력 감소 (현재 체력 비율에서 공격력 퍼센트 감소)
                float damagePercent = attackDamage / 100f;
                monsterUI.value = Mathf.Clamp(monsterUI.value - damagePercent, 0f, 1f);
            }
        }
    }
}
