using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeopon
{
    void Equip();//무기 이미지 인벤토리 슬롯에 장착, 플레이어가 칼들고있게 구현
    void Attack(float level); //안에서 무기애니메이션 재생, 공격 콜라이더 등등 구현
}
