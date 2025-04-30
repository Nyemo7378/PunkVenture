using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject slotPrefab;
    [SerializeField] uint slotCount = 12;
    [SerializeField] List<Slot> SlotList;
    void Awake()
    {
        SlotList = new List<Slot>();
        for (int i = 0; i < slotCount; i++)
        {
            GameObject prefab = Instantiate(slotPrefab, transform);
            SlotList.Add(prefab.GetComponent<Slot>());
            prefab.name = "Slot_" + i.ToString();
        }


    }
    void Update()
    {
    }
}
