using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ItemType
{
    Weapon,
    Accessory,
    Consumable,
    Material
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string name;
    public string detail;
    public Sprite icon;
    public ItemType type;
    public uint count = 1;

    public bool IsEquipable()
    {
        if (type == ItemType.Consumable || type == ItemType.Material)
        {
            return false;
        }
        return true;
    }
}