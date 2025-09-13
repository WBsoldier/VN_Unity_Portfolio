using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 결과 화면 전용 효과음(등장/스탬프) 재생.
/// </summary>
public class ResultSoundManager : MonoBehaviour
{
    public AudioClip resultShowAudio;
    public AudioClip stampAudio;
    public AudioSource audioSource;

    public void PlayShowAudio()
    {
        audioSource.clip = resultShowAudio;
        audioSource.Play();
    }

    public void PlayStampAudio()
    {
        audioSource.clip = stampAudio;
        audioSource.Play();
    }
}
