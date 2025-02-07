using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 외부에서 몬스터 참조하고 싶다 => 무조건 IMonster로 찾아주세요
// 필요한 함수있으시면 주석으로 달아주세요
public interface IMonster
{ 
    void ApplyDamage(float damage);
}