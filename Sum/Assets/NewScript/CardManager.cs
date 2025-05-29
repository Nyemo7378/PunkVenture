using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    Dictionary<int, List<GameObject>> cards;
    Dictionary<int, Vector3> initPos;

    public float cardSpacing = 1.2f;
    public float animationDuration = 1.0f;
    public Vector3 tablePosition = new Vector3(0, 2, 0);
    public float tableCardSpacingX = 1.5f;

    public KeyCode drawKey = KeyCode.Q;
    public int drawPrice = 2;
    public KeyCode checkKey = KeyCode.Space;

    public Vector3 playerCardStartOffset = new Vector3(-1.2f, -0.5f);
    public float stackCardSpacingY = 0.5f;
    public int maxStackRows = 2;
    public int maxCardsInTable = 5;
    public float timeBonusRate = 1.0f;

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
       // Score.Instance.AddScore(30);

        List<int> nums = new List<int>();
        for (int i = 1; i < 10; i++) nums.Add(i);
        Shuffle(nums);

        for (int i = 0; i < nums.Count; i++)
        {
            int n = nums[i];
            GameObject obj = CardPoolManager.Instance.GetCard();
            obj.GetComponent<Card>().SetNumber(n);
            obj.GetComponent<SpriteRenderer>().sortingOrder = sortOrder--;

            obj.transform.position = new Vector3(i * cardSpacing, 0, 0) + playerCardStartOffset;
            initPos[n] = obj.transform.position;
            cards[n] = new List<GameObject>();
            cards[n].Add(obj);
            UpdateColliders(n);
        }

        for (int i = 0; i < 9; i++)
        {
            int cardNum = Random.Range(1, 10);

            List<int> availableStacks = new List<int>();
            foreach (var kv in cards)
                if (kv.Value.Count < maxStackRows)
                    availableStacks.Add(kv.Key);

            if (availableStacks.Count == 0)
            {
                Debug.Log("모든 스택이 가득 찼습니다. 카드를 뽑을 수 없습니다.");
                return;
            }

            int targetStack = availableStacks[Random.Range(0, availableStacks.Count)];

            GameObject obj = CardPoolManager.Instance.GetCard();
            obj.GetComponent<Card>().SetNumber(cardNum);
            obj.GetComponent<SpriteRenderer>().sortingOrder = sortOrder--;

            Vector3 offset = new Vector3(0, -stackCardSpacingY * cards[targetStack].Count, 0);
            obj.transform.position = cardEntryPoint ? cardEntryPoint.position : new Vector3(-10, initPos[targetStack].y, 0);

            if (cards[targetStack].Count >= maxStackRows)
            {
                CardPoolManager.Instance.ReturnCard(obj);
                Debug.Log("더 이상 해당 스택에 카드를 추가할 수 없습니다.");
                return;
            }
            cards[targetStack].Add(obj);
            UpdateColliders(targetStack);
            obj.transform.position = initPos[targetStack] + offset;
        }
    }

    void Update()
    {
        if (!canDraw) return;

        if (Input.GetKeyDown(drawKey))
        {
            AddCard();
        }
        if (Input.GetKeyDown(checkKey))
        {
            CheckAndRemoveCards();
        }
    }

    public void AddCard()
    {
        int cardNum = Random.Range(1, 10);

        List<int> availableStacks = new List<int>();
        foreach (var kv in cards)
            if (kv.Value.Count < maxStackRows)
                availableStacks.Add(kv.Key);

        if (availableStacks.Count == 0)
        {
            Debug.Log("모든 스택이 가득 찼습니다. 카드를 뽑을 수 없습니다.");
            return;
        }

        int targetStack = availableStacks[Random.Range(0, availableStacks.Count)];

        if (Score.Instance.SubtractScore(drawPrice))
        {
            GameObject obj = CardPoolManager.Instance.GetCard();
            obj.GetComponent<Card>().SetNumber(cardNum);
            obj.GetComponent<SpriteRenderer>().sortingOrder = sortOrder--;

            Vector3 offset = new Vector3(0, -stackCardSpacingY * cards[targetStack].Count, 0);
            obj.transform.position = cardEntryPoint ? cardEntryPoint.position : new Vector3(-10, initPos[targetStack].y, 0);

            if (cards[targetStack].Count >= maxStackRows)
            {
                CardPoolManager.Instance.ReturnCard(obj);
                Debug.Log("더 이상 해당 스택에 카드를 추가할 수 없습니다.");
                return;
            }
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

            int insertIndex = tableCards.Count;
            tableCards.Insert(insertIndex, card.gameObject);
            ReorderTableCards();
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

    public void CheckAndRemoveCards()
    {
        int sum = 0;
        Dictionary<int, int> numberCount = new Dictionary<int, int>();

        foreach (var card in tableCards)
        {
            int cardNum = card.GetComponent<Card>().GetNumber();
            sum += cardNum;

            if (numberCount.ContainsKey(cardNum))
            {
                numberCount[cardNum]++;
            }
            else
            {
                numberCount[cardNum] = 1;
            }
        }

        if (sum % 10 == 0 && sum != 0)
        {
            int basePoints = tableCards.Count * tableCards.Count;

            int duplicateBonusPoints = 0;
            foreach (var kv in numberCount)
            {
                int count = kv.Value;
                if (count > 1)
                {
                    duplicateBonusPoints += count;
                }
            }

            int totalPoints = basePoints + duplicateBonusPoints;

            int timeBonus = (sum / 10);
            GameTimer gameTimer = FindObjectOfType<GameTimer>();
            if (gameTimer != null)
            {
                gameTimer.AddTime(timeBonus);
            }

            Score.Instance.AddScore(totalPoints);

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
        CardPoolManager.Instance.ReturnCard(card);
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