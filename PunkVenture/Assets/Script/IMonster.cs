using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �ܺο��� ���� �����ϰ� �ʹ� => ������ IMonster�� ã���ּ���
// �ʿ��� �Լ������ø� �ּ����� �޾��ּ���
public interface IMonster
{ 
    void ApplyDamage(float damage);
}