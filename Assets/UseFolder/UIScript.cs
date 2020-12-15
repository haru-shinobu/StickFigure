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

    [SerializeField]
    RectTransform[] rect = new RectTransform[4];
    Vector3 Rect_AcivePosition, Rect_ReposePosition;
    Vector3 Rect_AcivePosition_down, Rect_ReposePosition_down;

    Box_PlayerController PSc;

    [SerializeField]
    Outline[] controlls_img_outlines = new Outline[4];
    Color textOutline_Color;
    Color textOutline_Color_half;
    Color textOutline_Color_zero;
    [SerializeField]
    Text tex;
    //コルーチン動作状態
    enum outline_active_state
    {
        
        blinking_active = 0,
        blinking_move = 1,
        change_blink = 2,
        free = 3,
    }
    outline_active_state[] state = new outline_active_state[4];
    IEnumerator[] routine = new IEnumerator[8];
    //橋コルーチン予備チェック
    bool BridgeCorutineFlag = false;
    // Start is called before the first frame update
    void Start()
    {
        // 何故か要素数が受け取れない
        //spriteTimer = Resources.LoadAll<Sprite>("Canvas/Sprites/NChip");
        ImageObj = GameObject.Find("Image_10");
        image_10 = ImageObj.GetComponent<Image>();
        ImageObj = GameObject.Find("Image_01");
        image_01 = ImageObj.GetComponent<Image>();
        PSc = GameObject.FindWithTag("Player").GetComponent<Box_PlayerController>();
        textOutline_Color_zero = textOutline_Color_half = textOutline_Color = controlls_img_outlines[0].effectColor;
        textOutline_Color_half.a /= 2;
        textOutline_Color_zero.a = 0;
        controlls_img_outlines[0].effectColor =
        controlls_img_outlines[1].effectColor =
        controlls_img_outlines[2].effectColor =
        controlls_img_outlines[3].effectColor = textOutline_Color_zero;
        for (int i = 0; i < state.Length; i++)
            state[i] = outline_active_state.change_blink;
        for (int i = 0; i < controlls_img_outlines.Length; i++)
        {
            rect[i] = controlls_img_outlines[i].transform.parent.GetComponent<RectTransform>();
        }
        Rect_AcivePosition = rect[0].localPosition;
        Rect_ReposePosition = Rect_AcivePosition - new Vector3(0, rect[0].sizeDelta.y*2, 0);
        Rect_AcivePosition_down = rect[3].localPosition;
        Rect_ReposePosition_down = Rect_AcivePosition_down - new Vector3(0, rect[3].sizeDelta.y * 2, 0);
    }

    void Update()
    {
        if (!PSc.GetGrapLing)
        {
            CheckSlideDownFall_UI();
            CheckGrapLingUI();
            CheckBridgeMake_UI();

            judgeUI();
        }
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
        if (!image_10)
        {
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
            switch (PSc.GrapGimmickType())
            {
                case 0:
                    //スイッチなど　ギミック
                    if (state[0] != outline_active_state.blinking_move)
                    state[0] = outline_active_state.blinking_active;
                if (state[2] == outline_active_state.blinking_move)
                    state[2] = outline_active_state.change_blink;
                    break;
                case 1:
                    //不透過壁
                    if (state[0] == outline_active_state.blinking_move)
                        state[0] = outline_active_state.change_blink;
                    if (state[2] == outline_active_state.blinking_move)
                        state[2] = outline_active_state.change_blink;
                    break;
                case 2:
                    //足場など登攀
                    if (state[0] == outline_active_state.blinking_move)
                        state[0] = outline_active_state.change_blink;
                    if (state[2] != outline_active_state.blinking_move)
                        state[2] = outline_active_state.blinking_active;
                    break;
            }
        }
        else
        {
            if (state[0] == outline_active_state.blinking_move)
                state[0] = outline_active_state.change_blink;
            if (state[2] == outline_active_state.blinking_move)
                state[2] = outline_active_state.change_blink;
        }
    }
    void CheckSlideDownFall_UI()
    {
        if (PSc.CheckSlipDown())
        {
            if (state[3] != outline_active_state.blinking_move)
                state[3] = outline_active_state.blinking_active;
        }
        else
        {
            if (state[3] == outline_active_state.blinking_move)
                state[3] = outline_active_state.change_blink;
        }
    }

    void CheckBridgeMake_UI()
    {
        if (PSc.CheckBridgeMakeAria())
        {
            tex.text = "はしをかける";
            if (state[1] != outline_active_state.blinking_move || !BridgeCorutineFlag)
                state[1] = outline_active_state.blinking_active;
        }
        else
        {
            if (state[1] == outline_active_state.blinking_move)
                state[1] = outline_active_state.change_blink;
            if (PSc.CheckBridgeBaseAria())
            {
                tex.text = "はしを…あれ？";
            }
            else
            {
                tex.text = "はしをかける";
            }
        }
    }
    //====================================================================================
    //アクティブ化切り替えでなく、アルファの濃さで調整すること。
    //ついでに、選択できる間はゆらゆら点滅があるといい。
    //なお、グラップリングに関しては、立ち止まったときに表示されるようにすること？？
    //====================================================================================
    void judgeUI()
    {
        for (int i = 0; i < state.Length; i++)
        {
            switch (state[i])
            {
                case outline_active_state.blinking_active:
                    //消灯コルーチンが動作しているとき消灯停止
                    if (routine[i] == EndBlinking_Line(controlls_img_outlines[i]))
                        StopCoroutine(routine[i]);
                    routine[i] = Blinking_Line(controlls_img_outlines[i]);
                    StartCoroutine(routine[i]);
                    state[i] = outline_active_state.blinking_move;
                    controlls_img_outlines[i].transform.parent.SetAsLastSibling();
                    controlls_img_outlines[1].transform.parent.SetAsLastSibling();
                    if (i == 3)
                    {
                        if (routine[state.Length + i] == NonActivePoint(rect[i], true))
                            StopCoroutine(routine[state.Length + i]);
                        routine[state.Length + i] = ActivePoint(rect[i], true);
                        StartCoroutine(routine[state.Length + i]);
                    }
                    else
                    {
                        if (routine[state.Length + i] == NonActivePoint(rect[i], false))
                            StopCoroutine(routine[state.Length + i]);
                        routine[state.Length + i] = ActivePoint(rect[i], false);
                        StartCoroutine(routine[state.Length + i]);
                    }
                    break;

                case outline_active_state.blinking_move:
                    //隠れているか判断し、隠れているなら稼働させる
                    if (rect[i].localPosition.y <= Rect_ReposePosition.y || rect[i].localPosition.y <= Rect_ReposePosition_down.y)
                        state[i] = outline_active_state.blinking_active;
                    break;

                case outline_active_state.change_blink:
                    //点滅コルーチンが動作しているので点滅停止
                    if (routine[i] == Blinking_Line(controlls_img_outlines[i]))
                        StopCoroutine(routine[i]);
                    routine[i] = EndBlinking_Line(controlls_img_outlines[i]);
                    StartCoroutine(routine[i]);
                    state[i] = outline_active_state.free;
                    if (i == 3)
                    {
                        if (routine[state.Length + i] == ActivePoint(rect[i], true))
                            StopCoroutine(routine[state.Length + i]);
                        routine[state.Length + i] = NonActivePoint(rect[i], true);
                        StartCoroutine(routine[state.Length + i]);
                    }
                    else
                    {
                        if (routine[state.Length + i] == ActivePoint(rect[i], false))
                            StopCoroutine(routine[state.Length + i]);
                        routine[state.Length + i] = NonActivePoint(rect[i], false);
                        StartCoroutine(routine[state.Length + i]);
                    }
                    break;

                case outline_active_state.free:
                    break;
            }
        }
    }
    IEnumerator Blinking_Line(Outline line)
    {
        float timer = 0;
        line.effectColor = textOutline_Color;
        bool timer_flag = true;
        while (true)
        {
            yield return new WaitForEndOfFrame();
            timer += (timer_flag) ? Time.deltaTime : -Time.deltaTime;
            if (timer <= 0 || 1 <= timer) timer_flag = !timer_flag;
            line.effectColor = Color.Lerp(textOutline_Color, textOutline_Color_half, timer);
        }
    }
    IEnumerator EndBlinking_Line(Outline line)
    {
        Color nowColor = line.effectColor;
        float timer = 0;
        var clearcolor = textOutline_Color;
        clearcolor.a = 0;
        while (timer < 1)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            line.effectColor = Color.Lerp(nowColor, clearcolor, timer);
        }
    }
    //アイコン上下移動
    IEnumerator ActivePoint(RectTransform target, bool type)
    {
        var img = target.transform.GetComponent<Image>();
        Color colo = img.color;
        colo.a = 255;
        img.color = colo;
        float timer = 0;
        while (timer < 1)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime * 5;
            if (type)
                target.localPosition = Vector3.Lerp(Rect_ReposePosition_down, Rect_AcivePosition_down, timer);
            else
                target.localPosition = Vector3.Lerp(Rect_ReposePosition, Rect_AcivePosition, timer);
        }
        if(target == rect[1]) BridgeCorutineFlag = true;
    }
    IEnumerator NonActivePoint(RectTransform target,bool type)
    {
        var img = target.transform.GetComponent<Image>();
        float timer = 0;
        while (timer < 1)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            img.color = new Color32(255, 255,255, (byte)((1-timer)*255));
            if (type)
                target.localPosition = Vector3.Lerp(Rect_AcivePosition_down, Rect_ReposePosition_down, timer);
            else
                target.localPosition = Vector3.Lerp(Rect_AcivePosition, Rect_ReposePosition, timer);
        }
        if (target == rect[1]) BridgeCorutineFlag = false;
    }
}
