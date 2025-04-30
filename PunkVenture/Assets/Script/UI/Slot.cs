using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Slot : MonoBehaviour
{
    [SerializeField] Image m_iconImage;
    [SerializeField] Text m_countText;
    public Item m_item;
    public bool m_IsEquipSlot = false;

    [SerializeField] Item m_Inititem;

    private void Awake()
    {
        if (m_Inititem == null)
            return;

        Insert(m_Inititem);
    }

    public bool Insert(Item sourceItem)
    {
        if (sourceItem == null)
            return false;

        if (sourceItem.count <= 0)
            Debug.LogAssertion("아이템 갯수는 반드시 1이상이어야 합니다.");

        if (m_item == null)
        {
            m_iconImage.sprite = sourceItem.icon;
            m_iconImage.color = new Color(1, 1, 1, 1);
            m_item = Instantiate(sourceItem);

            if (m_item.count > 1)
            {
                m_countText.text = m_item.count.ToString();
            }
            return true;
        }

        if (sourceItem.name != m_item.name)
            return false;

        if (true == sourceItem.IsEquipable())
            return false;

        m_item.count += sourceItem.count;
        m_countText.text = m_item.count.ToString();
        return true;
    }

    public void Swap(Slot other)
    {
        if(other == this)
        {
            return;
        }

        Item temp1 = null;
        Item temp2 = null;

        if(other.m_item)
        {
            temp1 = Instantiate(other.m_item);
        }

        if(m_item)
        {
            temp2 = Instantiate(m_item);
        }

        Clear();
        other.Clear();

        Insert(temp1);
        other.Insert(temp2);
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
