using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    [HideInInspector] public bool count = false; // 이 라인을 이미 채웠는지(중복 처리 방지)

    [SerializeField] private LineRenderer line;   // 라인 렌더러
    [SerializeField] private Gradient startColor; // 기본 색
    [SerializeField] private Gradient activeColor;// 채워졌을 때 색
    public LineGameManager lineGameManager;       // 매니저 참조
    public int startId; // 시작 포인트 id
    public int endId;   // 끝 포인트 id

    //사운드관련
    public AudioClip connectAudioClip;
    public AudioSource audioSource;

    void Start()
    {
        count = false;

        // 매니저가 (시작→끝)과 (끝→시작) 양방향으로 같은 라인을 찾을 수 있도록 딕셔너리에 등록
        Line edge = transform.GetComponent<Line>();
        Vector2Int normal = new Vector2Int(startId, endId);
        Vector2Int reverse = new Vector2Int(normal.y, normal.x);
        lineGameManager.usedLine[normal] = edge;
        lineGameManager.usedLine[reverse] = edge;

        // 리셋할 때를 위해 전체 라인 리스트에도 추가
        lineGameManager.lines.Add(edge);
    }


    // 라인을 채웠을 때: 사운드 재생 + 색 변경 + 카운트 증가
    public void Add()
    {
        audioSource.clip = connectAudioClip;
        audioSource.Play();
        count = true;
        line.colorGradient = activeColor;
        lineGameManager.clearCount++;
    }

    // 라인 상태 복구(리셋)
    public void Return()
    {
        count = false;
        line.colorGradient = startColor;
    }
}
