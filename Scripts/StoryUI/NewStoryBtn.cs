using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Naninovel;
using Naninovel.UI;
using UnityEngine.UI;
using Naninovel.Commands;
using System;
using System.Threading.Tasks;

/// <summary>
/// '새 이야기 시작' 버튼 동작을 담당.
/// - 확인 창 표시 후, 상태 초기화 및 장면 전환 연출
/// - Naninovel 관련 서비스들 초기화/재생
/// </summary>
public class NewStoryBtn : MonoBehaviour
{
    private IUIManager uiManager;               // UI 매니저
    private IConfirmationUI confirmationUI;     // 확인 다이얼로그 UI
    private IStateManager stateManager;         // 상태 저장/복원 매니저
    private IScriptPlayer scriptPlayer;         // 나니노벨 스크립트 플레이어

    protected static string ConfirmationMessage = "선택한 지점부터 시작하시겠습니까?";

    public GameObject informationCanvas;        // 마우스 오버 안내용 캔버스

    void Awake()
    {
        uiManager = Engine.GetService<IUIManager>();
        stateManager = Engine.GetService<IStateManager>();
    }

    void Start()
    {
        scriptPlayer = Engine.GetService<IScriptPlayer>();
        confirmationUI = uiManager.GetUI<IConfirmationUI>();
    }

    /// <summary>
    /// 본편 진입 전 블랙 페이드 후 챕터 이동.
    /// </summary>
    void GoNovel()
    {
        uiManager.GetUI("BlackUI")?.Show();
        Invoke("GoChapter", 0.5f);
    }

    /// <summary>
    /// 스크립트/상태 초기화 후 본편으로 진입.
    /// </summary>
    private async void GoChapter()
    {
        scriptPlayer.ResetService();
        await stateManager.ResetStateAsync();                      // 상태 초기화
        gameObject.GetComponent<PlayScript>().Play();              // 나니노벨 PlayScript 재생
        await Task.Delay(1500);                                    // 간단한 연출 대기
        uiManager.GetUI("BlackUI")?.Hide();
    }

    /// <summary>
    /// 확인창을 띄운 뒤 승인 시 본편으로 진입.
    /// BGM 정지 후 GoNovel 호출.
    /// </summary>
    private async void GoSelectChapterAsync()
    {
        if (!await confirmationUI.ConfirmAsync(ConfirmationMessage))
            return;

        var stopBGM = new StopBgm();
        stopBGM.ExecuteAsync().Forget();

        GoNovel();
    }

    /// <summary> 클릭: 확인창 절차 시작 </summary>
    public void MouseClick() => GoSelectChapterAsync();

    /// <summary> 마우스 오버: 정보 패널 서서히 표시 </summary>
    public void MouseOver() => StartCoroutine(FadeIn());

    /// <summary> 마우스 이탈: 정보 패널 서서히 숨김 </summary>
    public void MouseExit() => StartCoroutine(FadeOut());

    /// <summary> 정보 캔버스 알파를 0→1로 페이드인 </summary>
    private IEnumerator FadeIn()
    {
        for (float f = 0f; f < 1; f += 0.1f)
        {
            informationCanvas.GetComponent<CanvasGroup>().alpha = f;
            if (f >= 0.9f)
                informationCanvas.GetComponent<CanvasGroup>().alpha = 1f;

            yield return null;
        }
    }

    /// <summary> 정보 캔버스 알파를 1→0으로 페이드아웃 </summary>
    private IEnumerator FadeOut()
    {
        for (float f = 1f; f > 0; f -= 0.1f)
        {
            informationCanvas.GetComponent<CanvasGroup>().alpha = f;
            if (f <= 0.1f)
                informationCanvas.GetComponent<CanvasGroup>().alpha = 0f;

            yield return null;
        }
    }
}
