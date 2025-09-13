using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 여러 페이지를 좌우 버튼으로 넘기는 단순 페이지네이션 UI 관리.
/// - 이전/다음 버튼 활성화 제어
/// - 현재 페이지 인덱스 기반으로 페이지 활성/비활성 전환
/// </summary>
public class RecordPageManager : MonoBehaviour
{
    private int currentPage = 0;    // 현재 페이지 인덱스 (0부터 시작)
    public GameObject[] maxPage;    // 각 페이지 루트 오브젝트
    public TMP_Text pageText;       // 표시용 페이지 숫자 텍스트
    public Button previousBtn;      // 이전 버튼
    public Button nextBtn;          // 다음 버튼

    void Awake()
    {
        // 시작 시 0번 페이지만 보이게 하고 나머지는 숨김
        for (int i = 1; i < maxPage.Length; i++)
            maxPage[i].SetActive(false);
    }

    void Update()
    {
        // 첫 페이지에서는 이전 버튼 비활성
        previousBtn.interactable = currentPage != 0;

        // 마지막 페이지에서는 다음 버튼 비활성
        nextBtn.interactable = currentPage != (maxPage.Length - 1);

        // 1부터 시작하는 페이지 숫자 표시
        pageText.text = (currentPage + 1).ToString();
    }

    /// <summary> UI 이벤트: 이전 버튼 클릭 </summary>
    public void PushPreviousBtn() => PreviousPage();

    /// <summary> UI 이벤트: 다음 버튼 클릭 </summary>
    public void PushNextBtn() => NextPage();

    /// <summary>
    /// 다음 페이지로 이동.
    /// 현재 페이지가 마지막이면 무시.
    /// </summary>
    void NextPage()
    {
        if (currentPage == maxPage.Length) return;

        // 현재 활성 페이지를 찾아 끄고 다음 페이지를 켬
        for (int i = currentPage; i < maxPage.Length; i++)
        {
            if (maxPage[i].activeSelf)
            {
                maxPage[i].SetActive(false);
                int nextIndex = i + 1;
                maxPage[nextIndex].SetActive(true);
                currentPage++;
                break;
            }
        }
    }

    /// <summary>
    /// 이전 페이지로 이동.
    /// 현재 페이지가 0이면 무시.
    /// </summary>
    void PreviousPage()
    {
        if (currentPage == 0) return;

        // 현재 활성 페이지를 찾아 끄고 이전 페이지를 켬
        for (int i = currentPage; i < maxPage.Length; i--)
        {
            if (maxPage[i].activeSelf)
            {
                maxPage[i].SetActive(false);
                int prevIndex = i - 1;
                maxPage[prevIndex].SetActive(true);
                currentPage--;
                break;
            }
        }
    }
}
