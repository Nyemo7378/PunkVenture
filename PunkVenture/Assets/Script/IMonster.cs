using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// �ܺο��� ���� �����ϰ� �ʹ� => ������ IMonster�� ã���ּ���
// �ʿ��� �Լ������ø� �ּ����� �޾��ּ���
public interface IMonster
{ 
    void ApplyDamage(float damage);
    
}