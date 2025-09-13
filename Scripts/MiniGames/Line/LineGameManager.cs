using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Naninovel;
using DG.Tweening;

public class LineGameManager : MonoBehaviour
{
    public Button cleanBtn;

    //도전과제용 숫자
    private int achieveValue = 0;  // 클리어 방식에 따른 업적 플래그 계산용
    private string ScriptName = "LinePuzzle";
    private string Label;

    [SerializeField] private LineRenderer mouseLine; // 마우스로 긋는 임시 라인
    private Point startPoint, endPoint;              // 현재 시작/끝 포인트

    //시작 지점 끝지점을 만들어서 선이 인식해서 색이 바뀌도록
    [HideInInspector] public int startId = -1;
    [HideInInspector] public int endId = -1;

    //이미 지나간 선을 저장해서 다시 인식하지 않도록 지나간 선 저장
    public Dictionary<Vector2Int, Line> usedLine; // (시작,끝) → 해당 라인
    //리셋하기 위해서 모든 선을 저장할 리스트
    [HideInInspector] public List<Line> lines;

    //지금 이어지는 포인트로 하고있는지(다른 곳에서 시작되지 않게 하기 위해서)
    int currentId = -1; // 현재 이어서 가야 하는 다음 포인트 id

    //클리어 조건 만들기
    //클리어한 퍼즐이 몇번째 퍼즐인지(퍼즐순서에 따라 값을 주고 그 값에 따른 나니노벨 변수에 true로 반환해서 스킵가능한지 여부확인)
    public string clearLinePuzzleCount;

    //선이 색칠되고 올라가는 카운트(둘이서 비교해서 변화하는순간 끝지점이 시작지점이 되도록하기)
    [HideInInspector] public int clearCount;
    private int currentClearCount;

    //클리어 유무
    private bool hasGameFinished = false;

    //클리어 연출 관련
    public Material lineMaterial;           // 라인 페이드용 머티리얼
    public CanvasGroup stageOnePoints;      // 포인트 표시 그룹
    public Image clearStageOneImage;        // 1단계 흑백 이미지
    public Image clearStageOneRealImage;    // 1단계 컬러 이미지

    //클리어 관련 오브젝트 
    public TMP_Text clearPerText;     // 클리어 퍼센트 표시
    public GameObject clearPerBarFill;
    public GameObject clearPerBarFillFirst;
    public GameObject clearPerBar;
    public GameObject finishBlack;    // 마지막 페이드용 블랙
    public Animator finishAnimFirst;  // 1차 연출
    public Animator finishAnim;       // 2차 연출

    float loadingPercentage = 0f;         // 최종 퍼센트바
    float loadingMultiplier = 60f;
    float clearPerBarWidth;

    float loadingPercentageFirst = 0f;    // 1차 퍼센트바
    float loadingMultiplierFirst = 30f;
    float clearPerBarWidthFirst;

    bool finishGameFirst=false;   // 1차 퍼센트 진행 중
    bool finishGameSecond=false;  // 2차 연출 시작 여부
    bool finishGameLast=false;    // 마지막 퍼센트 진행 중
    bool endGame=false;           // 최종 종료 포인트 도달

    bool startRealImage=false;    // 컬러 이미지 페이드 시작 여부
    Color lineImageColor = new Color(1f,1f,1f,0f); // 흑백 이미지 알파
    Color lineColor = new Color(1f,1f,1f,0f);      // 라인 알파
    Color realImageColor = new Color(1f,1f,1f,0f); // 컬러 이미지 알파

    //오디오관련
    public AudioSource audioSource;            // 진행 중 BGM/효과
    public AudioSource firstFinishAudioSource; // 1차 퍼센트 사운드
    public AudioSource secondFinishAudioSource;// 2차 퍼센트 사운드
    bool secondFinishAudioSourceStop = false;  // 2차 사운드 한 번만 페이드아웃

    void Awake()
    {
        // 초기화
        currentId = -1;
        mouseLine.gameObject.SetActive(false);
        startId = -1;
        endId = -1;

        usedLine = new Dictionary<Vector2Int, Line>();
        lines = new List<Line>();

        // 클리어 연출 초기 상태 세팅(투명)
        Color clearColor = clearStageOneImage.GetComponent<Image>().color;
        clearColor.a = 0f;
        clearStageOneImage.GetComponent<Image>().color = clearColor;
        clearPerBar.GetComponent<Image>().color = clearColor;
        clearPerText.GetComponent<TMP_Text>().color = clearColor;

        finishAnim.SetBool("Finish" , false);
        finishAnimFirst.SetBool("Finish" , false);

        clearPerBarWidth = clearPerBarFill.GetComponent<RectTransform>().rect.width;
        clearPerBarWidthFirst = clearPerBarFillFirst.GetComponent<RectTransform>().rect.width;

        finishBlack.GetComponent<Image>().color = new Color(0f,0f,0f,0f);

        finishGameFirst=false;
        finishGameSecond=false;
        finishGameLast=false;

        //도전과제용 숫자
        achieveValue =0;
    }

    void Start()
    {
        // 나니노벨 변수에서 레이블 획득
        var getLabel = Engine.GetService<ICustomVariableManager>();
        getLabel.TryGetVariableValue<int>("LineCheck", out var labelWant);
        Label = labelWant.ToString();
    }

    void Update()
    {
        // ===== 퍼센트/연출 진행 =====
        clearPerText.text = loadingPercentage.ToString("F0")+"%";

        // 1차 퍼센트 진행
        if(finishGameFirst == true)
            loadingPercentageFirst += loadingMultiplierFirst * Time.deltaTime;

        loadingPercentageFirst = Mathf.Clamp(loadingPercentageFirst, 0, 100);
        clearPerBarFillFirst.transform.localPosition =
            new Vector3(-clearPerBarWidthFirst + (clearPerBarWidthFirst*loadingPercentageFirst/100) , 0, 0);

        // 1차 퍼센트 완료 시 사운드 정지
        if(loadingPercentageFirst >= 100f)
            firstFinishAudioSource.Stop();

        // 퍼센트에 맞춰 흑백 이미지/라인 알파 반비례, 이미지 페이드인
        if(lineImageColor.a < 1 && finishGameFirst == true && lineMaterial.color.a < 1f)
        {
            StartCoroutine(FadeOut(audioSource , 5f)); // 배경 오디오 서서히 줄이기
            lineImageColor.a = loadingPercentageFirst/50f;
            lineColor.a = 1f - lineImageColor.a;
            lineMaterial.color = lineColor;
            stageOnePoints.alpha = lineColor.a;
            clearStageOneImage.GetComponent<Image>().color = lineImageColor;
        }
        else if(lineImageColor.a > 1)
        {
            lineImageColor.a = 1f;
            startRealImage = true; // 다음 단계(컬러 이미지) 시작
        }

        if(lineMaterial.color.a > 1f)
            lineMaterial.color = new Color (1f,1f,1f,1f);

        // 컬러 이미지 등장
        if(startRealImage == true)
            realImageColor.a = (loadingPercentageFirst/50f)-1f;

        clearStageOneRealImage.GetComponent<Image>().color = realImageColor;

        // 컬러 이미지 페이드 완료 → 두 번째 연출로
        if(realImageColor.a >= 1f && finishGameSecond == false)
        {
            finishGameSecond = true;
            StartCoroutine(GameFinishSecond());
        }

        // 마지막 퍼센트 진행
        if(finishGameLast == true)
            loadingPercentage += loadingMultiplier * Time.deltaTime;

        loadingPercentage = Mathf.Clamp(loadingPercentage, 0, 100);
        clearPerBarFill.transform.localPosition =
            new Vector3(-clearPerBarWidth + (clearPerBarWidth*loadingPercentage/100) , 0, 0);

        // 최종 퍼센트 완료 → 블랙 페이드 후 종료 처리
        if(loadingPercentage>=100 && endGame == false)
        {
            endGame = true;
            StartCoroutine(GameFinishPerCentage());
        }

        // 85% 지점에서 2차 사운드 페이드아웃 한 번만
        if(loadingPercentage>=85 && secondFinishAudioSourceStop == false)
        {
            secondFinishAudioSourceStop = true;
            StartCoroutine(FadeOut(secondFinishAudioSource,1f));
        }

        // ===== 게임 로직(마우스 입력) =====
        if(hasGameFinished) return;

        // 마우스 다운: 시작 포인트 지정 + 임시 라인 초기화
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (!hit) return;

            startPoint = hit.collider.gameObject.GetComponent<Point>();
            startId = startPoint.id;

            // 이미 다른 포인트에서 이어가는 중이면, 같은 포인트에서만 다시 시작
            if(currentId != -1 && currentId != startPoint.id)
                return;

            mouseLine.gameObject.SetActive(true);
            mouseLine.positionCount = 2;
            mouseLine.SetPosition(0, startPoint.Position);
            mouseLine.SetPosition(1, startPoint.Position);
        }
        // 마우스 드래그: 레이캐스트로 끝 포인트 갱신 + 유효 라인이면 확정
        else if (Input.GetMouseButton(0) && startPoint != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit) endPoint = hit.collider.gameObject.GetComponent<Point>();

            mouseLine.SetPosition(1, mousePos2D);

            if (startPoint == endPoint || endPoint == null) return;

            // 첫 연결(아직 연속 중 아님)
            if(CheckFirst())
            {
                currentId = endPoint.id;
                usedLine[new Vector2Int(startPoint.id, endPoint.id)].Add(); // 라인 활성화
                startPoint = endPoint; // 다음 시작점으로 이동
                mouseLine.SetPosition(0, startPoint.Position);
                mouseLine.SetPosition(1, startPoint.Position);
            }
            // 연속 연결(현재 이어가는 포인트에서만 인정)
            if(CheckSecond())
            {
                currentId = endPoint.id;
                usedLine[new Vector2Int(startPoint.id, endPoint.id)].Add();
                CheckWin(); // 클리어 확인
                startPoint = endPoint;
                mouseLine.SetPosition(0, startPoint.Position);
                mouseLine.SetPosition(1, startPoint.Position);
            }
        }
        // 마우스 업: 라인 숨기고 클리어 확인
        else if (Input.GetMouseButtonUp(0))
        {
            mouseLine.gameObject.SetActive(false);
            startPoint = null;
            endPoint = null;
            CheckWin();
        }
    }

    // 공용: 오디오 페이드아웃
    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }
        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    //시작점과 끝점의 id가 유효한지 확인 후 시작점을 끝점으로 이동하는 함수(첫 연결)
    bool CheckFirst()
    {
        if (currentId != -1) return false; // 이미 연속 연결 중이면 첫 연결로 취급하지 않음
        Vector2Int edge = new Vector2Int(startPoint.id, endPoint.id);
        if (!usedLine.ContainsKey(edge)) return false; // 존재하지 않는 라인
        return true;  
    }

    // 연속 연결(현재 이어가야 하는 포인트에서만 다음 라인 인정)
    bool CheckSecond()
    {
        if(currentId != startPoint.id) return false;

        Vector2Int edge = new Vector2Int(startPoint.id, endPoint.id);
        if(usedLine.TryGetValue(edge, out Line currentEdge))
        {
            if(currentEdge == null || currentEdge.count) return false; // 이미 채운 라인 등
        }
        else return false;

        return true;
    }

    // 리셋 버튼(임시 라인/상태/라인 색상 복구)
    public void ResetGameBtn()
    {
        mouseLine.gameObject.SetActive(false);
        startPoint = null;
        endPoint = null;
        currentId = -1;

        for (int i = 0; i < lines.Count; i++)
            lines[i].Return();

        achieveValue++; // 도전과제(리셋 횟수 기반) 체크용
    }

    //클리어 확인 함수: 모든 라인이 count=true이면 클리어
    private void CheckWin()
    {
        foreach (var item in usedLine)
        {
            if (!item.Value.count) return;
        }

        hasGameFinished = true;

        // 나니노벨 변수로 클리어 전달
        var clearVariable = Engine.GetService<ICustomVariableManager>();
        clearVariable.TrySetVariableValue(clearLinePuzzleCount, true);

        StartCoroutine(GameFinishFirst()); // 1차 연출 시작
    }

    //클리어 후 재생하는 씬(1차 연출)
    private IEnumerator GameFinishFirst()
    {
        cleanBtn.interactable = false;
        StartCoroutine(FadeOut(audioSource , 5f)); // 배경 사운드 서서히 끄기
        finishAnimFirst.SetBool("Finish" , true);
        yield return new WaitForSeconds(2f);
        finishGameFirst = true; // 1차 퍼센트 시작
    }

    // 2차 연출
    private IEnumerator GameFinishSecond()
    {
        finishAnim.SetBool("Finish",true);
        yield return new WaitForSeconds(2.3f);
        finishGameLast = true; // 최종 퍼센트 시작
    }

    // 최종 퍼센트 완료 → 블랙 페이드 → 종료
    private IEnumerator GameFinishPerCentage()
    {
        yield return new WaitForSeconds(0.5f);
        for (float f = 0f; f < 1; f += 0.008f)
        {
            Color c = finishBlack.GetComponent<Image>().color;
            c.a = f;
            finishBlack.GetComponent<Image>().color = c;
            if(c.a >= 0.99f)
            {
                c.a = 1f;
                finishBlack.GetComponent<Image>().color = c;
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        LastGameEnd();
    }

    // 마지막 종료 처리(업적 변수 갱신 후 스크립트 플레이)
    void LastGameEnd()
    {
        var acheive = Engine.GetService<ICustomVariableManager>();
        acheive.TryGetVariableValue<bool>("Unlock15", out var unlock15);
        acheive.TryGetVariableValue<bool>("Unlock16", out var unlock16);

        // 리셋 없이 1트 클리어
        if(achieveValue == 0)
        {
            unlock15 = true;
            acheive.TrySetVariableValue("Unlock15", unlock15);
        }
        // 여러 번 리셋
        else if(achieveValue >=5)
        {
            unlock16 = true;
            acheive.TrySetVariableValue("Unlock16", unlock16);
        }

        var player = Engine.GetService<IScriptPlayer>();
        player.PreloadAndPlayAsync(ScriptName, label: Label).Forget();
    }
}
