
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    Dictionary<int, List<GameObject>> cards;
    Dictionary<int, Vector3> initPos;

    public GameObject cardPrefab;
    public float cardSpacing = 1.2f;
    public float animationDuration = 1.0f;
    public Vector3 tablePosition = new Vector3(0, 2, 0);
    public float tableCardSpacingX = 1.5f;
    public KeyCode drawKey = KeyCode.Space;
    public int drawPrice = 2;
    [Header("Card Layout Settings")]
    public Vector3 playerCardStartOffset = new Vector3(-1.2f, -0.5f);
    public float stackCardSpacingY = 0.5f;
    public int maxCardsInTable = 5;

    int sortOrder = 32766;
    List<GameObject> tableCards = new List<GameObject>();

    public Transform cardEntryPoint;
    public Transform cardExitPoint;

    private bool canDraw = true;

    public void SetDrawEnabled(bool enabled)
    {
        canDraw = enabled;
    }

    void Start()
    {
        cards = new Dictionary<int, List<GameObject>>();
        initPos = new Dictionary<int, Vector3>();
        Score.Instance.AddScore(30);

        List<int> nums = new List<int>();
        for (int i = 1; i < 10; i++) nums.Add(i);
        Shuffle(nums);

        for (int i = 0; i < nums.Count; i++)
        {
            int n = nums[i];
            GameObject obj = Instantiate(cardPrefab);
            obj.transform.position = new Vector3(i * cardSpacing, 0, 0) + playerCardStartOffset;
            obj.GetComponent<Card>().SetNumber(n);
            initPos[n] = obj.transform.position;
            cards[n] = new List<GameObject>();
            cards[n].Add(obj);
            UpdateColliders(n);
        }
    }

    void Update()
    {
        if (!canDraw) return;
        if (Input.GetKeyDown(drawKey))
        {
            AddCard();
        }
    }

    void AddCard()
    {
        int cardNum = Random.Range(1, 10); // 숫자는 랜덤
        int targetStack = GetRandomStackKey(); // 들어갈 스택도 랜덤

        if (Score.Instance.SubtractScore(drawPrice))
        {
            GameObject obj = Instantiate(cardPrefab);
            obj.GetComponent<Card>().SetNumber(cardNum);
            obj.GetComponent<SpriteRenderer>().sortingOrder = sortOrder--;

            Vector3 offset = new Vector3(0, -stackCardSpacingY * cards[targetStack].Count, 0);
            obj.transform.position = cardEntryPoint ? cardEntryPoint.position : new Vector3(-10, initPos[targetStack].y, 0);

            cards[targetStack].Add(obj);
            UpdateColliders(targetStack);
            StartCoroutine(FlyInCard(obj, initPos[targetStack] + offset));
            SEManager.Instance.Play("draw");
        }
        else
        {
            Debug.Log("점수가 부족하여 카드를 추가할 수 없습니다.");
        }
    }

    int GetRandomStackKey()
    {
        List<int> keys = new List<int>(cards.Keys);
        int index = Random.Range(0, keys.Count);
        return keys[index];
    }

    
    public void OnCardClicked(Card card)
    {
        int num = card.cardNumber;

        if (tableCards.Count >= maxCardsInTable && !tableCards.Contains(card.gameObject))
        {
            Debug.Log("카드가 5개 이상입니다. 더 이상 카드를 이동할 수 없습니다.");
            return;
        }

        if (tableCards.Contains(card.gameObject))
        {
            int returnStack = card.lastStackKey;
            if (returnStack != -1)
            {
                cards[returnStack].Add(card.gameObject);
                UpdateColliders(returnStack);
                card.GetComponent<SpriteRenderer>().sortingOrder = sortOrder--;
                int idx = cards[returnStack].Count - 1;
                Vector3 target = initPos[returnStack] + new Vector3(0, -stackCardSpacingY * idx, 0);
                StartCoroutine(FlyInCard(card.gameObject, target));
            }

            tableCards.Remove(card.gameObject);
            ReorderTableCards();
            CheckAndRemoveCards();
        }
        else
        {
            int curStack = GetCurrentStackKey(card.gameObject);
            if (curStack != -1 && cards[curStack].Contains(card.gameObject))
            {
                card.lastStackKey = curStack;
                cards[curStack].Remove(card.gameObject);
                UpdateColliders(curStack);
                RepositionStack(curStack);
            }

            int insertIndex = tableCards.Count / 2;
            tableCards.Insert(insertIndex, card.gameObject);
            ReorderTableCards();
            CheckAndRemoveCards();
        }
    }
    void RepositionStack(int num)
    {
        for (int i = 0; i < cards[num].Count; i++)
        {
            GameObject go = cards[num][i];
            Vector3 target = initPos[num] + new Vector3(0, -stackCardSpacingY * i, 0);
            StartCoroutine(FlyInCard(go, target));
        }
    }

    void ReorderTableCards()
    {
        float startX = tablePosition.x - ((tableCards.Count - 1) * tableCardSpacingX / 2f);
        for (int i = 0; i < tableCards.Count; i++)
        {
            Vector3 target = new Vector3(startX + i * tableCardSpacingX, tablePosition.y, tablePosition.z);
            StartCoroutine(FlyInCard(tableCards[i], target));
        }
    }

    void CheckAndRemoveCards()
    {
        int sum = 0;
        foreach (var card in tableCards)
        {
            sum += card.GetComponent<Card>().GetNumber();
        }
        if (sum % 10 == 0 && sum != 0)
        {
            int points = tableCards.Count * tableCards.Count;
            Score.Instance.AddScore(points);
            SEManager.Instance.Play("score");
            foreach (var card in tableCards)
            {
                StartCoroutine(FlyOutCard(card));
            }
            tableCards.Clear();
        }
    }

    int GetCurrentStackKey(GameObject card)
    {
        foreach (var kv in cards)
            if (kv.Value.Contains(card)) return kv.Key;
        return -1;
    }

    void UpdateColliders(int num)
    {
        if (!cards.ContainsKey(num)) return;
        int count = cards[num].Count;
        for (int i = 0; i < count; i++)
        {
            var card = cards[num][i];
            var col = card.GetComponent<Collider2D>();
            col.enabled = true;
            Vector3 pos = card.transform.position;
            pos.z = -i * 0.01f;
            card.transform.position = pos;
        }
    }

    IEnumerator FlyInCard(GameObject card, Vector3 targetPos)
    {
        Card cardComponent = card.GetComponent<Card>();
        if (cardComponent != null) cardComponent.SetMoving(true);
        float t = 0f;
        Vector3 start = card.transform.position;
        while (t < animationDuration)
        {
            card.transform.position = Vector3.Lerp(start, targetPos, t / animationDuration);
            t += Time.deltaTime;
            yield return null;
        }
        card.transform.position = targetPos;
        if (cardComponent != null) cardComponent.SetMoving(false);
    }

    IEnumerator FlyOutCard(GameObject card)
    {
        Card cardComponent = card.GetComponent<Card>();
        if (cardComponent != null) cardComponent.SetMoving(true);
        float t = 0f;
        Vector3 start = card.transform.position;
        Vector3 target = cardExitPoint.position;
        while (t < animationDuration)
        {
            card.transform.position = Vector3.Lerp(start, target, t / animationDuration);
            t += Time.deltaTime;
            yield return null;
        }
        card.transform.position = target;
        Destroy(card);
    }

    void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
