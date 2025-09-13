using UnityEngine;
using UnityEngine.UI;
using Naninovel;

/// <summary>
/// 뮤직 갤러리 UI 전체를 관리하는 매니저.
/// - 특정 음악을 선택하면 해당 UI만 활성화.
/// - 모든 음악 버튼의 상태를 초기화.
/// - 음악 재생 제어 및 UI 닫기 기능 포함.
/// </summary>
public class MusicGalleryManager : MonoBehaviour
{
    public GameObject[] musicObjs;        // 음악 관련 UI 오브젝트들
    public GameObject[] musicPlayBtns;    // 음악 재생 버튼들
    public AudioSource audioSource;       // 실제 음악 재생용 AudioSource

    /// <summary>
    /// 특정 음악을 선택해 재생하는 메서드.
    /// - 모든 버튼을 초기화한 후 선택된 음악만 활성화.
    /// </summary>
    public void PlayMusic(int value)
    {
        // 모든 버튼을 '재생 중 아님' 상태로 초기화
        for (int i = 0; i < musicPlayBtns.Length; i++)
        {
            musicPlayBtns[i].GetComponent<MusicPlayBtn>().isPlayMusic = false;
        }

        // 모든 음악 UI 비활성화
        for (int i = 0; i < musicObjs.Length; i++)
        {
            musicObjs[i].SetActive(false);
        }

        // 선택된 음악만 활성화
        musicObjs[value].SetActive(true);
        musicPlayBtns[value].GetComponent<MusicPlayBtn>().isPlayMusic = true;
    }

    /// <summary>
    /// 뮤직 갤러리 UI를 닫는 버튼 이벤트.
    /// - 모든 버튼을 초기화하고, 오디오 정지.
    /// - Naninovel PlayScript 재생 재개.
    /// </summary>
    public void HideMusicUIBtn()
    {
        for (int i = 0; i < musicPlayBtns.Length; i++)
        {
            musicPlayBtns[i].GetComponent<MusicPlayBtn>().isPlayMusic = false;
        }

        audioSource.Stop();
        gameObject.GetComponent<PlayScript>().Play();
    }
}
