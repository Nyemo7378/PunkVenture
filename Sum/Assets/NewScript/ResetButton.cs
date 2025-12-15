// ResetButton.cs - RESET 버튼에 붙여서 사용 (씬 리로드 없이 완전 리셋)
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetButton : MonoBehaviour
{
    private Button resetBtn;   // 본인 버튼 (자동 연결)

    void Start()
    {
        resetBtn = GetComponent<Button>();
        if (resetBtn != null)
            resetBtn.onClick.AddListener(ResetGame);  // 클릭 시 ResetGame 호출
    }

    public void ResetGame()
    {
        // 1. GameTimer 초기화
        GameTimer gt = FindObjectOfType<GameTimer>();
        if (gt != null)
        {
            gt.StopAllCoroutines();  // 모든 애니메이션/페이드 정지

            gt.timeLeft = gt.timeLimit;
            if (gt.digitalText != null)
                gt.digitalText.text = $"time: {gt.timeLimit:F1}";

            if (gt.timerHand != null)
            {
                gt.timerHand.rotation = Quaternion.Euler(0, 0, 0);
                Image handImg = gt.timerHand.GetComponent<Image>();
                if (handImg != null)
                    handImg.color = new Color(handImg.color.r, handImg.color.g, handImg.color.b, 1f);
            }

            if (gt.tableScoreText != null)
                gt.tableScoreText.color = new Color(gt.tableScoreText.color.r, gt.tableScoreText.color.g, gt.tableScoreText.color.b, 1f);

            if (gt.endTextObject != null)
                gt.endTextObject.SetActive(false);

            if (gt.drawButton != null)
            {
                gt.drawButton.interactable = true;
                BaseButton db2 = gt.drawButton.GetComponent<BaseButton>();
                if (db2 != null) db2.enabled = true;
                Image dImg = gt.drawButton.image;
                Text dTxt = gt.drawButton.GetComponentInChildren<Text>();
                if (dImg != null) dImg.color = new Color(dImg.color.r, dImg.color.g, dImg.color.b, 1f);
                if (dTxt != null) dTxt.color = new Color(dTxt.color.r, dTxt.color.g, dTxt.color.b, 1f);
            }

            if (gt.giveButton != null)
            {
                gt.giveButton.interactable = true;
                BaseButton gb2 = gt.giveButton.GetComponent<BaseButton>();
                if (gb2 != null) gb2.enabled = true;
                Image gImg = gt.giveButton.image;
                Text gTxt = gt.giveButton.GetComponentInChildren<Text>();
                if (gImg != null) gImg.color = new Color(gImg.color.r, gImg.color.g, gImg.color.b, 1f);
                if (gTxt != null) gTxt.color = new Color(gTxt.color.r, gTxt.color.g, gTxt.color.b, 1f);
            }

            // isTimeOver false로 (시간 다시 시작)
            gt.isTimeOver = false;
        }

        // 2. Score 초기화 (최신 Score.cs 기준)
        if (Score.Instance != null)
        {
            Score.Instance.score = 0;
            Score.Instance.UpdateScoreText();
        }

        // 3. CardManager 초기화 (카드 풀에 돌려보내고 재배치)
        CardManager cm = FindObjectOfType<CardManager>();
        if (cm != null)
        {
            // 기존 카드 모두 풀에 반환
            foreach (var kv in cm.cards)
            {
                foreach (GameObject card in kv.Value)
                {
                    CardPoolManager.Instance.ReturnCard(card);
                }
            }
            cm.cards.Clear();

            foreach (GameObject card in cm.tableCards.ToArray())
            {
                CardPoolManager.Instance.ReturnCard(card);
            }
            cm.tableCards.Clear();

            cm.UpdateCurrentScoreText();

            // 초기 카드 재배치 (CardManager.Start() 로직 재사용)
            List<int> nums = new List<int>();
            for (int i = 1; i < 10; i++) nums.Add(i);
            cm.Shuffle(nums);  // Shuffle 함수 호출 (private라서 public으로 바꿔야 함, 아래 팁 참조)

            cm.cards = new Dictionary<int, List<GameObject>>();
            cm.initPos = new Dictionary<int, Vector3>();
            cm.sortOrder = 32766;

            for (int i = 0; i < nums.Count; i++)
            {
                int n = nums[i];
                GameObject obj = CardPoolManager.Instance.GetCard();
                obj.GetComponent<Card>().SetNumber(n);
                obj.GetComponent<SpriteRenderer>().sortingOrder = cm.sortOrder--;

                obj.transform.position = new Vector3(i * cm.cardSpacing, 0, 0) + cm.playerCardStartOffset;
                cm.initPos[n] = obj.transform.position;
                cm.cards[n] = new List<GameObject>();
                cm.cards[n].Add(obj);
                cm.UpdateColliders(n);
            }

            for (int i = 0; i < 9; i++)
            {
                int cardNum = Random.Range(1, 10);

                List<int> availableStacks = new List<int>();
                foreach (var kv in cm.cards)
                    if (kv.Value.Count < cm.maxStackRows)
                        availableStacks.Add(kv.Key);

                if (availableStacks.Count == 0) continue;

                int targetStack = availableStacks[Random.Range(0, availableStacks.Count)];

                GameObject obj = CardPoolManager.Instance.GetCard();
                obj.GetComponent<Card>().SetNumber(cardNum);
                obj.GetComponent<SpriteRenderer>().sortingOrder = cm.sortOrder--;

                Vector3 offset = new Vector3(0, -cm.stackCardSpacingY * cm.cards[targetStack].Count, 0);
                obj.transform.position = cm.cardEntryPoint ? cm.cardEntryPoint.position : new Vector3(-10, cm.initPos[targetStack].y, 0);

                cm.cards[targetStack].Add(obj);
                cm.UpdateColliders(targetStack);
                obj.transform.position = cm.initPos[targetStack] + offset;
            }

            // 입력/드로우 활성화
            cm.SetInputEnabled(true);
            cm.SetDrawEnabled(true);
        }
    }
}