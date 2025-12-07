using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    Dictionary<int, List<GameObject>> cards;
    Dictionary<int, Vector3> initPos;

    // 맨 위쪽에 추가 (public 변수들 옆에)
    [Header("Current Table Score UI")]
    public Text currentScoreText;   // ← 인스펙터에서 레거시 Text 드래그할 거임

    // CardManager.cs 맨 위에 변수 추가/수정
    private bool isChecking = false;      // ← 기존 있었으면 그대로
    private bool isProcessingCards = false;  // ← 이거 새로 추가 (중요!!!)
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
    private bool canInput = true;
    private int shakingCardsCount = 0;   // ← 이거 추가!!
    int sortOrder = 32766;
    List<GameObject> tableCards = new List<GameObject>();

    public Transform cardEntryPoint;
    public Transform cardExitPoint;

    private bool canDraw = true;

    public void SetInputEnabled(bool enabled)
    {
        canInput = enabled;
    }

    public void SetDrawEnabled(bool enabled)
    {
        canDraw = enabled;
    }

    // 이 함수 하나 추가 (아무데나 붙여도 됨)
    private void UpdateCurrentScoreText()
    {
        if (currentScoreText == null) return;

        int sum = 0;
        foreach (GameObject cardGO in tableCards)
        {
            Card card = cardGO.GetComponent<Card>();
            if (card != null)
                sum += card.GetNumber();
        }

        currentScoreText.text = "table: " + sum;   // ← 여기만 "Current:" → "table:" 로 변경
    }

    void Start()
    {
        UpdateCurrentScoreText();

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
        if (!canInput || isProcessingCards) return;   // ← isProcessingCards 추가!

        if (canDraw && Input.GetKeyDown(drawKey))
            AddCard();

        if (Input.GetKeyDown(checkKey))
            CheckAndRemoveCards();
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
        if (!canInput || card.IsMoving()) return;

        int num = card.cardNumber;

        if (tableCards.Count >= maxCardsInTable && !tableCards.Contains(card.gameObject))
        {
            Debug.Log("카드가 5개 이상입니다. 더 이상 카드를 이동할 수 없습니다.");
            return;
        }

        if (tableCards.Contains(card.gameObject))
        {
            // 테이블 → 스택으로 되돌리기
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
            UpdateCurrentScoreText();   // 추가
        }
        else
        {
            // 스택 → 테이블로 이동
            int curStack = GetCurrentStackKey(card.gameObject);
            if (curStack != -1 && cards[curStack].Contains(card.gameObject))
            {
                card.lastStackKey = curStack;
                cards[curStack].Remove(card.gameObject);
                UpdateColliders(curStack);
                RepositionStack(curStack);
            }

            tableCards.Add(card.gameObject);
            ReorderTableCards();
            UpdateCurrentScoreText();   // 추가
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
        // 1. 이미 처리 중이면 완전 무시
        if (isChecking || isProcessingCards || tableCards.Count == 0) return;

        isChecking = true;
        isProcessingCards = true;   // ← 여기서부터 처리 시작

        int sum = 0;
        Dictionary<int, int> numberCount = new Dictionary<int, int>();

        foreach (var card in tableCards)
        {
            int n = card.GetComponent<Card>().GetNumber();
            sum += n;
            numberCount[n] = numberCount.GetValueOrDefault(n, 0) + 1;
        }

        // 성공
        if (sum % 10 == 0 && sum != 0)
        {
            int basePoints = tableCards.Count * tableCards.Count;
            int dupBonus = 0;
            foreach (var kv in numberCount)
                if (kv.Value > 1) dupBonus += kv.Value;

            int totalPoints = basePoints + dupBonus;
            int timeBonus = sum / 10;

            GameTimer gt = FindObjectOfType<GameTimer>();
            if (gt) gt.AddTime(timeBonus);

            Score.Instance.AddScore(totalPoints);
            SEManager.Instance.Play("score");

            // 카드 날리기 시작
            StartCoroutine(ProcessSuccessCards());
        }
        // 실패
        else if (sum != 0)
        {
            SEManager.Instance.Play("fail");
            ShakeTableCards();

            // 흔들기 끝나면 처리 끝
            StartCoroutine(WaitAndFinishProcessing(0.6f));
        }
        else
        {
            // 빈 테이블이면 바로 끝
            isChecking = false;
            isProcessingCards = false;
        }
    }
    private IEnumerator WaitAndFinishProcessing(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        isChecking = false;
        isProcessingCards = false;
    }
    private IEnumerator ProcessSuccessCards()
    {
        // 실제로 카드 날리기
        foreach (var card in tableCards.ToArray())  // ToArray() 중요! 리스트 변경 중 안전하게
        {
            StartCoroutine(FlyOutCard(card));
        }

        // 애니메이션 끝날 때까지 대기 (FlyOutCard의 animationDuration과 맞춰!)
        yield return new WaitForSeconds(animationDuration + 0.1f);

        tableCards.Clear();
        UpdateCurrentScoreText();

        // 처리 완전 종료
        isChecking = false;
        isProcessingCards = false;
    }
    // 4. ShakeTableCards() 맨 처음에 카운트 초기화
    private void ShakeTableCards()
    {
        shakingCardsCount = tableCards.Count;   // ← 이 줄 추가!!

        foreach (GameObject cardGO in tableCards)
        {
            Card card = cardGO.GetComponent<Card>();
            if (card == null || card.IsMoving())
            {
                shakingCardsCount--;   // 이미 이동 중이면 카운트 빼기
                continue;
            }

            card.SetMoving(true);
            StartCoroutine(ShakeAndRestore(cardGO));
        }

        // 만약 흔들 카드가 하나도 없으면 바로 정렬
        if (shakingCardsCount <= 0)
            ReorderTableCards();
    }

    // 2. 새 코루틴: 흔들기 + 끝난 후 자동 정렬까지!
    private IEnumerator ShakeAndRestore(GameObject cardGO)
    {
        Vector3 originalPos = cardGO.transform.position;
        float duration = 0.5f;
        float intensity = 0.15f;
        float elapsed = 0f;

        // 흔들기
        while (elapsed < duration)
        {
            float x = Mathf.Sin(elapsed * 50f) * intensity;
            cardGO.transform.position = originalPos + new Vector3(x, 0, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 정확히 원위치
        cardGO.transform.position = originalPos;

        // 흔들기 끝 → 클릭 허용
        Card card = cardGO.GetComponent<Card>();
        if (card != null)
            card.SetMoving(false);

        // 핵심: 모든 카드가 흔들기 끝날 때까지 기다렸다가 한 번만 정렬!
        // → 이걸 위해 카운터 사용
        shakingCardsCount--;
        if (shakingCardsCount <= 0)
        {
            ReorderTableCards();   // ← 여기서 딱 한 번만 정렬!!
            shakingCardsCount = 0; // 안전장치
        }
    }

    // 1. ShakeCard 코루틴을 이걸로 완전 교체 (핵심!!!)
    private IEnumerator ShakeCard(GameObject cardGO)
    {
        Card card = cardGO.GetComponent<Card>();
        if (card == null || card.IsMoving()) yield break;  // 이미 이동 중이면 흔들지 말고 패스

        // 흔들리는 동안은 절대 이동 못 하게 강제 플래그
        card.SetMoving(true);

        Vector3 originalPos = cardGO.transform.position;
        float duration = 0.5f;
        float intensity = 0.15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // ← 여기서만 위치 건드림
            float x = Mathf.Sin(elapsed * 50f) * intensity;
            cardGO.transform.position = originalPos + new Vector3(x, 0, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 정확히 원위치 복귀
        cardGO.transform.position = originalPos;

        // 흔들기 끝 → 이동 다시 허용
        card.SetMoving(false);
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