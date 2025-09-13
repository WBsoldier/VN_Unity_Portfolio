using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 오디오 스펙트럼 데이터를 이용해 시각적 이퀄라이저 효과를 구현.
/// </summary>
public class MusicEqualizerEffect : MonoBehaviour
{
    public AudioSource audioSource; // 대상 오디오 소스
    public RectTransform[] bars;    // 이퀄라이저 막대들
    public float intensity = 100f;  // 감도
    public float maxHeight = 80f;   // 막대 최대 높이
    public float smoothingSpeed = 20f; // 변화 부드럽게 적용하는 속도

    private float[] spectrum; // 스펙트럼 데이터 배열

    void Start()
    {
        spectrum = new float[64];
    }

    void Update()
    {
        if (audioSource == null || !audioSource.isPlaying || audioSource.clip == null) return;

        // 스펙트럼 데이터 추출
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        // 막대 크기 갱신
        for (int i = 0; i < bars.Length; i++)
        {
            float target = Mathf.Clamp(spectrum[i] * intensity, 5f, maxHeight);
            float current = bars[i].sizeDelta.y;
            float smooth = Mathf.Lerp(current, target, Time.deltaTime * smoothingSpeed);
            bars[i].sizeDelta = new Vector2(bars[i].sizeDelta.x, smooth);
        }
    }
}
