using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] Slot m_slot;
    [SerializeField] Slot m_slot2;
    [SerializeField] Slot m_slot3;
    [SerializeField] Item m_item;
    [SerializeField] Item m_item2;
    [SerializeField] Item m_item3;

    void Awake()
    {
       
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            m_slot.Insert(m_item);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            m_slot2.Insert(m_item2);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            m_slot3.Insert(m_item3);
        }
    }
}
