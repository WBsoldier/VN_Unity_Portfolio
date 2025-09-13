using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;
using Naninovel;

/// <summary>
/// 스토리 숏컷(챕터 점프) 메뉴를 관리.
/// - 나니노벨 변수 값에 따라 버튼 노출 한계 제어
/// - 메뉴 슬라이드 인/아웃 연출
/// - 마우스 오버 시 살짝 이동하는 연출
/// </summary>
public class StoryShortCutManager : MonoBehaviour
{
    public GameObject ShortCutContents;       // 숏컷 메뉴 루트 (슬라이드 대상)
    [HideInInspector] public bool showingShortCut = false; // 슬라이드 중 여부
    [HideInInspector] public bool showShortCut = false;    // 현재 표시 상태

    public Ease ease;                         // 열고 닫을 때의 Ease
    public Ease overEase;                     // 오버 시 미세 이동 Ease

    public GameObject[] shortCutBtns;         // 챕터 버튼들
    public GameObject[] mouseOverBtns;        // 오버 안내/보조 버튼들
    public string getVariable;                // 나니노벨 커스텀 변수명(진행도)
    public AudioClip pageMoveSound;           // 클릭/이동 사운드
    public AudioSource audioSource;           // 사운드 재생 소스
    public bool arangStory = true;            // true: 페이지→장 변환 규칙 적용

    void Update()
    {
        // 나니노벨 커스텀 변수 값 취득 (진행도 등)
        var naniVariable = Engine.GetService<ICustomVariableManager>();
        naniVariable.TryGetVariableValue<int>(getVariable, out var naniLimit);

        // 버튼 노출 제한 로직
        for (int i = 0; i < shortCutBtns.Length; i++)
        {
            if (arangStory)
            {
                // 페이지 값을 챕터 인덱스로 변환하여 사용
                int chapterIndex = PageToChapter(naniLimit);
                shortCutBtns[i].SetActive(i <= chapterIndex);
            }
            else
            {
                // 그대로 페이지 인덱스를 사용
                shortCutBtns[i].SetActive(i <= naniLimit);
            }
        }

        // 이동 중에는 상호작용 잠금
        ShortCutContents.GetComponent<CanvasGroup>().interactable = !showingShortCut;
    }

    /// <summary>
    /// 페이지 값을 챕터 인덱스로 변환.
    /// (게임 디자인에 맞춘 특수 규칙)
    /// </summary>
    private int PageToChapter(int page)
    {
        if (page <= 6) return page;                // 1~7페이지 = 1~7장
        else if (page >= 7 && page <= 10) return 7;// 8~11페이지 = 8장
        else if (page >= 11) return page - 3;      // 12페이지 = 9장
        return page;
    }

    /// <summary>
    /// 페이지 이동 사운드 재생(메뉴 표시 중에는 무음).
    /// </summary>
    public void PlayPageSound()
    {
        if (showShortCut) return;

        audioSource.clip = pageMoveSound;
        audioSource.Play();
    }

    // ========== 메뉴 슬라이드 인/아웃 ==========

    /// <summary> 메뉴 표시 애니메이션 </summary>
    private IEnumerator CutMenuShow()
    {
        showingShortCut = true;
        ShortCutContents.GetComponent<RectTransform>().DOAnchorPosX(770, 0.4f).SetEase(ease, 0.8f);
        yield return new WaitForSeconds(0.5f);

        showingShortCut = false;
        showShortCut = true;

        // 약간의 여유 후 마우스오버 보조 버튼들 활성화
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < mouseOverBtns.Length; i++)
            mouseOverBtns[i].SetActive(true);
    }

    /// <summary> 메뉴 숨김 애니메이션 </summary>
    private IEnumerator CutMenuHide()
    {
        showingShortCut = true;
        ShortCutContents.GetComponent<RectTransform>().DOAnchorPosX(-5, 0.4f).SetEase(ease, 0.8f);
        yield return new WaitForSeconds(0.5f);

        showingShortCut = false;
        showShortCut = false;

        // 약간의 여유 후 마우스오버 보조 버튼들 다시 활성화
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < mouseOverBtns.Length; i++)
            mouseOverBtns[i].SetActive(true);
    }

    /// <summary>
    /// 토글 동작: 닫혀있으면 열고, 열려있으면 닫음.
    /// </summary>
    public void ShowCutMenu()
    {
        // 애니메이션 중 표시를 제한하기 위해 보조 버튼들 잠시 숨김
        for (int i = 0; i < mouseOverBtns.Length; i++)
            mouseOverBtns[i].SetActive(false);

        if (!showShortCut) StartCoroutine(CutMenuShow());
        else               StartCoroutine(CutMenuHide());
    }

    /// <summary> 외부에서 강제로 닫기 </summary>
    public void HideCutMenu() => StartCoroutine(CutMenuHide());

    /// <summary> 마우스 오버 시 살짝 우측으로 이동 </summary>
    public void OverMouse()
    {
        ShortCutContents.GetComponent<RectTransform>().DOAnchorPosX(15, 0.2f).SetEase(overEase);
    }

    /// <summary> 마우스 이탈 시 원위치로 이동 </summary>
    public void ExitMouse()
    {
        ShortCutContents.GetComponent<RectTransform>().DOAnchorPosX(-5, 0.2f).SetEase(overEase);
    }
}
