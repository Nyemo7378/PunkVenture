using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    [SerializeField] private float attackDamage = 20f; // ���ݷ� (�ۼ�Ʈ�� �����)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            MonsterUI monsterUI = collision.GetComponent<MonsterUI>();
            if (monsterUI != null)
            {
                // ü�� ���� (���� ü�� �������� ���ݷ� �ۼ�Ʈ ����)
                float damagePercent = attackDamage / 100f;
                monsterUI.value = Mathf.Clamp(monsterUI.value - damagePercent, 0f, 1f);
            }
        }
    }
}
