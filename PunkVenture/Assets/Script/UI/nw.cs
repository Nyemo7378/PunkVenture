using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nw : MonoBehaviour
{
    [SerializeField] Slot m_ws;
    [SerializeField] IWeaponManager m_wm;
    // Start is called before the first frame update
    void Start()
    {
        m_wm = GameObject.Find("Player").GetComponent<IWeaponManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_ws.m_item != null)
        {
            m_wm.EquipWeapon(m_ws.m_item.name);
        }
    }
}
