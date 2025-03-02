using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    Consume,
    Weapon,
    Accessory,
    Etc
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string name;
    public string detail;
    public Sprite icon;
    ItemType type;
    private uint count = 1;
    public bool isStackable;

    public uint GetCount()
    {
        return count;
    }
    public void Stack(uint _count)
    {
        count += _count;
    }
}

public class Slot : MonoBehaviour
{
    [SerializeField] Image m_iconImage;
    [SerializeField] Text m_countText;
    public Item m_item;

    void Awake()
    {
    }

    public bool Add(Item item)
    {
        if(m_item == null)
        {
            m_iconImage.sprite = item.icon;
            m_iconImage.color = new Color(1, 1, 1, 1);
            m_item = Instantiate(item);

            return true;
        }

        // 아이템 이미 있다면

        // 다른 아이템이면 리턴
        if(item.name != m_item.name)
        {
            return false;
        }

        // 같은 아이템일때 쌓을수없으면 리턴
        if(m_item.isStackable == false)
        {
            return false;
        }

        // 같은 아이템이고 쌓을수 있다면
        m_item.Stack(1);
        m_countText.text = m_item.GetCount().ToString();

        return true;   
    }

    public void Clear()
    {
        if (m_item == null)
            return;

        m_iconImage.sprite = null;
        m_iconImage.color = new Color(1, 1, 1, 0);
        m_countText.text = "";
        Destroy(m_item);
        m_item = null;
    }
}
