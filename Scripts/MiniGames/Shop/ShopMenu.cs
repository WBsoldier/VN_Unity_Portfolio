using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 단일 메뉴 아이템의 구매/취소 로직.
/// - 고정 구매(rock) 항목 처리
/// - 금액/합계 반영
/// - 타입별 카운트는 GivePoint에서 ShopManager로 전달
/// </summary>
public class ShopMenu : MonoBehaviour
{
    public ShopManager shopManager;

    //이 제품이 몇번째 제품인지 표시하는 정수'
    public int menuOrder = 0;

    //산 제품인지 표시하는 이미지
    public GameObject buyCheckImage;

    //살꺼임?하고 묻는 표시이미지 투명버전
    public GameObject buyQuestion;

    //현재 이 물품을 샀는지
    [HideInInspector] public bool buyThisMenu = false;

    public int thisMenuMoney;           // 가격
    [HideInInspector] public bool buyMenuCheck; // 정산 시 구매 여부

    //고정으로 사는 물품이 있으면 무조건사게
    public bool rockBuyMenu;     // 시작부터 확정 구매
    bool rockMenuCount;          // 중복 차감 방지

    //돈 부족하면 나오는 말풍선 
    public GameObject moneyNeedObj;
    Coroutine showMoneyNeedCoroutine;

    //종류별로 물품을 샀는지 확인하는 변수
    public enum MenuType { Main, Sause, ETC }
    public MenuType menuType;

    void Start()
    {
        // rock이면 바로 구매 상태로
        buyThisMenu = rockBuyMenu ? true : false;
    }

    void Update()
    {
        // 고정 구매는 버튼 비활성 + 한 번만 금액 반영
        if(rockBuyMenu == true)
        {
            this.GetComponent<Button>().interactable = false;
            if(rockMenuCount == false)
            {
                shopManager.currentMoney -= thisMenuMoney;
                shopManager.menuSum += thisMenuMoney;
                rockMenuCount = true;
            }
        }

        // 구매 체크 마크 표시
        buyCheckImage.SetActive(buyThisMenu);
    }

    //이거 지워야 할수도 있음(호버용)
    public void EnterBtn()  => buyQuestion.SetActive(true);
    public void ExitBtn()   => buyQuestion.SetActive(false);

    // 클릭으로 구매/취소 토글
    public void ClickBuyBtn()
    {
        if(buyThisMenu == false)
        {
            // 잔액 부족
            if(shopManager.currentMoney < thisMenuMoney)
            {
                moneyNeedObj.transform.SetAsLastSibling();
                if (showMoneyNeedCoroutine != null)
                    StopCoroutine(showMoneyNeedCoroutine);

                showMoneyNeedCoroutine = StartCoroutine(ShowMoneyNeed());
                return;
            }

            // 구매
            buyThisMenu = true;
            shopManager.currentMoney -= thisMenuMoney;
            shopManager.menuSum += thisMenuMoney;
        }
        else
        {
            // 취소
            buyThisMenu = false;
            shopManager.currentMoney += thisMenuMoney;
            shopManager.menuSum -= thisMenuMoney;
        }
    }

    // 잔액 부족 말풍선 페이드
    public IEnumerator ShowMoneyNeed()
    {
        for (float f = 0f; f < 1; f += 0.02f)
        {
            Color c = moneyNeedObj.GetComponent<Image>().color;
            c.a = f;
            moneyNeedObj.GetComponent<Image>().color = c;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        for (float f = 1f; f > 0; f -= 0.02f)
        {
            Color c = moneyNeedObj.GetComponent<Image>().color;
            c.a = f;
            moneyNeedObj.GetComponent<Image>().color = c;
            yield return null;
        }
    }

    //구매 끝 버튼 누르면 플레이어가 샀으면 변수를 true로 반환
    public void GivePoint()
    {
        //선택한 메뉴의 종류를 인식하기
        if(buyThisMenu == true)
        {
            buyMenuCheck = true;

            if(menuType == MenuType.Main)
                shopManager.buyMain = true;
            else if(menuType == MenuType.Sause)
                shopManager.buySause += 1;
            else
                return; // ETC는 현재 점수 반영 없음
        }
        else
        {
            buyMenuCheck = false;
        }
    }
}
