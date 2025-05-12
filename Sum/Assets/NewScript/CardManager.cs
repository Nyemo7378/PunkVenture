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
    public Vector3 playerCardStartOffset = new Vector3(-1.2f, -0.5f);

    int sortOrder = 32766;
    List<GameObject> tableCards = new List<GameObject>();

    public Transform cardEntryPoint;
    public Transform cardExitPoint;

    private bool canDraw = true; // 🛑 타이머 종료 시 false로 설정

    public void SetDrawEnabled(bool enabled)
    {
        canDraw = enabled;
    }

    void Start()
    {
        cards = new Dictionary<int, List<GameObject>>();
        initPos = new Dictionary<int, Vector3>();
        Score.Instance.AddScore(30);

        for (int i = 1; i < 10; i++)
        {
            GameObject obj = Instantiate(cardPrefab);
            obj.transform.position = new Vector3(i * cardSpacing, 0, 0) + playerCardStartOffset;
            obj.GetComponent<Card>().SetNumber(i);
            initPos[i] = obj.transform.position;
            cards[i] = new List<GameObject>();
            cards[i].Add(obj);
            UpdateColliders(i);
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
        int num = Random.Range(1, 10);

        if (Score.Instance.SubtractScore(drawPrice))
        {
            GameObject obj = Instantiate(cardPrefab);
            obj.GetComponent<Card>().SetNumber(num);
            obj.GetComponent<SpriteRenderer>().sortingOrder = sortOrder--;
            Vector3 offset = new Vector3(0, -0.2f * cards[num].Count, 0);
            obj.transform.position = new Vector3(-10, initPos[num].y, 0);

            cards[num].Add(obj);
            UpdateColliders(num);

            StartCoroutine(FlyInCard(obj, initPos[num] + offset));

            // 🔊 카드 뽑기 효과음
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

        if (tableCards.Contains(card.gameObject))
        {
            tableCards.Remove(card.gameObject);
            cards[num].Add(card.gameObject);
            UpdateColliders(num);

            card.GetComponent<SpriteRenderer>().sortingOrder = sortOrder--;
            int idx = cards[num].Count - 1;
            Vector3 target = initPos[num] + new Vector3(0, -0.2f * idx, 0);
            StartCoroutine(FlyInCard(card.gameObject, target));

            ReorderTableCards();
            CheckAndRemoveCards();
        }
        else
        {
            if (cards[num].Contains(card.gameObject))
            {
                cards[num].Remove(card.gameObject);
                UpdateColliders(num);
            }

            RepositionStack(num);

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
            Vector3 target = initPos[num] + new Vector3(0, -0.2f * i, 0);
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

            // 🔊 점수 효과음 재생
            SEManager.Instance.Play("score");

            foreach (var card in tableCards)
            {
                StartCoroutine(FlyOutCard(card));
            }

            tableCards.Clear();
        }
    }

    void UpdateColliders(int num)
    {
        if (!cards.ContainsKey(num)) return;

        int count = cards[num].Count;

        for (int i = 0; i < count; i++)
        {
            var card = cards[num][i];
            var col = card.GetComponent<Collider2D>();

            // 실제로 맨 위에 있는 카드는 리스트의 0번째 원소
            bool isTopCard = (i == 0);
            col.enabled = isTopCard;

            // Z 정렬도 맞춰주면 좋음
            Vector3 pos = card.transform.position;
            pos.z = isTopCard ? -1f : 0f;
            card.transform.position = pos;
        }
    }

    IEnumerator FlyInCard(GameObject card, Vector3 targetPos)
    {
        float t = 0f;
        Vector3 start = card.transform.position;

        while (t < animationDuration)
        {
            card.transform.position = Vector3.Lerp(start, targetPos, t / animationDuration);
            t += Time.deltaTime;
            yield return null;
        }

        card.transform.position = targetPos;
    }

    IEnumerator FlyOutCard(GameObject card)
    {
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
}
