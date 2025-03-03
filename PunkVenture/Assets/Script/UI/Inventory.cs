using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] Slot m_slot;
    [SerializeField] Item m_item;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            m_slot.Insert(m_item);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            m_slot.Clear();
        }
    }
}
