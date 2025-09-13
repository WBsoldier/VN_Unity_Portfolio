using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 라인 퍼즐 시작 연출 전체 컨트롤.
/// - 인트로 영상 페이드아웃
/// - 블랙 배경 페이드아웃
/// - 보조 캔버스/텍스트 페이드인
/// - 포인트/라인 등장
/// - BGM 페이드인
/// </summary>
public class StartLinePuzzle : MonoBehaviour
{
    public GameObject startVideo;
    public GameObject blackBack;
    public GameObject pointsCanvas;
    public Material lineMat;
    public GameObject circleCanvas;
    public GameObject nameCanvas;
    public Animator startAnim;

    //오디오 소스들
    public AudioSource audioSource;            // 배경
    public AudioSource startAudioSource;       // 블랙 배경 사라질 때
    public AudioClip startClip;
    public AudioSource startSecondAudioSource; // 포인트/라인 등장 단계
    public AudioClip startSecondClip;

    void Start()
    {
        // 초기 투명/잠금 세팅
        pointsCanvas.GetComponent<CanvasGroup>().alpha = 0f;
        nameCanvas.GetComponent<CanvasGroup>().alpha = 0f;
        pointsCanvas.GetComponent<CanvasGroup>().interactable = false;
        circleCanvas.GetComponent<CanvasGroup>().alpha = 0f;
        lineMat.color = new Color(1f, 1f, 1f, 0f);

        StartCoroutine(FadeInStart());
        startAnim.SetBool("Start" , false);
    }

    //페이드 아웃(전체 인트로 연출)
    public IEnumerator FadeInStart()
    {
        // 1) 인트로 영상 페이드아웃
        yield return new WaitForSeconds(5.8f);
        for (float f = 1f; f > 0; f -= 0.02f)
        {
            Color c = startVideo.GetComponent<RawImage>().color;
            c.a = f;
            startVideo.GetComponent<RawImage>().color = c;
            yield return null;
        }
        startVideo.SetActive(false);

        // 2) 블랙 배경 페이드아웃 + 사운드
        yield return new WaitForSeconds(0.5f);
        startAudioSource.clip = startClip;
        startAudioSource.Play();
        for (float f = 1f; f > 0; f -= 0.02f)
        {
            Color c = blackBack.GetComponent<Image>().color;
            c.a = f;
            blackBack.GetComponent<Image>().color = c;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        blackBack.SetActive(false);

        // 3) 배경 애니메이션 트리거
        startAnim.SetBool("Start" , true);
        yield return new WaitForSeconds(2f);

        // 4) 보조 캔버스(원형 등) 페이드 인/아웃/인
        for (float f = 0f; f < 1; f += 0.1f)
        {
            float a = circleCanvas.GetComponent<CanvasGroup>().alpha;
            a = f;
            circleCanvas.GetComponent<CanvasGroup>().alpha = a;
            if(a>0.99f) a=1f;
            yield return null;
        }
        for (float f = 1f; f > 0; f -= 0.1f)
        {
            float a = circleCanvas.GetComponent<CanvasGroup>().alpha;
            a = f;
            circleCanvas.GetComponent<CanvasGroup>().alpha = a;
            if(a>0.99f) a=1f;
            yield return null;
        }
        for (float f = 0f; f < 1; f += 0.1f)
        {
            float a = circleCanvas.GetComponent<CanvasGroup>().alpha;
            a = f;
            circleCanvas.GetComponent<CanvasGroup>().alpha = a;
            if(a>0.99f) a=1f;
            yield return null;
        }

        // 5) 타겟 이름 페이드인
        yield return new WaitForSeconds(1f);
        for (float f = 0f; f < 1; f += 0.02f)
        {
            float a = nameCanvas.GetComponent<CanvasGroup>().alpha;
            a = f;
            nameCanvas.GetComponent<CanvasGroup>().alpha = a;
            if(a>0.999f) a=1f;
            yield return null;
        }

        // 6) 포인트/라인 등장 + 사운드
        yield return new WaitForSeconds(1f);
        startSecondAudioSource.clip = startSecondClip;
        startSecondAudioSource.Play();
        StartCoroutine(ShowPointsStart());
        StartCoroutine(ShowLineStart());

        // 7) 포인트 인터랙션 활성화 + 배경 사운드 페이드인
        yield return new WaitForSeconds(1f);
        pointsCanvas.GetComponent<CanvasGroup>().interactable = true;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FadeIn(audioSource,5));
    }

    // 배경 오디오 페이드인
    public static IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
    {
        float startVolume = 0.2f;
        audioSource.volume = 0;
        audioSource.Play();

        while (audioSource.volume < 0.3f)
        {
            audioSource.volume += startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }
        audioSource.volume = 0.3f;
    }

    //페이드 인(단일 이미지용, 필요 시 사용)
    public IEnumerator FadeOutStart(GameObject fadeObj)
    {
        fadeObj.SetActive(true);
        for (float f = 0f; f < 1; f += 0.05f)
        {
            Color c = fadeObj.GetComponent<Image>().color;
            c.a = f;
            fadeObj.GetComponent<Image>().color = c;
            yield return null;
        }
    }

    //포인트와 선 나오게하기(캔버스 알파)
    public IEnumerator ShowPointsStart()
    {
        for (float f = 0f; f < 1; f += 0.02f)
        {
            float a = pointsCanvas.GetComponent<CanvasGroup>().alpha;
            a = f;
            pointsCanvas.GetComponent<CanvasGroup>().alpha = a;
            if(a>0.999f) a=1f;
            yield return null;
        }
    }

    // 라인 머티리얼 알파 페이드
    public IEnumerator ShowLineStart()
    {
        for (float f = 0f; f < 1; f += 0.02f)
        {
            Color c = lineMat.color;
            c.a = f;
            lineMat.color = c;
            if(c.a>0.999f) c.a=1f;
            yield return null;
        }
    }
}
