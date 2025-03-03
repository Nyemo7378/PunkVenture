using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public enum ItemType
{
    Any,
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

    public bool IsStackable()
    {
        if (type == ItemType.Consumable || type == ItemType.Material)
        {
            return true;
        }
        return false;

    }
}

public class Slot : MonoBehaviour
{
    [SerializeField] Image m_iconImage;
    [SerializeField] Text m_countText;
    [SerializeField] ItemType m_allowType = ItemType.Any;
    public Item m_item;

    void Awake()
    {
    }

    public bool Insert(Item item)
    {
        if (item.count <= 0)
        {
            Debug.LogAssertion("������ ������ �ݵ�� 1�̻��̾�� �մϴ�.");
        }

        if (m_allowType != ItemType.Any)
        {
            if (m_allowType != item.type)
            {
                return false;
            }
        }

        if (m_item == null)
        {
            m_iconImage.sprite = item.icon;
            m_iconImage.color = new Color(1, 1, 1, 1);
            m_item = Instantiate(item);

            if (m_item.count > 1)
            {
                m_countText.text = m_item.count.ToString();
            }
            return true;
        }
        // ���Կ� ������ ������ ������ ����

        if (item.name != m_item.name)
        {
            return false;
        }
        // �ٸ� �������̸� ����

        if (item.type != ItemType.Consumable || item.type != ItemType.Material)
        {
            return false;
        }
        // ���� �������϶� ������������ ����

        m_item.count += item.count;
        m_countText.text = m_item.count.ToString();
        return true;
        // ���� �������̰� ������ �ִٸ�
    }

    public bool Swap(Slot other)
    {
        Item temp1 = null;
        Item temp2 = null;

        temp1 = other.m_item;
        temp2 = m_item;

        other.Clear();
        Clear();

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
