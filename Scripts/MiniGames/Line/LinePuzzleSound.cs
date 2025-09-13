using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 간단한 퍼즐 효과음 관리(두 종류 + 정지).
/// UI 애니메이션/트리거에서 호출용.
/// </summary>
public class LinePuzzleSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip1;
    public AudioClip audioClip2;

    public void StartAudio1()
    {
        audioSource.clip = audioClip1;
        audioSource.Play();
    }

    public void StartAudio2()
    {
        audioSource.clip = audioClip2;
        audioSource.Play();
    }

    public void StopAudio()
    {
        audioSource.Stop();
    }
}
