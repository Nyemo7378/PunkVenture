using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class WeoponManager : MonoBehaviour, IWeaponManager
{
    Dictionary<string, GameObject> m_weoponList;
    GameObject m_curWeopon = null;
    void Start()
    {
        m_weoponList = new Dictionary<string, GameObject>();
        Transform group = transform.Find("WeoponGroup");
        foreach (Transform child in group)
        {
            GameObject weopon = child.gameObject;
            m_weoponList.Add(weopon.name, weopon);
        }
        EquipWeapon("Gun");
    }

#if DEBUG
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            // 현재 무기의 이름을 가져옴
            string currentWeaponName = "";
            if (m_curWeopon != null)
            {
                currentWeaponName = m_curWeopon.name;
            }

            // 딕셔너리의 모든 키를 배열로 가져옴
            string[] weaponNames = m_weoponList.Keys.ToArray();

            // 현재 무기의 인덱스 찾기
            int currentIndex = 0;
            for (int i = 0; i < weaponNames.Length; i++)
            {
                if (weaponNames[i] == currentWeaponName)
                {
                    currentIndex = i;
                    break;
                }
            }

            // 다음 무기의 인덱스 계산 (끝에 도달하면 0으로)
            int nextIndex = (currentIndex + 1) % weaponNames.Length;

            // 다음 무기로 전환
            EquipWeapon(weaponNames[nextIndex]);
        }
    }
#endif
    public void EquipWeapon(string name)
    {
        if (false == m_weoponList.ContainsKey(name))
            Debug.DebugBreak();

        if(m_curWeopon == m_weoponList[name])
        {
            return;
        }


        if (m_curWeopon != null)
        {
            m_curWeopon.SetActive(false);
        }

        m_curWeopon = m_weoponList[name];
        m_curWeopon.SetActive(true);
    }
}
