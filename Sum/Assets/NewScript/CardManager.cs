using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    Dictionary<int, LinkedList<GameObject>> cards;
    Dictionary<int, Vector3> initPos;

    public GameObject cardPrefab;
    public float cardSpacing = 1.2f;
    public float animationDuration = 1.0f;
    public Vector3 tablePosition = new Vector3(0, 2, 0); // 테이블 위치 기준점
    public float tableCardSpacingX = 1.5f; // 테이블 카드 간격
    public KeyCode drawKey = KeyCode.Space;
    public int drawPrice = 2;
    public Vector3 playerCardStartOffset = new Vector3(-1.2f,-0.5f);

    int sortOrder = 32766;
    List<GameObject> tableCards = new List<GameObject>();

    // 인스펙터에서 지정할 Transform들
    public Transform cardEntryPoint; // 카드가 날아올 위치
    public Transform cardExitPoint; // 카드가 나갈 위치

    void Start()
    {
        cards = new Dictionary<int, LinkedList<GameObject>>();
        initPos = new Dictionary<int, Vector3>();
        Score.Instance.AddScore(30);
        for (int i = 1; i < 10; i++)
        {
            GameObject obj = Instantiate(cardPrefab);
            obj.transform.position = new Vector3(i * cardSpacing, 0, 0) + playerCardStartOffset;
            obj.GetComponent<Card>().SetNumber(i);
            initPos.Add(i, obj.transform.position);
            cards.Add(i, new LinkedList<GameObject>());
            cards[i].AddLast(obj);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(drawKey))
        {
            AddCard();
        }
    }

    void AddCard()
    {
        int randomCardNumber = Random.Range(1, 10);


        if (Score.Instance.SubtractScore(drawPrice))
        {
            GameObject obj = Instantiate(cardPrefab);
            obj.GetComponent<Card>().SetNumber(randomCardNumber);
            obj.GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
            sortOrder--;
            int curCardCount = cards[randomCardNumber].Count;
            obj.transform.position = new Vector3(-10, initPos[randomCardNumber].y + (0.2f * curCardCount), 0);

            cards[randomCardNumber].AddLast(obj);

            StartCoroutine(FlyInCard(obj, initPos[randomCardNumber] + new Vector3(0, -0.2f * curCardCount, 0)));
        }
        else
        {
            // 점수 부족 시 카드 추가 안 함
            Debug.Log("점수가 부족하여 카드를 추가할 수 없습니다.");
        }
    }

    public void OnCardClicked(Card card)
    {
        int number = card.cardNumber;

        if (tableCards.Contains(card.gameObject))
        {
            // 테이블에서 제거
            tableCards.Remove(card.gameObject);

            // 다시 리스트 맨 아래로
            cards[number].AddLast(card.gameObject);
            card.gameObject.GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
            sortOrder--;
            int newIndex = cards[number].Count - 1;
            Vector3 targetPos = initPos[number] + new Vector3(0, -0.2f * newIndex, 0);
            StartCoroutine(FlyInCard(card.gameObject, targetPos));

            // 테이블 카드 재정렬
            ReorderTableCards();

            // 👉 여기 추가
            CheckAndRemoveCards();
        }
        else
        {
            // 리스트에서 제거
            if (cards.ContainsKey(number) && cards[number].Contains(card.gameObject))
            {
                cards[number].Remove(card.gameObject);
            }

            // 리스트 재정렬
            int index = 0;
            foreach (var go in cards[number])
            {
                Vector3 targetPos = initPos[number] + new Vector3(0, -0.2f * index, 0);
                StartCoroutine(FlyInCard(go, targetPos));
                index++;
            }

            // 새 카드 중앙에 삽입
            int insertIndex = tableCards.Count / 2;
            tableCards.Insert(insertIndex, card.gameObject);
            ReorderTableCards();

            // 테이블 카드 합 계산 후 제거
            CheckAndRemoveCards();
        }
    }

    void ReorderTableCards()
    {
        // 카드가 중앙 기준으로 정렬되도록 시작 X 위치 계산
        float startX = tablePosition.x - ((tableCards.Count - 1) * tableCardSpacingX / 2f);

        for (int i = 0; i < tableCards.Count; i++)
        {
            Vector3 targetPos = new Vector3(startX + i * tableCardSpacingX, tablePosition.y, tablePosition.z);
            StartCoroutine(FlyInCard(tableCards[i], targetPos));
        }
    }

    void CheckAndRemoveCards()
    {
        int sum = 0;
        List<GameObject> cardsToRemove = new List<GameObject>();

        // 테이블에 있는 카드들의 숫자 합 계산
        foreach (var card in tableCards)
        {
            sum += card.GetComponent<Card>().GetNumber();
        }

        // 합이 10이면 점수 계산 후 제거
        if (sum == 10)
        {
            int points = tableCards.Count * tableCards.Count; // 카드 개수의 제곱을 점수로

            // 점수 추가
            Score.Instance.AddScore(points);

            // 카드들을 제거
            foreach (var card in tableCards)
            {
                StartCoroutine(FlyOutCard(card)); // 카드 나가는 애니메이션
                cardsToRemove.Add(card);
            }

            // 테이블에서 카드 삭제
            tableCards.Clear();
        }
    }

    IEnumerator FlyInCard(GameObject card, Vector3 targetPos)
    {
        float timeElapsed = 0f;
        Vector3 startingPos = card.transform.position;

        while (timeElapsed < animationDuration)
        {
            card.transform.position = Vector3.Lerp(startingPos, targetPos, timeElapsed / animationDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        card.transform.position = targetPos;
    }

    IEnumerator FlyOutCard(GameObject card)
    {
        float timeElapsed = 0f;
        Vector3 startingPos = card.transform.position;
        Vector3 targetPos = cardExitPoint.position; // 카드 나갈 위치 (인스펙터에서 지정)

        while (timeElapsed < animationDuration)
        {
            card.transform.position = Vector3.Lerp(startingPos, targetPos, timeElapsed / animationDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        card.transform.position = targetPos;
        Destroy(card); // 카드 삭제
    }
}
