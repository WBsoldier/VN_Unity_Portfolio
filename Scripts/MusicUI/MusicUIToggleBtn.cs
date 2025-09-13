using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Naninovel;

/// <summary>
/// 뮤직 UI 토글 버튼 동작을 담당.
/// - 특정 Toggle을 찾아 On 상태로 만들고
/// - 모든 음악 재생 상태를 초기화하며
/// - 오디오를 정지하고 Naninovel PlayScript를 재개.
/// </summary>
public class MusicUIToggleBtn : MonoBehaviour
{
    public GameObject disappearImage;     // 마우스 오버 시 표시/숨김될 이미지
    public string findToggleBtn;          // GameObject.Find로 찾을 Toggle 이름
    public AudioSource audioSource;       // 음악 재생 소스
    private GameObject[] resetMusicPlayBtns; // 재생 상태 초기화를 위한 버튼들 캐시
    public MusicGalleryManager musicGalleryManager; // 매니저 참조

    void Start()
    {
        // 갤러리 매니저에서 버튼 배열을 가져와 캐싱
        resetMusicPlayBtns = musicGalleryManager.musicPlayBtns;
    }

    /// <summary>
    /// 클릭 시: 모든 곡 재생 상태를 false로 만들고,
    /// 지정된 Toggle을 On으로 설정한 뒤 오디오를 정지, Naninovel 재생 재개.
    /// </summary>
    public void MouseClick()
    {
        for (int i = 0; i < resetMusicPlayBtns.Length; i++)
            resetMusicPlayBtns[i].GetComponent<MusicPlayBtn>().isPlayMusic = false;

        // 명시된 이름의 Toggle을 찾아서 On
        var findToggle = GameObject.Find(findToggleBtn).GetComponent<Toggle>().isOn = true;

        // 오디오 정지 및 나니노벨 PlayScript 재개
        audioSource.Stop();
        musicGalleryManager.GetComponent<PlayScript>().Play();
    }

    /// <summary> 마우스 오버: 안내 이미지 표시 </summary>
    public void MouseOver()
    {
        disappearImage.SetActive(true);
    }

    /// <summary> 마우스 이탈: 안내 이미지 숨김 </summary>
    public void MouseExit()
    {
        disappearImage.SetActive(false);
    }
}
