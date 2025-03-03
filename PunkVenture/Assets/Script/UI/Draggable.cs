using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private bool returnToOriginalPosition = false; // 드래그 후 원래 위치로 돌아갈지 여부

    private Vector2 pointerOffset; // 클릭 위치와 오브젝트 중심 간의 오프셋
    private Vector3 originalPosition; // 원래 위치 저장
    private RectTransform rectTransform;
    private Canvas canvas;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("이 UI 오브젝트는 Canvas 안에 있어야 합니다!");
        }
    }

    void Start()
    {
        // 초기 위치 저장
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

        if(targetSlot != null)
        {
            Slot mySlot = gameObject.GetComponentInParent<Slot>();
            mySlot.Swap(targetSlot);
            targetSlot.Swap(mySlot); 
        }

        // 원래 위치로 돌아가기 옵션
        if (returnToOriginalPosition)
        {
            rectTransform.localPosition = originalPosition;
        }
    }
}
