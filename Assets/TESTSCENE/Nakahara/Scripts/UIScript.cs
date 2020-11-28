using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    private GameObject ImageObj;

    private Image image_10;
    private Image image_01;

    [SerializeField]
    Sprite[] spritePrefab;

    int _inum;

    Box_PlayerController PSc;
    bool _bholdcheck = false;
    Text tex;
    // Start is called before the first frame update
    void Start()
    {
           //_PManager = PlayerMObj.GetComponent<PlayerManager>();
           // 何故か要素数が受け取れない
           //spriteTimer = Resources.LoadAll<Sprite>("Canvas/Sprites/NChip");
           ImageObj = GameObject.Find("Image_10");
        image_10 = ImageObj.GetComponent<Image>();
        ImageObj = GameObject.Find("Image_01");
        image_01 = ImageObj.GetComponent<Image>();
        PSc = GameObject.FindWithTag("Player").GetComponent<Box_PlayerController>();
        tex = PSc.transform.parent.GetChild(2).GetChild(0).GetComponent<Text>();
    }

    void Update()
    {
        tex.text = "";
        CheckSlideDownFall_UI();
        CheckGrapLingUI();
        CheckBridgeMake_UI();
    }

    private void SetObj()
    {
        ImageObj = GameObject.Find("Image_10");
        image_10 = ImageObj.GetComponent<Image>();
        ImageObj = GameObject.Find("Image_01");
        image_01 = ImageObj.GetComponent<Image>();
    }


    // 一定フレームで処理
    public void ChangeNum(int nNum)
    {
        if (image_10)
            Debug.Log("true");
        else
        {
            Debug.Log("false");
            SetObj();
        }
        _inum = nNum;
        image_10.sprite = spritePrefab[nNum / 10];
        image_01.sprite = spritePrefab[nNum % 10];
    }
    
    void CheckGrapLingUI()
    {
        //グラップリングできるタイミング
        if (PSc.OnGroundGraplingJudge())
        {
            //グラップリング種
            if (PSc.GrapGimmickType())
            {
                //スイッチなど　ギミック
                tex.text += " Attack";
            }
            else
            {
                //足場など登攀
                tex.text += " Grap";
            }
        }
    }
    void CheckSlideDownFall_UI()
    {
        if (PSc.CheckSlipDown())
        {
            tex.text += " ↓";
        }
    }
    
    void CheckBridgeMake_UI()
    {
        if (!_bholdcheck)
        {
            if (PSc.CheckBridgeMakeAria())
                tex.text += " ！";
        }
        else
            if (!PSc.CheckBridgeBaseAria())
            tex.text += " ？";
    }
}
