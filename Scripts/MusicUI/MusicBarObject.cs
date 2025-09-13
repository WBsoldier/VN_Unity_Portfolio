using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// BGM 재생 진행도를 슬라이더와 시간 텍스트로 표시하는 클래스.
/// </summary>
public class MusicBarObject : MonoBehaviour
{
    private AudioSource bgmSource; // 현재 재생 중인 BGM 소스
    public MusicPlayBtn musicPlayBtn; // 재생 버튼 스크립트 참조
    public Slider progressBar; // BGM 진행도 슬라이더
    public TMP_Text timeText; // 남은 시간 텍스트
    public TMP_Text playingTimeText; // 남은 시간 텍스트 (중복 표기용)

    void Start()
    {
        progressBar.minValue = 0;
        progressBar.maxValue = 1;
        bgmSource = musicPlayBtn.audioSource;
    }

    void Update()
    {
        // BGM 전체 길이 표시
        float firstTime = musicPlayBtn.bgmClip.length;
        int firstMin = Mathf.FloorToInt(firstTime / 60f);
        int firstSec = Mathf.FloorToInt(firstTime % 60f);
        timeText.text = $"{firstMin:D2}:{firstSec:D2}";
        playingTimeText.text = $"{firstMin:D2}:{firstSec:D2}";

        if (bgmSource != null && bgmSource.clip != null)
        {
            float currentTime = bgmSource.time;
            float totalTime = musicPlayBtn.bgmClip.length;
            float remainingTime;

            if (musicPlayBtn.isPlayMusic && bgmSource.isPlaying)
            {
                remainingTime = totalTime - currentTime;
                progressBar.value = currentTime / totalTime; // 진행도 갱신
            }
            else
            {
                remainingTime = musicPlayBtn.bgmClip.length;
                progressBar.value = 0f;
            }

            // 남은 시간 표시
            int remainingMin = Mathf.FloorToInt(remainingTime / 60f);
            int remainingSec = Mathf.FloorToInt(remainingTime % 60f);

            timeText.text = $"{remainingMin:D2}:{remainingSec:D2}";
            playingTimeText.text = $"{remainingMin:D2}:{remainingSec:D2}";
        }
        else
        {
            progressBar.value = 0f;
        }
    }
}