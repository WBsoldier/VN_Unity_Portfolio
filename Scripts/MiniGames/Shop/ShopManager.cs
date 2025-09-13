using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Naninovel.Commands;
using Naninovel;

/// <summary>
/// 상점 퍼즐 전체 진행 관리.
/// - 예산/합계/구매 상태 관리
/// - 영수증 UI 흐름 및 결과 산출
/// - 나니노벨 변수로 결과 전달 후 다음 스크립트 재생
/// </summary>
public class ShopManager : MonoBehaviour
{
    private string ScriptName = "ShopPuzzle";
    private string Label;

    //게임이 시작할때 가지고 있는 돈
    public int entireMoney = 20000;

    public ReceiptManager receiptManager; // 영수증 데이터 관리자(외부)
    //현재 플레이어가 가진돈
    [HideInInspector] public int currentMoney;

    //플레이어가 구매한 물품의 가격 합
    [HideInInspector] public int menuSum;

    //상점 물품구매오브젝트
    public GameObject shopList;
    //상점 구매 결과 오브젝트
    public GameObject shopResult;
    //상점 구매 영수증 오브젝트
    public GameObject shopReceipt;

    //헌재돈을 표시하는 텍스트오브젝트
    public TMP_Text moneyCountText;
    public TMP_Text receiptCountText;

    //게임을 끝내는 버튼을 누르면 비활성화 하기위한 오브젝트
    public GameObject gameFinishBtn;

    bool finishGame;

    //매뉴판에서 넣는 물품들
    public ShopMenu[] entireMenu;
    //그 물품을 사면 산걸로 인식하는 변수를 담은 배열
    [HideInInspector] public bool[] entireBuyMenu;

    //클리어 후 점수 체크용 변수
    [HideInInspector] public int pointCount;

    //메뉴를 사면 어떤종류를 샀는지 인식하는 변수 (전부 true값이면 점수체크용 변수에 점수 추가)
    [HideInInspector] public bool buyMain = false;
    [HideInInspector] public int buySause = 0; // 소스는 2개 이상 필요
    [HideInInspector] public int buyETC = 0;

    //나올 말풍선들(소스 부족 알림)
    public GameObject sauseNeedObj;
    Coroutine showSauseNeedCoroutine;

    //게임이 끝나고 나온 결과를 저장하는 변수
    string resultFood;
    int resultMoney;
    public bool plusChoice; // 잔액 여유에 따른 추가 선택 플래그
    public GameObject finishBlack; // 종료 페이드

    void Awake()
    {
        entireBuyMenu = new bool[receiptManager.receiptMenu.Length];
        finishBlack.GetComponent<Image>().color = new Color(0f,0f,0f,0f);
    }

    void Start()
    {
        // 예산 초기화 및 변수 셋업
        currentMoney = entireMoney;
        var getLabel = Engine.GetService<ICustomVariableManager>();
        plusChoice = false;
        getLabel.TrySetVariableValue("ShopPlusChoice", plusChoice);

        // 다음 이동 레이블 획득
        getLabel.TryGetVariableValue<int>("ShopCheck", out var labelWant);
        Label = labelWant.ToString();

        ResetGame();

        menuSum = 0;
        buyMain = false;
        buySause = 0;
        buyETC = 0;
    }

    public void ResetGame()
    {
        currentMoney = entireMoney;
        finishGame = false;
        pointCount = 0;
    }

    void Update()
    {
        //게임이 종료되면 정산버튼을 비활성화
        gameFinishBtn.GetComponent<Button>().interactable = !finishGame;

        // 디스플레이 텍스트 갱신
        moneyCountText.text = "총액  " + menuSum.ToString() + " 원";
        receiptCountText.text = menuSum.ToString();
    }

    // 정산 버튼: 구매 결과를 확정하고 영수증으로 이동
    public void ClickFinishBtn()
    {
        buyMain = false;
        buySause = 0;
        buyETC = 0;

        // 전체 메뉴 검사 → 구매 플래그 채우기
        for (int i = 0; i < entireMenu.Length; i++)
        {
            entireMenu[i].GivePoint(); // 메뉴 타입 카운트, buyMenuCheck 셋
            entireBuyMenu[i] = entireMenu[i].buyMenuCheck;
        }

        // 소스 2개 이상인지 체크
        if(buySause>=2)
        {
            finishGame = true;
            shopReceipt.GetComponent<Animator>().SetBool("Show",true);
        }
        else
        {
            SauseNeed(); // 부족 알림
            return;
        }
    }

    void SauseNeed()
    {
        sauseNeedObj.transform.SetAsLastSibling();
        if (showSauseNeedCoroutine != null)
            StopCoroutine(showSauseNeedCoroutine);

        showSauseNeedCoroutine = StartCoroutine(ShowSause());
        return;
    }

    // 소스 부족 알림 말풍선 페이드 인/아웃
    public IEnumerator ShowSause()
    {
        for (float f = 0f; f < 1; f += 0.02f)
        {
            Color c = sauseNeedObj.GetComponent<Image>().color;
            c.a = f;
            sauseNeedObj.GetComponent<Image>().color = c;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        for (float f = 1f; f > 0; f -= 0.02f)
        {
            Color c = sauseNeedObj.GetComponent<Image>().color;
            c.a = f;
            sauseNeedObj.GetComponent<Image>().color = c;
            yield return null;
        }
    }

    // 영수증 확인: Yes
    public void ClickReceiptYes()
    {
        StartCoroutine(ReceiptFinishCheck());
    }

    // 영수증 확인: No (다시 고르기)
    public void ClickReceiptNo()
    {
        buySause = 0;
        finishGame = false;
        shopReceipt.GetComponent<Animator>().SetBool("Show",false);
    }

    // 나니노벨로 복귀
    void LastGameEnd()
    {
        var player = Engine.GetService<IScriptPlayer>();
        player.PreloadAndPlayAsync(ScriptName, label: Label).Forget();
    }

    // 최종 결과 계산 + 블랙 페이드 후 이동
    public IEnumerator ReceiptFinishCheck()
    {
        resultMoney = currentMoney;
        var acheive = Engine.GetService<ICustomVariableManager>();

        // 잔액 여유(예: 5000 이상)면 추가 선택 가능
        if(resultMoney >=5000)
        {
            plusChoice = true;
            acheive.TrySetVariableValue("ShopPlusChoice", plusChoice);
        }

        // 조합에 따라 결과 문자열 설정
        if(buyMain == false)
        {
            resultFood = "noChicken";
            acheive.TrySetVariableValue("ShopFood", resultFood);
        }
        else if(entireBuyMenu[1]==true && entireBuyMenu[3]==true && entireBuyMenu[4]==true)
        {
            resultFood = "soyGarlic";
            acheive.TrySetVariableValue("ShopFood", resultFood);
        }
        else if(entireBuyMenu[1]==true && entireBuyMenu[3]==true)
        {
            resultFood = "soy";
            acheive.TrySetVariableValue("ShopFood", resultFood);
        }
        else if(entireBuyMenu[7]==true && entireBuyMenu[5]==true && entireBuyMenu[9]==true && entireBuyMenu[4]==true)
        {
            resultFood = "hotSpicy";
            acheive.TrySetVariableValue("ShopFood", resultFood);
        }
        else if(entireBuyMenu[7]==true && entireBuyMenu[5]==true && entireBuyMenu[9]==true)
        {
            resultFood = "hotSpicy";
            acheive.TrySetVariableValue("ShopFood", resultFood);
        }
        else if(entireBuyMenu[7]==true && entireBuyMenu[5]==true && entireBuyMenu[4]==true)
        {
            resultFood = "spicy";
            acheive.TrySetVariableValue("ShopFood", resultFood);
        }
        else if(entireBuyMenu[6]==true && entireBuyMenu[8]==true)
        {
            resultFood = "salt";
            acheive.TrySetVariableValue("ShopFood", resultFood);
        }
        else
        {
            resultFood = "strange";
            acheive.TrySetVariableValue("ShopFood", resultFood);
        }

        // 블랙 페이드
        finishBlack.GetComponent<Image>().raycastTarget=true;
        for (float f = 0f; f < 1.2; f += 0.08f)
        {
            Color c = finishBlack.GetComponent<Image>().color;
            c.a = f;
            finishBlack.GetComponent<Image>().color = c;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        LastGameEnd();
    }
}
