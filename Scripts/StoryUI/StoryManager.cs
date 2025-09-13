using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 스토리 페이지, 챕터 전환, 캐릭터(아랑/지은) 연출, 페이드 인/아웃을 관리.
/// - 페이지 이동 시 슬라이드 애니메이션
/// - 특정 캐릭터 파트 진입 시 단계적 연출
/// </summary>
public class StoryManager : MonoBehaviour
{
    public StoryPageValue storyPageValue;            // 현재 페이지 값(외부 ScriptableObject나 클래스일 가능성)
    public StoryShortCutManager storyShortCutManager;// 숏컷(챕터 점프) UI 제어
    public GameObject StoryContents;                 // 전체 페이지 컨테이너 (슬라이드 대상)
    [HideInInspector] public bool movePage = false;  // 페이지 이동 중 여부(입력 잠금)
    public TMP_Text pageText;                        // "n 장" 텍스트
    public GameObject otherPage;                     // 반대 파트(상호 잠금/해제용)

    // 아랑/지은 파트의 연출 대상들
    public GameObject[] arangStory;
    public GameObject[] arangBlack;
    public GameObject[] jieunStory;
    public GameObject[] jieunStart;
    public GameObject[] arangStart;
    public GameObject arangBack;
    public GameObject arangBackOrigin;
    public GameObject StoryMain;

    public AudioClip jChangeSound;                   // 전환 사운드
    public AudioSource audioSource;                  // 사운드 재생 소스

    void Update()
    {
        // 페이지 값에 따른 표기 규칙
        if (storyPageValue.pageValue + 1 < 9)
            pageText.text = (storyPageValue.pageValue + 1) + " 장";
        else if (storyPageValue.pageValue + 1 == 9 || storyPageValue.pageValue + 1 == 10 || storyPageValue.pageValue + 1 == 11)
            pageText.text = "8 장";
        else if (storyPageValue.pageValue + 1 == 14)
            pageText.text = "최종장";
        else if (storyPageValue.pageValue + 1 == 15)
            pageText.text = "후일담";
        else
            pageText.text = (storyPageValue.pageValue - 2) + " 장";

        // 이동 중에는 상호작용 잠금
        StoryContents.GetComponent<CanvasGroup>().interactable = !movePage;
    }

    /// <summary>
    /// StoryContents를 X축으로 슬라이드하여 페이지 이동.
    /// </summary>
    private IEnumerator pageMove()
    {
        movePage = true;
        StoryContents.transform.DOLocalMoveX(storyPageValue.pageValue * -3840, 1).SetEase(Ease.OutQuad);
        yield return new WaitForSeconds(1f);
        movePage = false;
    }

    /// <summary>
    /// 지은 파트로 진입하는 연출. (Start → Story 순차 페이드인)
    /// 반대 파트 상호작용 잠그고, 완료 후 해제.
    /// </summary>
    private IEnumerator GoJieunPage()
    {
        movePage = true;
        storyShortCutManager.HideCutMenu();
        StoryMain.GetComponent<CanvasGroup>().interactable = false;

        audioSource.clip = jChangeSound;
        audioSource.Play();

        // 지은 파트 시작 연출 순차 페이드인
        for (int i = 0; i < jieunStart.Length; i++)
        {
            StartCoroutine(StartFadeIn(jieunStart[i], 0.5f));
            yield return new WaitForSeconds(0.5f);
        }

        // 지은 파트 본편 순차 페이드인
        for (int i = 0; i < jieunStory.Length; i++)
        {
            StartCoroutine(FadeIn(jieunStory[i], 1f));
            yield return new WaitForSeconds(1f);
        }

        // 아랑 파트는 전부 숨김
        for (int i = 0; i < arangStory.Length; i++)
            arangStory[i].GetComponent<CanvasGroup>().alpha = 0;

        yield return new WaitForSeconds(1f);

        // 상호작용 재개 및 반대 파트 잠금/해제
        StoryMain.GetComponent<CanvasGroup>().interactable = true;
        for (int i = 0; i < arangStory.Length; i++)
            StartCoroutine(FadeOut(arangStory[i], 0f));

        for (int i = 0; i < arangBlack.Length; i++)
            StartCoroutine(StartFadeOut(arangBlack[i], 0f));

        for (int i = 0; i < jieunStory.Length; i++)
            jieunStory[i].GetComponent<CanvasGroup>().interactable = true;

        for (int i = 0; i < arangStory.Length; i++)
        {
            arangStory[i].GetComponent<CanvasGroup>().interactable = false;
            arangStory[i].GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        otherPage.GetComponent<CanvasGroup>().interactable = true;
        otherPage.GetComponent<CanvasGroup>().blocksRaycasts = true;

        movePage = false;
    }

    /// <summary>
    /// 아랑 파트로 진입하는 연출.
    /// </summary>
    private IEnumerator GoArangPage()
    {
        movePage = true;
        storyShortCutManager.HideCutMenu();
        StoryMain.GetComponent<CanvasGroup>().interactable = false;

        audioSource.clip = jChangeSound;
        audioSource.Play();

        // 지은 파트 상호작용 비활성
        for (int i = 0; i < jieunStory.Length; i++)
            jieunStory[i].GetComponent<CanvasGroup>().interactable = false;

        // 아랑 파트 시작 연출
        for (int i = 0; i < arangStart.Length; i++)
        {
            StartCoroutine(StartFadeIn(arangStart[i], 0.5f));
            yield return new WaitForSeconds(0.5f);
        }

        // 배경 등 전환
        StartCoroutine(FadeIn(arangBack, 0.5f));
        yield return new WaitForSeconds(0.5f);
        arangBackOrigin.GetComponent<CanvasGroup>().alpha = 1f;

        // 지은 파트는 숨김/초기화
        for (int i = 0; i < jieunStory.Length; i++)
            jieunStory[i].GetComponent<CanvasGroup>().alpha = 0f;

        for (int i = 0; i < jieunStart.Length; i++)
            jieunStart[i].GetComponent<Image>().fillAmount = 0f;

        for (int i = 0; i < arangStart.Length; i++)
            arangStart[i].GetComponent<Image>().fillAmount = 0f;

        arangBack.GetComponent<CanvasGroup>().alpha = 0f;

        // 검은 연출 → 본편 순차 진입
        for (int i = 0; i < arangBlack.Length; i++)
        {
            StartCoroutine(StartFadeIn(arangBlack[i], 0.5f));
            yield return new WaitForSeconds(0.5f);
        }

        for (int i = 0; i < arangStory.Length; i++)
        {
            StartCoroutine(FadeIn(arangStory[i], 1f));
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(1f);

        // 상호작용 재개 및 반대 파트 잠금/해제
        StoryMain.GetComponent<CanvasGroup>().interactable = true;
        otherPage.GetComponent<CanvasGroup>().interactable = false;
        otherPage.GetComponent<CanvasGroup>().blocksRaycasts = false;

        for (int i = 0; i < arangStory.Length; i++)
        {
            arangStory[i].GetComponent<CanvasGroup>().interactable = true;
            arangStory[i].GetComponent<CanvasGroup>().blocksRaycasts = true;
        }

        movePage = false;
    }

    // ========= 공통 연출 유틸 =========

    /// <summary> CanvasGroup 알파를 0→1로 duration 동안 선형 보간 </summary>
    private IEnumerator FadeIn(GameObject fadeObj, float duration)
    {
        float elapsed = 0f;
        CanvasGroup canvasGroup = fadeObj.GetComponent<CanvasGroup>();

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            canvasGroup.alpha = t;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    /// <summary> CanvasGroup 알파를 1→0으로 duration 동안 선형 보간 </summary>
    private IEnumerator FadeOut(GameObject fadeObj, float duration)
    {
        float elapsed = 1f;
        CanvasGroup canvasGroup = fadeObj.GetComponent<CanvasGroup>();

        while (elapsed > duration)
        {
            elapsed -= Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            canvasGroup.alpha = t;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

    /// <summary> Image.fillAmount를 0→1로 페이드인(원형/막대 형태의 채우기 연출) </summary>
    private IEnumerator StartFadeIn(GameObject fadeObj, float duration)
    {
        float elapsed = 0f;
        Image img = fadeObj.GetComponent<Image>();

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            img.fillAmount = t;
            yield return null;
        }
        img.fillAmount = 1f;
    }

    /// <summary> Image.fillAmount를 1→0으로 페이드아웃 </summary>
    private IEnumerator StartFadeOut(GameObject fadeObj, float duration)
    {
        float elapsed = 1f;
        Image img = fadeObj.GetComponent<Image>();

        while (elapsed > duration)
        {
            elapsed -= Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            img.fillAmount = t;
            yield return null;
        }
        img.fillAmount = 0f;
    }

    // ========= 버튼 이벤트 =========

    /// <summary> 다음 페이지로 이동 </summary>
    public void NextPush()
    {
        storyPageValue.pageValue++;
        StartCoroutine(pageMove());
    }

    /// <summary> 이전 페이지로 이동 </summary>
    public void PrevPush()
    {
        storyPageValue.pageValue--;
        StartCoroutine(pageMove());
    }

    /// <summary> 지은 파트로 전환 </summary>
    public void JPush() => StartCoroutine(GoJieunPage());

    /// <summary> 아랑 파트로 전환 </summary>
    public void APush() => StartCoroutine(GoArangPage());

    /// <summary> 컷 씬 등 페이지 슬라이드만 수행 </summary>
    public void CutPush() => StartCoroutine(pageMove());
}
