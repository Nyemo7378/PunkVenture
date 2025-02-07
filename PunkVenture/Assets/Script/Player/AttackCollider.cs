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
            IMonster monster = collision.GetComponent<IMonster>();
            if (monster != null)
            {
                monster.ApplyDamage(5);
            }
        }
    }
}
