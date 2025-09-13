using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Naninovel.Commands;
using Naninovel;
using UnityEngine.EventSystems;

/// <summary>
/// 뮤직 갤러리에서 각 곡을 재생/정지하는 버튼을 제어하는 클래스.
/// - 버튼 클릭 시 오디오 재생/정지
/// - UI 상태(재생 중, 마우스 오버 등)에 따른 아이콘 전환
/// - 드래그/스크롤 이벤트를 부모 ScrollRect로 전달
/// </summary>
public class MusicPlayBtn : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
{
    public ScrollRect ParentSR;                   // 부모 ScrollRect (스크롤 이벤트 전달용)
    public MusicGalleryManager musicGalleryManager; // 뮤직 갤러리 매니저 참조
    public int bgmValue;                          // 음악 인덱스 값
    public AudioClip bgmClip;                     // 재생할 오디오 클립
    public AudioSource audioSource;               // 오디오 재생기
    [HideInInspector] public bool isPlayMusic;    // 현재 곡 재생 여부
    [HideInInspector] public Sprite[] sprites;    // [0]: 재생 중 아이콘, [1]: 정지 아이콘

    public GameObject musicObj;                   // 곡 UI 오브젝트
    public GameObject[] isPlayingText;            // [0]: 재생 중 텍스트, [1]: 정지 상태 텍스트
    public GameObject equalizerObj;               // 이퀄라이저 효과 오브젝트
    public GameObject playBtn;                    // 재생 버튼 UI
    bool btnOverCheck;                            // 마우스 오버 상태 확인

    /// <summary>
    /// 버튼 클릭 시 실행.
    /// - 현재 곡이 재생 중이 아니면: 다른 곡 정지 후 해당 곡 재생.
    /// - 재생 중이면: 정지 및 Naninovel 플레이 재개.
    /// </summary>
    public void PushPlayBtn()
    {
        var stopBGM = new StopBgm();

        if (isPlayMusic == false) // 재생 시작
        {
            stopBGM.ExecuteAsync().Forget(); // Naninovel BGM 정지
            musicGalleryManager.PlayMusic(bgmValue); // 선택 곡으로 전환
            audioSource.Stop();
            audioSource.clip = bgmClip;
            audioSource.Play();
        }
        else // 재생 중 → 정지
        {
            audioSource.Stop();
            musicGalleryManager.GetComponent<PlayScript>().Play(); // Naninovel 스크립트 재개
            isPlayMusic = false;
        }
    }

    void Update()
    {
        if (isPlayMusic == true) // 재생 중 상태
        {
            musicObj.GetComponent<Image>().sprite = sprites[0];
            playBtn.GetComponent<CanvasGroup>().alpha = 0f;
            equalizerObj.GetComponent<CanvasGroup>().alpha = 1f;
            isPlayingText[0].SetActive(true);
            isPlayingText[1].SetActive(false);
        }
        else if (btnOverCheck == true) // 마우스 오버 상태
        {
            playBtn.GetComponent<CanvasGroup>().alpha = 1f;
            equalizerObj.GetComponent<CanvasGroup>().alpha = 0f;
            musicObj.GetComponent<Image>().sprite = sprites[1];
            isPlayingText[1].SetActive(true);
            isPlayingText[0].SetActive(false);
        }
        else // 기본 정지 상태
        {
            playBtn.GetComponent<CanvasGroup>().alpha = 1f;
            equalizerObj.GetComponent<CanvasGroup>().alpha = 0f;
            musicObj.GetComponent<Image>().sprite = sprites[1];
            isPlayingText[0].SetActive(false);
            isPlayingText[1].SetActive(true);
        }

        // 곡이 끝나면 상태 초기화
        if (audioSource.isPlaying == false)
        {
            isPlayMusic = false;
        }
    }

    /// <summary> 마우스 오버 시 호출 </summary>
    public void BtnOver() => btnOverCheck = true;

    /// <summary> 마우스가 버튼에서 벗어났을 때 호출 </summary>
    public void BtnExit() => btnOverCheck = false;

    // ================= 드래그/스크롤 이벤트 전달 =================

    public void OnBeginDrag(PointerEventData eventData)
    {
        ParentSR.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        ParentSR.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ParentSR.OnEndDrag(eventData);
    }

    public void OnScroll(PointerEventData eventData)
    {
        ParentSR.OnScroll(eventData);
    }
}
