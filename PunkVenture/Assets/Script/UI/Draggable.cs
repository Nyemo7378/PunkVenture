using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private bool returnToOriginalPosition = false; // �巡�� �� ���� ��ġ�� ���ư��� ����

    private Vector2 pointerOffset; // Ŭ�� ��ġ�� ������Ʈ �߽� ���� ������
    private Vector3 originalPosition; // ���� ��ġ ����
    private RectTransform rectTransform;
    private Canvas canvas;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("�� UI ������Ʈ�� Canvas �ȿ� �־�� �մϴ�!");
        }
    }

    void Start()
    {
        // �ʱ� ��ġ ����
        originalPosition = rectTransform.localPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint);

        pointerOffset = rectTransform.localPosition - (Vector3)localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rectTransform == null || canvas == null)
            return;

        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPointerPosition))
        {
            rectTransform.localPosition = localPointerPosition + pointerOffset;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        Slot targetSlot = null;
        foreach (RaycastResult result in results)
        {
            if(result.gameObject == gameObject)
            {
                continue;
            }

            if(result.gameObject.CompareTag("Slot"))
            {
                targetSlot = result.gameObject.GetComponentInParent<Slot>();
                break;
            }
        }

        // ���� ��ġ�� ���ư��� �ɼ�
        if (returnToOriginalPosition)
        {
            rectTransform.localPosition = originalPosition;
        }

        if (targetSlot != null)
        {
            Slot mySlot = gameObject.GetComponentInParent<Slot>();

            bool bEquip1 = mySlot.m_IsEquipSlot;
            bool bEquip2 = targetSlot.m_IsEquipSlot;

            if(bEquip1 == true && bEquip2 == true)
            {
                mySlot.Swap(targetSlot);
                return;
            }
            if(bEquip1 == bEquip2)
            {
                mySlot.Swap(targetSlot);
                return;
            }

            // ���⼭ ���� �ϳ��� ��񽽷��̶�°� Ȯ��
            Slot EquipSlot;
            Slot GeneralSlot;
            if (bEquip1)
            {
                EquipSlot = mySlot;
                GeneralSlot = targetSlot;
            }
            else 
            { 
                EquipSlot = targetSlot;
                GeneralSlot = mySlot;
            }

            // GeneralSlot�� ���������� ����? Swap����

            if(GeneralSlot.m_item != null)
            {
                if (GeneralSlot.m_item.IsEquipable() == false)
                {
                    return;
                }
            }

            mySlot.Swap(targetSlot);
            return;
        }
    }
}
