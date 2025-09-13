using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Naninovel;

/// <summary>
/// 상점 퍼즐 결과 화면 표시.
/// - 나니노벨 변수(ShopFood) 읽어서 결과 이미지/텍스트 세팅
/// - 다음 스크립트로 이동 버튼 처리
/// </summary>
public class ResultManager : MonoBehaviour
{
    ShopManager shopManager;

    // 결과: 음식 이미지/이름/설명
    public GameObject resultFoodImage; 
    public Sprite[] foodImage;
    public GameObject resultFoodName; 
    public Sprite[] foodName;
    public TMP_Text resultFoodDetail;

    // A 캐릭터 결과(표정/리뷰/스탬프)
    public GameObject aResultFace; 
    public Sprite[] aFace;
    public GameObject aResultReview; 
    public Sprite[] aReview;
    public GameObject aResultStamp; 
    public Sprite[] aStamp;

    // R 캐릭터 결과(표정/리뷰/스탬프)
    public GameObject rResultFace; 
    public Sprite[] rFace;
    public GameObject rResultReview; 
    public Sprite[] rReview;
    public GameObject rResultStamp; 
    public Sprite[] rStamp;

    private string ScriptName = "ShopResultPuzzle";
    private string Label;

    void Start()
    {
        ResultShow();

        // 다음 이동 레이블 획득
        var getLabel = Engine.GetService<ICustomVariableManager>();
        getLabel.TryGetVariableValue<int>("ShopCheck", out var labelWant);
        Label = labelWant.ToString();
    }

    // ShopFood 변수값에 따라 결과 UI 설정
    public void ResultShow()
    {
        var acheive = Engine.GetService<ICustomVariableManager>();
        acheive.TryGetVariableValue<string>("ShopFood", out var cookFood);

        if(cookFood == "noChicken")
        {
            int noChickenInt = 0;
            resultFoodImage.GetComponent<Image>().sprite = foodImage[noChickenInt];
            resultFoodName.GetComponent<Image>().sprite = foodName[noChickenInt];
            resultFoodDetail.text = "noChicken예시답안";

            aResultFace.GetComponent<Image>().sprite = aFace[0];
            aResultReview.GetComponent<Image>().sprite = aReview[noChickenInt];
            aResultStamp.GetComponent<Image>().sprite = aStamp[0];

            rResultFace.GetComponent<Image>().sprite = rFace[1];
            rResultReview.GetComponent<Image>().sprite = rReview[noChickenInt];
            rResultStamp.GetComponent<Image>().sprite = rStamp[1];
        }
        else if(cookFood == "soyGarlic")
        {
            int soyGarlic = 1;
            resultFoodImage.GetComponent<Image>().sprite = foodImage[soyGarlic];
            resultFoodName.GetComponent<Image>().sprite = foodName[soyGarlic];
            resultFoodDetail.text = "soyGarlic예시답안";

            aResultFace.GetComponent<Image>().sprite = aFace[4];
            aResultReview.GetComponent<Image>().sprite = aReview[soyGarlic];
            aResultStamp.GetComponent<Image>().sprite = aStamp[4];

            rResultFace.GetComponent<Image>().sprite = rFace[4];
            rResultReview.GetComponent<Image>().sprite = rReview[soyGarlic];
            rResultStamp.GetComponent<Image>().sprite = rStamp[4];
        }
        else if(cookFood == "soy")
        {
            int soy = 2;
            resultFoodImage.GetComponent<Image>().sprite = foodImage[soy];
            resultFoodName.GetComponent<Image>().sprite = foodName[soy];
            resultFoodDetail.text = "soy예시답안";

            aResultFace.GetComponent<Image>().sprite = aFace[2];
            aResultReview.GetComponent<Image>().sprite = aReview[soy];
            aResultStamp.GetComponent<Image>().sprite = aStamp[2];

            rResultFace.GetComponent<Image>().sprite = rFace[2];
            rResultReview.GetComponent<Image>().sprite = rReview[soy];
            rResultStamp.GetComponent<Image>().sprite = rStamp[2];
        }
        else if(cookFood == "hotSpicy")
        {
            int hotSpicy = 3;
            resultFoodImage.GetComponent<Image>().sprite = foodImage[hotSpicy];
            resultFoodName.GetComponent<Image>().sprite = foodName[hotSpicy];
            resultFoodDetail.text = "hotSpicy예시답안";

            aResultFace.GetComponent<Image>().sprite = aFace[0];
            aResultReview.GetComponent<Image>().sprite = aReview[hotSpicy];
            aResultStamp.GetComponent<Image>().sprite = aStamp[0];

            rResultFace.GetComponent<Image>().sprite = rFace[4];
            rResultReview.GetComponent<Image>().sprite = rReview[hotSpicy];
            rResultStamp.GetComponent<Image>().sprite = rStamp[4];
        }
        else if(cookFood == "spicy")
        {
            int spicy = 4;
            resultFoodImage.GetComponent<Image>().sprite = foodImage[spicy];
            resultFoodName.GetComponent<Image>().sprite = foodName[spicy];
            resultFoodDetail.text = "spicy예시답안";

            aResultFace.GetComponent<Image>().sprite = aFace[2];
            aResultReview.GetComponent<Image>().sprite = aReview[spicy];
            aResultStamp.GetComponent<Image>().sprite = aStamp[2];

            rResultFace.GetComponent<Image>().sprite = rFace[3];
            rResultReview.GetComponent<Image>().sprite = rReview[spicy];
            rResultStamp.GetComponent<Image>().sprite = rStamp[3];
        }
        else if(cookFood == "salt")
        {
            int salt = 5;
            resultFoodImage.GetComponent<Image>().sprite = foodImage[salt];
            resultFoodName.GetComponent<Image>().sprite = foodName[salt];
            resultFoodDetail.text = "salt예시답안";

            aResultFace.GetComponent<Image>().sprite = aFace[3];
            aResultReview.GetComponent<Image>().sprite = aReview[salt];
            aResultStamp.GetComponent<Image>().sprite = aStamp[3];

            rResultFace.GetComponent<Image>().sprite = rFace[2];
            rResultReview.GetComponent<Image>().sprite = rReview[salt];
            rResultStamp.GetComponent<Image>().sprite = rStamp[2];
        }
        else if(cookFood == "strange")
        {
            int strange = 6;
            resultFoodImage.GetComponent<Image>().sprite = foodImage[strange];
            resultFoodName.GetComponent<Image>().sprite = foodName[strange];
            resultFoodDetail.text = "strange예시답안";

            aResultFace.GetComponent<Image>().sprite = aFace[1];
            aResultReview.GetComponent<Image>().sprite = aReview[strange];
            aResultStamp.GetComponent<Image>().sprite = aStamp[1];

            rResultFace.GetComponent<Image>().sprite = rFace[0];
            rResultReview.GetComponent<Image>().sprite = rReview[strange];
            rResultStamp.GetComponent<Image>().sprite = rStamp[0];
        }
    }

    // 결과 확인 다음으로 이동
    public void NextBtnClick()
    {
        var player = Engine.GetService<IScriptPlayer>();
        player.PreloadAndPlayAsync(ScriptName, label: Label).Forget();
    }
}
