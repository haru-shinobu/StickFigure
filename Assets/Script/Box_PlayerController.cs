using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_PlayerController : MonoBehaviour
{
    [SerializeField, Range(0.2f, 0.5f)] float MoveRange = 0.4f;
    [SerializeField, Range(0.1f, 10)] float DropSpeed = 1;

    Inputmanager inputer;
    Vector3 NowPlayerMovePoint, OldPlayerMovePoint, horizontalPlayerMovePoint;
    int MoveCount = 0;
    [SerializeField] int FlameCount = 10;

    // プレイヤーのグラップリング時に使うタイム数値
    [SerializeField] private float fInitDeepTime = 0.2f;
    [SerializeField] private float fDestDeepTime_Normal_Wait = 0.8f;
    [SerializeField] private float fDestDeepTime_Normal_Interval = 0.08f;
    [SerializeField] private float fDestDeepTime_Button_Wait = 0.1f;
    [SerializeField] private float fDestDeepTime_Button_Interval = 0.07f;

    [SerializeField] GameManager gameManager;

    bool _bControll = false;
    bool _bBridgeMaking = false;
    bool OnBridge = false;
    bool bStandGround = false;

    enum BridgeAriaState
    {
        ignore = 0,
        action = 1,
    }
    BridgeAriaState bridgestate = BridgeAriaState.ignore;
    bool GrapLing = false;
    private float offset = 0.05f;
    //プレイヤーの縦横の半分を記録
    Vector2 Player_verticalhorizontal;
    ///プレイヤーの縦横移動範囲設定、壁移動のたび再記録
    Vector3 MoveAriaLeftTop, MoveAriaRightBottom;
    ///箱回転枠範囲設定、壁移動のたび再記録
    Vector3 RollAriaLeftTop, RollAriaRightBottom;
    ///橋の縦横移動範囲設定、橋かけるたび再記録
    Vector3 BridgeAriaLeftTop, BridgeAriaRightBottom;

    [SerializeField, Header("橋Prefab")]
    GameObject Bridge;

    [SerializeField, Header("着地煙")]
    ParticleSystem[] groundSmokes = new ParticleSystem[2];
    enum FollState
    {
        Foll,
        Folling,
        Stand,
        Landing,
    }
    FollState follState = FollState.Foll;
    private string bridgetag = "BridgeBase";
    Vector3 checkBridgeAriaLT, checkBridgeAriaRB;
    CameraManager camM;
    /// <summary>
    /// 生成橋
    /// </summary>
    GameObject BridgeObj;

    //現在いる箱
    SideColorBoxScript sidebox;
    GameData G_Data;
    GameObject nextBase;
    Rigidbody rb;

    private int nDCount = 5;

    //public GameObject[] gDeepObj = new GameObject[5];
    [SerializeField] GameObject gIDeep;
    GameObject[] Deeplist;
    [SerializeField] int listnum = 15;
    Quaternion quaternion_0 = new Quaternion(0, 0, 0, 0);

    private SoundManager SoundObj;

    enum GrapType
    {
        NormalGrap,
        Button
    }

    ParticleSystem FootStamp;

    [SerializeField] Animator[] aChildAnim = new Animator[4];

    enum SideRedLine
    {
        T, B, L, R, Non,
    }
    SideRedLine RedSide = SideRedLine.Non;
    IEnumerator nowIE;

    enum SideState
    {
        left_wall,
        normal,
        right_wall,
        left_right_wall,
        bottom_wall,
        right_bottom_wall,
        left_bottom_wall,
        left_right_bottom_wall,
    }
    SideState sideState = SideState.normal;
    public void sidewallstate(int value)
    {
        switch (value)
        {
            case 0:
                sideState = SideState.normal;
                break;
            case 1:
                if (sideState == SideState.left_wall)
                    sideState = SideState.left_right_wall;
                else
                    sideState = SideState.right_wall;
                break;
            case -1:
                if (sideState == SideState.right_wall)
                    sideState = SideState.left_right_wall;
                else
                    sideState = SideState.left_wall;
                break;
            case 2:
                if (sideState == SideState.right_wall)
                    sideState = SideState.right_bottom_wall;
                else
                if (sideState == SideState.left_wall)
                    sideState = SideState.left_bottom_wall;
                else
                if (sideState == SideState.left_right_wall)
                    sideState = SideState.left_right_bottom_wall;
                else
                    sideState = SideState.bottom_wall;
                break;
        }
    }
    public bool TopCollidersideState = false;
    //グラップリングできるかどうか
    bool GraplingJudge = false;

    [SerializeField]
    private int AnimCountSet = 25;
    private int AnimCount = 25;

    void Start()
    {
        rb = transform.parent.GetComponent<Rigidbody>();

        var sprvec = transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
        Player_verticalhorizontal = sprvec.bounds.extents;
        camM = Camera.main.GetComponent<CameraManager>();
        G_Data = GameObject.FindWithTag("BoxManager").GetComponent<GameData>();
        if (gameManager)
            gameManager = GameObject.FindWithTag("BoxManager").GetComponent<GameManager>();
        NowPlayerMovePoint = transform.parent.position;
        FootStamp = transform.parent.GetChild(1).GetComponent<ParticleSystem>();
        horizontalPlayerMovePoint = Vector3.zero;
        inputer = GameObject.FindWithTag("BoxManager").GetComponent<Inputmanager>();
        GameObject soundtarget = GameObject.Find("SoundObj");
        if (soundtarget)
            SoundObj = soundtarget.GetComponent<SoundManager>();
        Deeplist = new GameObject[listnum];
        Vector3 DeepPos;
        for (int i = 0; i < listnum; i++)
        {
            DeepPos = new Vector3(this.transform.position.x, this.transform.position.y + (0.75f * i), this.transform.position.z);
            Deeplist[i] = Instantiate(gIDeep, DeepPos, quaternion_0);
        }
    }
    
    void Update()
    {
        //メニュー開いてないとき
        if (!gameManager.b_menu)
        {
            //落下速度制限
            Vector3 vel = rb.velocity;
            if (vel.y < -DropSpeed * 10)
            {
                vel.y = -DropSpeed * 10;
            }
            if (vel.y < 0)
            {
                //足跡下降用
                var romain = FootStamp.main;
                romain.startRotation = new ParticleSystem.MinMaxCurve(2.967f, 3.316f);
                if (follState == FollState.Landing || follState == FollState.Stand)
                    follState = FollState.Foll;
                //Move_Anim(true);
            }
            else
            {
                if (follState != FollState.Stand)
                    follState = FollState.Landing;
                //Move_Anim(false);
            }
            rb.velocity = vel;
            if (!_bBridgeMaking)
            {
                if (Moving)
                {
                    CheckOnGround();
                    // プレイヤー移動範囲チェック
                    //橋の上でないとき
                    if (!CheckMoveBridgeAria())
                    {
                        //グラップリングしていないとき
                        if (!GrapLing)
                        {
                            //箱回転範囲を出たとき
                            if (CheckRollAria())
                            {
                                this.Moving = false;
                                var T = transform.parent.position.y + Player_verticalhorizontal.y;
                                var B = transform.parent.position.y - Player_verticalhorizontal.y;
                                var L = transform.parent.position.x - Player_verticalhorizontal.x;
                                var R = transform.parent.position.x + Player_verticalhorizontal.x;
                                GameObject wind = null;
                                if (G_Data.WindPrefab)
                                    wind = Instantiate(G_Data.WindPrefab, sidebox.transform.position, Quaternion.identity);
                                //上下左右
                                var rollways = 0;
                                if (T >= Front_LeftTop.y)
                                {
                                    rollways = 1;
                                    if (wind)
                                        wind.transform.rotation = Quaternion.Euler(0, 0, 90);
                                }
                                if (B <= Front_RightBottom.y)
                                {
                                    rollways = 2;
                                    if (wind)
                                        wind.transform.rotation = Quaternion.Euler(0, 0, -90);
                                }
                                if (L <= Front_LeftTop.x)
                                {
                                    rollways = 3;
                                    if (wind)
                                        wind.transform.rotation = Quaternion.Euler(0, 0, 180);
                                }
                                if (R >= Front_RightBottom.x)
                                {
                                    rollways = 4;
                                    if (wind)
                                        wind.transform.rotation = Quaternion.Euler(0, 0, 0);
                                }
                                if (BridgeObj)
                                {
                                    if (SoundObj)
                                        SoundObj.PropSE();
                                    BridgeObj.SendMessage("RollDestroy", 2);
                                    BridgeObj.transform.SetParent(null);
                                    BridgeObj = null;
                                    gameManager.CheckBridgeNum();
                                }
                                sidebox.ChangeBoxRoll(rollways);
                            }
                            else
                            {
                                // 一定フレームで処理
                                if (FlameCount <= ++MoveCount)
                                {
                                    MoveCount = 0;
                                    // プレイヤー移動範囲チェック
                                    RePositionMoveAria();
                                    OldPlayerMovePoint = NowPlayerMovePoint;
                                    this.Move(inputer.player_move_input[0], inputer.player_move_input[1]);
                                }
                                else
                                {
                                    RePositionMoveAria();
                                    NowPlayerMovePoint.y = transform.parent.position.y;
                                    NowPlayerMovePoint += (horizontalPlayerMovePoint / FlameCount);
                                    transform.parent.position = NowPlayerMovePoint;
                                }
                            }

                            if (inputer.player_jump_input)
                            {
                                bridgestate = BridgeAriaState.action;
                                //橋の判定など
                                MakeBridgeCheck();
                            }
                        }
                    }
                    else
                    //橋の上のとき
                    {
                        if (Moving)
                        {
                            this.Moving = false;
                            nowIE = InBridge();
                            StartCoroutine(nowIE);
                        }
                    }
                }
                else
                {
                    //プレイヤーが操作できないときは落ちない
                    rb.velocity = Vector3.zero;
                }
            }
        }
    }

    //移動範囲確認(箱)
    void RePositionMoveAria()
    {
        var pos = NowPlayerMovePoint;
        var T = pos.y + Player_verticalhorizontal.y;
        var B = pos.y - Player_verticalhorizontal.y;
        var R = pos.x + Player_verticalhorizontal.x;
        var L = pos.x - Player_verticalhorizontal.x;

        if (Move_Aria_FLT.x >= L) NowPlayerMovePoint += Vector3.right * MoveRange;
        if (R >= Move_Aria_FRB.x) NowPlayerMovePoint -= Vector3.right * MoveRange;
        if (T >= Move_Aria_FLT.y) NowPlayerMovePoint -= Vector3.up * MoveRange;
        if (Move_Aria_FRB.y == Front_RightBottom.y)
        {
            if (Move_Aria_FRB.y + G_Data.RedLine / 2 >= B)
            {
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }
        else
        {
            if (Move_Aria_FRB.y >= B)
            {
                NowPlayerMovePoint = transform.parent.position += new Vector3(0, Mathf.Abs(Move_Aria_FRB.y - B));
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }
    }

    //回転始動範囲(箱)
    bool CheckRollAria()
    {
        var pos = transform.parent.position;
        var T = pos.y + Player_verticalhorizontal.y;
        var B = pos.y - Player_verticalhorizontal.y;
        var R = pos.x + Player_verticalhorizontal.x;
        var L = pos.x - Player_verticalhorizontal.x;


        if (B < Move_Aria_FRB.y + MoveRange)
        {
            bStandGround = true;
            var vely = rb.velocity;
            if (vely.y > 0.1) vely.y = 0.1f; else vely.y = 0;
            rb.velocity = vely;
            Debug.Log(rb.velocity);
        }
        else
        {
            rb.isKinematic = false;
            bStandGround = false;
        }

        if (Front_LeftTop.x < L && R < Front_RightBottom.x)
            if (Front_RightBottom.y < B && T < Front_LeftTop.y)
            {
                return false;
            }
        if (sideState == SideState.right_wall ||
            sideState == SideState.left_right_wall ||
            sideState == SideState.right_bottom_wall ||
            sideState == SideState.left_right_bottom_wall)
        {
            if (R > Front_RightBottom.x) return false;
        }
        else if (sideState == SideState.left_wall ||
            sideState == SideState.left_right_wall ||
            sideState == SideState.left_bottom_wall ||
            sideState == SideState.left_right_bottom_wall)
        {
            if (Front_LeftTop.x > L) return false;
        }
        return true;
    }


    //移動範囲確認(橋)
    bool CheckMoveBridgeAria()
    {
        if (BridgeObj)
        {
            if (bridgestate == BridgeAriaState.action)
            {
                if (BridgeObj.transform.forward == Vector3.forward || BridgeObj.transform.forward == -Vector3.forward)
                {
                    if (CheckInBridgeAria())
                    {
                        NowPlayerMovePoint = transform.parent.position;
                        camM.Bridge = OnBridge = true;
                        return OnBridge;
                    }
                }
            }
        }
        camM.Bridge = OnBridge = false;
        return OnBridge;
    }

    bool CheckInBridgeAria()
    {
        var pos = transform.parent.position;
        var T = pos.y + Player_verticalhorizontal.y;
        var B = pos.y - Player_verticalhorizontal.y;
        var R = pos.x + Player_verticalhorizontal.x;
        var L = pos.x - Player_verticalhorizontal.x;
        //範囲内に触れたとき（プレイヤーが橋の上にいる）
        if (BridgeAriaLT.x < R && L < BridgeAriaBR.x)
            if (BridgeAriaBR.y < T && B < BridgeAriaLT.y)
                return true;
        return false;
    }
    //辺の枠範囲判定
    /// <summary>
    /// OnRedLine = true;
    /// </summary>
    public bool CheckRedAria()
    {
        //G_Data.RedLineは赤線の幅
        if (Front_LeftTop.x + G_Data.RedLine <= transform.parent.position.x - Player_verticalhorizontal.x && transform.parent.position.x + Player_verticalhorizontal.x <= Front_RightBottom.x - G_Data.RedLine)
            if (Front_RightBottom.y + G_Data.RedLine <= transform.parent.position.y - Player_verticalhorizontal.y && transform.parent.position.y + Player_verticalhorizontal.y <= Front_LeftTop.y - G_Data.RedLine)
            {
                RedSide = SideRedLine.Non;
                return false;
            }
        //左右を優先
        if (Front_LeftTop.x + G_Data.RedLine > transform.parent.position.x - Player_verticalhorizontal.x)
            RedSide = SideRedLine.L;
        else if (transform.parent.position.x + Player_verticalhorizontal.x > Front_RightBottom.x - G_Data.RedLine)
            RedSide = SideRedLine.R;
        else
        if (Front_RightBottom.y + G_Data.RedLine > transform.parent.position.y - Player_verticalhorizontal.y)
            RedSide = SideRedLine.B;
        else if (transform.parent.position.y - Player_verticalhorizontal.y > Front_LeftTop.y - G_Data.RedLine)
            RedSide = SideRedLine.T;
        return true;
    }
    //=======================================================================
    // 移動アニメーション
    //=======================================================================
    public void Move_Anim(bool flag)
    {
        foreach (Animator x in aChildAnim)
        {
            if (x.GetBool("work") != flag)
            {
                //Debug.Log("Anim" + flag);
                x.SetBool("work", flag);
            }
        }
    }

    //=======================================================================
    // プレイヤー移動
    //=======================================================================
    //this->
    void Move(float horizontal, float vartical)
    {
        horizontalPlayerMovePoint = Vector3.zero;
        if (horizontal > 0)
        {
            horizontalPlayerMovePoint = Vector3.right * MoveRange;
            //画像反転用
            transform.GetChild(0).localScale = Vector3.one;
            //足跡反転用
            var romain = FootStamp.main;
            romain.startRotation = new ParticleSystem.MinMaxCurve(1.396f, 1.745f);
            // サウンド再生
            if (AnimCount++ > AnimCountSet)
            {
                if (SoundObj)
                    SoundObj.MoveSE();
                AnimCount = 0;
            }
            //アニメーション
            Move_Anim(true);

        }
        else
        if (horizontal < 0)
        {
            horizontalPlayerMovePoint = -Vector3.right * MoveRange;
            //画像反転用
            transform.GetChild(0).localScale = new Vector3(-1, 1, 1);
            //足跡反転用
            var romain = FootStamp.main;
            romain.startRotation = new ParticleSystem.MinMaxCurve(-1.396f, -1.745f);
            // サウンド再生
            if (AnimCount++ > AnimCountSet)
            {
                if (SoundObj)
                    SoundObj.MoveSE();
                AnimCount = 0;
            }
            //アニメーション
            Move_Anim(true);
        }
        else
        {
            Move_Anim(false);
        }
        //else if (horizontal == 0 || vartical == 0)
        //    Move_Anim(false);

        if (BridgeObj)
            // 橋渡り終えたときの再判定による帰還バグ対策用
            if (bridgestate == BridgeAriaState.ignore && !CheckInBridgeAria())
                bridgestate = BridgeAriaState.action;
        //---------------------------------------------------------
        //**      ** 下を押したときの処理はここに描く    **      **
        //---------------------------------------------------------
        if (vartical < 0)
        {
            bool slide = false;
            //すり抜けられる床かどうか
            foreach (GameObject ground in sidebox.BoxInGround)
            {
                //正面に来ている地面のみ
                if (ground.transform.position.z <= Front_LeftTop.z)
                {
                    //画像の縦横をとり、地面が横長のとき、グラップリング対象とする
                    Vector3 exvec = ground.GetComponent<SpriteRenderer>().bounds.extents;
                    if (exvec.x > exvec.y)
                    {
                        //地面幅のなかにプレイヤーが存在し、なおかつその地面の上にプレイヤーがいるなら
                        if (ground.transform.position.x - exvec.x < transform.parent.position.x && transform.parent.position.x < ground.transform.position.x + exvec.x)
                        {
                            if (transform.parent.position.y - Player_verticalhorizontal.y - 0.1f < ground.transform.position.y + exvec.y &&
                                ground.transform.position.y < transform.parent.position.y)
                            {
                                ground.GetComponent<GroundScript>().SlipDown();
                                slide = true;
                            }
                        }
                    }
                }
            }

            if (!slide)
            {
                if (CheckRedAria())
                {
                    bool bflag = false;

                    GameObject obj = GetPlayerInBridgeBase();
                    if (obj)
                    {
                        bflag = true;
                        obj.SendMessage("SlipdroundLine");
                        if (BridgeObj)
                        {
                            var bridgebaseline = obj.transform.GetComponent<SpriteRenderer>().bounds.extents;
                            Vector3 _vec = new Vector3(
                                -Mathf.Abs(bridgebaseline.x),
                                 Mathf.Abs(bridgebaseline.y),
                                 Mathf.Abs(bridgebaseline.z));
                            Vector3 FLT = obj.transform.position + _vec;
                            Vector3 BRB = obj.transform.position - _vec;

                            if (FLT.x > BRB.x)
                            {
                                float sub = BRB.x;
                                BRB.x = FLT.x;
                                FLT.x = sub;
                            }
                            if (FLT.y < BRB.y)
                            {
                                float sub = BRB.y;
                                BRB.y = FLT.y;
                                FLT.y = sub;
                            }
                            if (FLT.z < BRB.z)
                                BRB.z = FLT.z;
                            else
                                FLT.z = BRB.z;
                            Vector3 checkpos = new Vector3(BridgeObj.transform.position.x, BridgeAriaLT.y, BridgeObj.transform.position.z);
                            if (FLT.x <= checkpos.x && checkpos.x <= BRB.x)
                                if (BRB.y <= checkpos.y && checkpos.y <= FLT.y)
                                    BridgeObj.GetComponent<bridgeScript>().second_NoCollider();
                        }
                    }

                    if (!bflag)
                        if (BridgeObj)
                            BridgeObj.GetComponent<bridgeScript>().second_NoCollider();
                }
                else
                    if (BridgeObj)
                    BridgeObj.GetComponent<bridgeScript>().second_NoCollider();
            }
            rb.isKinematic = false;
        }
    }
    //=======================================================================
    // プレイヤー壁当たり
    //=======================================================================
    public void playerMovePoint(Vector3 value)
    {
        NowPlayerMovePoint = (transform.parent.position + value);
    }
    //=======================================================================
    // 橋ベース(プレイヤーが内にいる橋ベースを返す)ないならnull
    //=======================================================================
    GameObject GetPlayerInBridgeBase()
    {
        //前面にある橋のベースを探索
        for (int i = 0; i < sidebox.GetBridgeLine.Length; i++)
        {
            if (sidebox.GetBridgeLine[i].tag == bridgetag)
            {
                if (sidebox.GetBridgeLine[i].transform.position.z <= Front_LeftTop.z)
                {
                    var bridgebaseline = sidebox.GetBridgeLine[i].transform.GetComponent<SpriteRenderer>().bounds.extents;

                    Vector3 _vec = new Vector3(
                        -Mathf.Abs(bridgebaseline.x),
                         Mathf.Abs(bridgebaseline.y),
                         Mathf.Abs(bridgebaseline.z));
                    Vector3 FLT = sidebox.GetBridgeLine[i].transform.position + _vec;
                    Vector3 BRB = sidebox.GetBridgeLine[i].transform.position - _vec;
                    //念のため
                    if (FLT.x > BRB.x)
                    {
                        var sub = BRB.x;
                        BRB.x = FLT.x;
                        FLT.x = sub;
                    }

                    if (FLT.y < BRB.y)
                    {
                        var sub = BRB.y;
                        BRB.y = FLT.y;
                        FLT.y = sub;
                    }

                    if (FLT.z < BRB.z)
                        BRB.z = FLT.z;
                    else
                        FLT.z = BRB.z;


                    var pos = transform.parent.position;
                    float T = pos.y + Player_verticalhorizontal.y;
                    float B = pos.y - Player_verticalhorizontal.y;
                    float R = pos.x + Player_verticalhorizontal.x;
                    float L = pos.x - Player_verticalhorizontal.x;

                    //範囲内のとき
                    if (FLT.x <= R && L <= BRB.x)
                        if (BRB.y <= T && B <= FLT.y)
                            return sidebox.GetBridgeLine[i].gameObject;
                }
            }
        }
        return null;
    }
    //=======================================================================
    // Groundの上にいるときそのGroundを返す(プレイヤーが上に居ないならnull)
    //=======================================================================
    GameObject GetPlayerOnGround()
    {
        foreach (GameObject ground in sidebox.BoxInGround)
        {
            //画像の縦横をとり、地面が横広のとき、グラップリング対象とする
            Vector3 exvec = ground.GetComponent<SpriteRenderer>().bounds.extents;
            //正面に来ている足場のみ
            if (ground.transform.position.z <= Front_LeftTop.z)
            {
                if (exvec.x > exvec.y)
                {
                    //足場幅のなかにプレイヤーが存在するなら
                    if (ground.transform.position.x - exvec.x < transform.parent.position.x && transform.parent.position.x < ground.transform.position.x + exvec.x)
                    {
                        //その足場の上にプレイヤーがいるなら
                        if (transform.parent.position.y - Player_verticalhorizontal.y - 0.1f < ground.transform.position.y + exvec.y
                            && ground.transform.position.y < transform.parent.position.y)
                            return ground;
                    }
                }
            }
        }
        return null;
    }
    //=======================================================================
    // 橋
    //=======================================================================
    //this->
    public void MakeBridgeCheck()
    {
        CheckRedAria();
        GameObject target = (RedSide != SideRedLine.Non) ? GetPlayerInBridgeBase() : null;

        //橋のベースの範囲内にプレイヤーがいたとき、そのベースがnullでなくなる
        if (target != null)
        {
            //取得全ベースから現在の箱と同じ箱についている橋のベース以外で一番近い橋Baseを得る
            nextBase = null;
            float distance = float.MaxValue;
            Vector3 posA = target.transform.position;

            Color colord = target.GetComponent<SpriteRenderer>().color;
            foreach (GameObject obj in G_Data.Bases)
            {
                //親箱が同じかどうか
                if (sidebox.transform.root == obj.transform.root)
                    continue;

                //色とレイヤーが異なるかどうか
                if (colord != obj.GetComponent<SpriteRenderer>().color || target.layer != obj.layer)
                    continue;


                Vector3 posB = obj.transform.position;
                //橋ベースがy軸方向の方が長いかどうか
                if (target.transform.localScale.x < target.transform.localScale.y)
                    posB.y = posA.y = 0;//longY
                else
                    posB.x = posA.x = 0;//longX

                float dis = Mathf.Abs(Vector3.Distance(posA, posB));

                //橋ベースのうち、最も近い橋ベースを探索
                if (dis < distance)
                {
                    //playerがその橋ベースの幅内にあるとき。
                    Vector3 obj_extents = obj.transform.GetComponent<SpriteRenderer>().bounds.extents;
                    var pos = obj.transform.position;
                    if (
                    (pos.x - obj_extents.x < NowPlayerMovePoint.x && NowPlayerMovePoint.x < pos.x + obj_extents.x) ||
                    (pos.y - obj_extents.y < NowPlayerMovePoint.y && NowPlayerMovePoint.y < pos.y + obj_extents.y))
                    {
                        distance = dis;
                        nextBase = obj;
                    }
                }
            }
            if (nextBase != null)
            {
                //橋基地同士の距離が橋以下のとき
                Vector3 bounds = Bridge.GetComponent<MeshRenderer>().bounds.extents * 2;
                float bridgedistance = (bounds.x > bounds.y) ? bounds.x : bounds.y;//橋の長さは長いほうをとる
                bridgedistance += 0.001f;
                Vector3 vecter = target.transform.position - nextBase.transform.position;
                Vector3 bounds_target = target.GetComponent<SpriteRenderer>().bounds.extents;
                //橋ベースがy軸方向の方が長いかどうか
                if (bridgedistance > ((bounds_target.x < bounds_target.y) ? Mathf.Abs(vecter.x) : Mathf.Abs(vecter.y)))
                    //生成距離比較
                    if (bridgedistance >= distance)
                    {
                        if (gameManager.nDCountCheck())
                            MakeBridge(target, nextBase);
                    }
                    else
                        if (target)//橋基地同士の距離が橋以上のとき
                        GrapLinger();
            }
            else
                    if (target)//橋基地相方が見つからないとき
                GrapLinger();
        }

        //ターゲット橋ベースが無い場合。グラップリング用
        if (target == null)
        {
            GrapLinger();
        }
    }


    //this.MakeBridgeCheck()->
    void MakeBridge(GameObject prevBB, GameObject nextBB)
    {
        //各生成場所セット
        Vector3 _vec = transform.parent.position;
        float _Angle = 0f;

        //プレイヤー位置を基準とするのではなく、赤ラインの半分(0.5f分)を基準とする。(橋の役割とき)
        switch (RedSide)
        {
            case SideRedLine.Non:
                //プレイヤー足元位置を基準として上方向に配置(梯子の役割のとき)
                _vec = transform.parent.position + new Vector3(0, -Player_verticalhorizontal.y);
                break;
            case SideRedLine.T:
                //プレイヤーのいる赤位置判定からの場所と方向決定
                _vec = new Vector3(transform.parent.position.x, Front_LeftTop.y - G_Data.RedLine / 2);
                break;
            case SideRedLine.B:
                //プレイヤーのいる赤位置判定からの場所と方向決定
                _vec = new Vector3(transform.parent.position.x, Front_RightBottom.y + G_Data.RedLine / 2);
                _Angle = 180f;
                break;
            case SideRedLine.L:
                //プレイヤーのいる赤位置判定からの場所と方向決定
                _vec = new Vector3(Front_LeftTop.x + G_Data.RedLine / 2, transform.parent.position.y);
                _Angle = 90f;
                break;
            case SideRedLine.R:
                //プレイヤーのいる赤位置判定からの場所と方向決定
                _vec = new Vector3(Front_RightBottom.x - G_Data.RedLine / 2, transform.parent.position.y);
                _Angle = 270f;
                break;
        }
        //めり込むため位置微調整
        _vec.z = transform.parent.position.z - 0.01f;
        bool skip = false;
        //前回の橋が残っているとき
        if (BridgeObj)
        {
            //前回の橋生成位置が同じ橋ベースのとき
            var Bsc = BridgeObj.GetComponent<bridgeScript>();
            if ((Bsc.BasePrev == nextBB && Bsc.BaseNext == prevBB) ||
                (Bsc.BasePrev == prevBB && Bsc.BaseNext == nextBB))
            {
                //橋位置をプレイヤー位置に対応させる。
                //(渡し側・受け側両方の)橋ベース範囲内にとどめること!

                BridgeObj.transform.position = _vec;
                BridgeObj.transform.rotation = Quaternion.Euler(180, 0, _Angle);
                skip = true;
            }
            else
            {
                if (SoundObj)
                    SoundObj.PropSE();
                //破棄する場合
                BridgeObj.SendMessage("RollDestroy", 2);
                BridgeObj.transform.SetParent(null);
                BridgeObj = null;
                //Destroy(BridgeObj);
                gameManager.CheckBridgeNum();
            }
        }
        if (!skip)
        {
            //橋生成
            InstanceMakeBridge(_vec, _Angle);
            // ゲームオーバーか判定はGaameManagerスクリプトで制御
            gameManager.nDCountDown();
        }
        //--------------------------------------------------------------------

        switch (RedSide)
        {
            case SideRedLine.Non: BridgeObj.transform.position += new Vector3(0, (BridgeObj.GetComponent<MeshRenderer>().bounds.extents.y)); break;
            case SideRedLine.T: BridgeObj.transform.position += new Vector3(0, (BridgeObj.GetComponent<MeshRenderer>().bounds.extents.y)); break;
            case SideRedLine.B: BridgeObj.transform.position -= new Vector3(0, (BridgeObj.GetComponent<MeshRenderer>().bounds.extents.y)); break;
            case SideRedLine.L: BridgeObj.transform.position -= new Vector3((BridgeObj.GetComponent<MeshRenderer>().bounds.extents.x), 0); break;
            case SideRedLine.R: BridgeObj.transform.position += new Vector3((BridgeObj.GetComponent<MeshRenderer>().bounds.extents.x), 0); break;
        }

        if (BridgeObj)
        {
            BridgeObj.transform.SetParent(sidebox.transform);

            MeshRenderer mesh = BridgeObj.transform.GetComponent<MeshRenderer>();

            var BSc = BridgeObj.GetComponent<bridgeScript>();
            BSc.BasePrev = prevBB;
            BSc.BaseNext = nextBB;
            BSc.second_NoCollider();

            var bridgePos = BridgeObj.transform.position;
            //橋の範囲を記録
            _vec = new Vector3(
                -Mathf.Abs(mesh.bounds.extents.x),
                Mathf.Abs(mesh.bounds.extents.y),
                Mathf.Abs(mesh.bounds.extents.z));
            Vector3 FLT = bridgePos + _vec;
            Vector3 BRB = bridgePos - _vec;

            if (FLT.x > BRB.x)
            {
                var sub = BRB.x;
                BRB.x = FLT.x;
                FLT.x = sub;
            }

            if (FLT.y < BRB.y)
            {
                var sub = BRB.y;
                BRB.y = FLT.y;
                FLT.y = sub;
            }

            if (FLT.z < BRB.z)
                BRB.z = FLT.z;
            else
                FLT.z = BRB.z;

            BridgeAriaLT = FLT;
            BridgeAriaBR = BRB;
        }
    }
    //this.MakeBridgeCheck()->
    void GrapLinger()
    {
        if (!GrapLing)
        {
            GameObject land = null;
            GameObject onebridgebase = null;

            foreach (GameObject ground in sidebox.BoxInGround)
            {
                //画像の縦横をとり、地面が横広のとき、グラップリング対象とする
                Vector3 exvec = ground.GetComponent<SpriteRenderer>().bounds.extents;
                //正面に来ている足場のみ
                if (ground.transform.position.z <= sidebox.transform.position.z - exvec.z)
                {
                    if (exvec.x > exvec.y)
                    {
                        //足場幅のなかにプレイヤーが存在するなら
                        if (ground.transform.position.x - exvec.x < NowPlayerMovePoint.x && NowPlayerMovePoint.x < ground.transform.position.x + exvec.x)
                        {

                            if (NowPlayerMovePoint.y < ground.transform.position.y)
                            {
                                if (land == null)
                                    land = ground;
                                else
                                if (land.transform.position.y > ground.transform.position.y)
                                    land = ground;
                            }
                        }
                    }
                }
            }

            //間に足場がないとき
            if (land == null)
            {
                foreach (GameObject obj in sidebox.GetBridgeLine)
                {
                    Vector3 exvec = obj.GetComponent<SpriteRenderer>().bounds.extents;
                    if (obj.transform.position.z <= sidebox.transform.position.z - exvec.z && exvec.x > exvec.y)
                    {
                        if (transform.parent.position.y < obj.transform.position.y)
                        {
                            if (obj.transform.position.x - exvec.x < transform.parent.position.x && transform.parent.position.x < obj.transform.position.x + exvec.x)
                            {
                                onebridgebase = obj;//足場の中で最も近い足場を得ている
                            }
                        }
                    }
                }
            }


            //グラップリング処理
            //ただし移動できない壁後付けするため注意。
            if (rb.isKinematic == true || rb.velocity.y == 0 || OnGroundGraplingJudge() || bStandGround)
            {
                //足跡上向き用
                var romain = FootStamp.main;
                romain.startRotation = new ParticleSystem.MinMaxCurve(-0.1746f, 0.1746f);


                Vector3 pos, pos1, pos2, pos3, pos4, pos5;
                pos = pos1 = pos2 = pos3 = pos4 = pos5 = Front_LeftTop;
                //不透過壁があるか否か
                //存在しないとき箱左上で返す
                pos = sidebox.sideWall();
                float a = pos.y;

                //回転スイッチの位置(仮)
                //存在しないとき箱左上で返す
                pos1 = sidebox.FindBoxRollerSwitch();
                if (a > pos1.y && pos1.y != Front_LeftTop.y) a = pos1.y;

                //橋の下限の位置
                if (BridgeObj)
                {
                    pos2 = BridgeObj.transform.position;
                    var ran = BridgeObj.GetComponent<MeshRenderer>().bounds.extents;
                    pos2.y -= ran.y;

                    if (a > pos2.y && transform.parent.position.y < pos2.y)
                        if (BridgeObj.transform.position.x - ran.x < transform.parent.position.x && transform.parent.position.x < BridgeObj.transform.position.x + ran.x)
                            a = pos2.y;
                }
                //足場の位置
                if (land)
                {
                    pos3 = land.transform.position;
                    var ran = land.GetComponent<SpriteRenderer>().bounds.extents;
                    pos3.y -= ran.y;
                    if (land.transform.position.y > transform.parent.position.y)
                        if (a > pos3.y && pos3.y != Front_LeftTop.y) a = pos3.y;
                }
                //橋ベースの位置
                if (onebridgebase)
                {
                    pos4 = onebridgebase.transform.position;
                    var ran = onebridgebase.GetComponent<SpriteRenderer>().bounds.extents;
                    pos4.y -= ran.y;
                    if (a > pos4.y && pos4.y != Front_LeftTop.y)
                        if (transform.parent.position.y < onebridgebase.transform.position.y)
                            if (onebridgebase.transform.position.x - ran.x < transform.parent.position.x && transform.parent.position.x < onebridgebase.transform.position.x + ran.x)
                                a = pos4.y;
                }
                //不透過壁の位置
                //存在しないとき箱左上で返す
                pos5 = sidebox.UnPassWallPos();
                if (a > pos5.y && pos5.y != Front_LeftTop.y) a = pos5.y;


                if (a != Front_LeftTop.y)
                {
                    if (a == pos1.y)
                    {
                        if (sidebox.FindBoxRollerSwitch() != Front_RightBottom)
                        {
                            GameObject wind = Instantiate(G_Data.WindPrefab, sidebox.transform.position, Quaternion.identity);
                            if (sidebox.GetRollSwitchWay())
                                wind.transform.rotation = Quaternion.Euler(-90, 0, 0);
                            else
                                wind.transform.rotation = Quaternion.Euler(90, 0, 0);
                            //箱へアクセス、スイッチの動作指令を送らせる
                            //sidebox.RollSwitchAction();
                            if (SoundObj)
                                SoundObj.PushButtonSE();
                            if (BridgeObj)
                            {
                                BridgeObj.SendMessage("RollDestroy", 2);
                                BridgeObj.transform.SetParent(null);
                                BridgeObj = null;
                                //Destroy(BridgeObj);
                                gameManager.CheckBridgeNum();
                            }

                            this.Moving = false;
                            GrapLing = true;
                            DeepGrap(sidebox.FindBoxRollerSwitch().y - transform.parent.position.y, GrapType.Button, true);
                            nowIE = GrapAttack();
                            StartCoroutine(nowIE);
                            DeepGrap(0.0f, GrapType.Button, false);
                        }
                    }
                    if (a == pos2.y)
                    {
                        //橋があるとき
                        if (BridgeObj)
                        {
                            Vector3 bridgeextent = BridgeObj.GetComponent<MeshRenderer>().bounds.extents;
                            Vector3 Brpos = BridgeObj.transform.position;

                            if (Brpos.x - bridgeextent.x < transform.parent.position.x + Player_verticalhorizontal.x && transform.parent.position.x - Player_verticalhorizontal.y < Brpos.x + bridgeextent.x)
                            {
                                //橋の下限まで伸ばす。
                                DeepGrap(BridgeObj.transform.position.y - transform.parent.position.y, GrapType.NormalGrap, true);
                                nowIE = Graplinger(new Vector3(transform.parent.position.x, BridgeObj.transform.position.y - bridgeextent.y - Player_verticalhorizontal.y, transform.parent.position.z));
                                StartCoroutine(nowIE);
                                DeepGrap(0.0f, GrapType.NormalGrap, false);
                                GrapLing = true;
                            }
                        }
                    }
                    if (a == pos3.y)
                    {
                        //地面があるとき
                        if (land)
                        {
                            if (transform.parent.position.y < land.transform.position.y)
                            {
                                Vector3 exvec = land.GetComponent<SpriteRenderer>().bounds.extents;
                                //他処理を停止してグラップリング移動させる
                                DeepGrap(land.transform.position.y - transform.parent.position.y, GrapType.NormalGrap, true);
                                nowIE = Graplinger(new Vector3(transform.parent.position.x, land.transform.position.y + exvec.y + Player_verticalhorizontal.y, transform.parent.position.z));
                                StartCoroutine(nowIE);
                                DeepGrap(0.0f, GrapType.NormalGrap, false);
                                GrapLing = true;
                            }
                        }
                    }
                    if (a == pos4.y)
                    {
                        //橋ベースがあるとき
                        if (onebridgebase)
                        {
                            Vector3 exvec = onebridgebase.GetComponent<SpriteRenderer>().bounds.extents;
                            DeepGrap(onebridgebase.transform.position.y, GrapType.NormalGrap, true);
                            nowIE = Graplinger(new Vector3(transform.parent.position.x, onebridgebase.transform.position.y - exvec.y + Player_verticalhorizontal.y, transform.parent.position.z));
                            StartCoroutine(nowIE);
                            DeepGrap(0.0f, GrapType.NormalGrap, false);
                            GrapLing = true;
                        }
                    }
                }
                if (!GrapLing)
                {
                    //間に何もないとき
                    if (Front_LeftTop.y <= pos.y && Front_LeftTop.y <= pos5.y)
                    {
                        //なおかつ、Playerが橋ベース枠内でないとき
                        GameObject Inbridgebase = (RedSide != SideRedLine.Non) ? GetPlayerInBridgeBase() : null;
                        if (Inbridgebase == null)
                        {
                            GrapLing = true;
                            //上限まで伸ばす
                            DeepGrap(Front_LeftTop.y - transform.parent.position.y, GrapType.NormalGrap, true);
                            nowIE = Graplinger(new Vector3(transform.parent.position.x, Front_LeftTop.y - Player_verticalhorizontal.y + 0.1f, transform.parent.position.z));
                            StartCoroutine(nowIE);
                            // グラップリングを消すIterator
                            DeepGrap(0.0f, GrapType.NormalGrap, false);
                        }
                        else
                        {
                            if (Inbridgebase.transform.position.y + Inbridgebase.GetComponent<SpriteRenderer>().bounds.extents.y < Front_LeftTop.y - G_Data.RedLine * 0.5f)
                            {
                                GrapLing = true;
                                //上限まで伸ばす
                                DeepGrap(Front_LeftTop.y - transform.parent.position.y, GrapType.NormalGrap, true);
                                nowIE = Graplinger(new Vector3(transform.parent.position.x, Front_LeftTop.y - Player_verticalhorizontal.y + 0.1f, transform.parent.position.z));
                                StartCoroutine(nowIE);
                                // グラップリングを消すIterator
                                DeepGrap(0.0f, GrapType.NormalGrap, false);
                            }
                        }
                    }
                }
            }
        }
    }

    void InstanceMakeBridge(Vector3 _vec, float _Angle)
    {
        if (SoundObj)
            SoundObj.ActionSE();
        _bBridgeMaking = false;
        BridgeObj = Instantiate(Bridge, _vec, Quaternion.Euler(180, 0, _Angle));
    }

    //=======================================================================
    // 各種設定受け渡し
    //=======================================================================
    //橋範囲右下記録
    public Vector3 BridgeAriaLT
    {
        get { return BridgeAriaLeftTop; }
        set { BridgeAriaLeftTop = value; }
    }
    //橋範囲右下記録
    public Vector3 BridgeAriaBR
    {
        get { return BridgeAriaRightBottom; }
        set { BridgeAriaRightBottom = value; }
    }
    /// <summary>
    /// 箱を移ったときの次の箱
    /// </summary>
    public void SetNextBox(SideColorBoxScript nextBox)
    {
        sidebox = nextBox;
    }
    /// <summary>
    /// プレイヤー行動許可
    /// </summary>
    public bool Moving
    {
        get { return _bControll; }
        set
        {
            _bControll = value;
            FootStamp.gameObject.SetActive(true);
        }
    }
    public bool FootStampActive
    {
        set { FootStamp.gameObject.SetActive(value); }
    }
    public Rigidbody P_rb
    {
        get { return rb; }
    }
    //行動範囲左上記録
    public Vector3 Front_LeftTop
    {
        get { return RollAriaLeftTop; }
        set
        {
            RollAriaLeftTop = value;

            var PPos = transform.parent.position;
            var T = PPos.y + Player_verticalhorizontal.y;
            var L = PPos.x - Player_verticalhorizontal.x;

            if (RollAriaLeftTop.x >= L)
                PPos.x = RollAriaLeftTop.x + Player_verticalhorizontal.x + offset;
            if (T >= RollAriaLeftTop.y)
                PPos.y = RollAriaLeftTop.y - Player_verticalhorizontal.y - offset;

            NowPlayerMovePoint = transform.parent.position = PPos;

        }
    }
    //行動範囲右下記録
    public Vector3 Front_RightBottom
    {
        get { return RollAriaRightBottom; }
        set
        {
            RollAriaRightBottom = value;

            var PPos = transform.parent.position;
            var B = PPos.y - Player_verticalhorizontal.y;
            var R = PPos.x + Player_verticalhorizontal.x;

            if (R >= RollAriaRightBottom.x)
                PPos.x = RollAriaRightBottom.x - Player_verticalhorizontal.x - offset;
            if (RollAriaRightBottom.y >= B)
                PPos.y = RollAriaRightBottom.y + Player_verticalhorizontal.y + offset;
            NowPlayerMovePoint = transform.parent.position = PPos;
        }
    }

    //行動範囲左上記録
    public Vector3 Move_Aria_FLT
    {
        get { return MoveAriaLeftTop; }
        set { MoveAriaLeftTop = value; }
    }

    //行動範囲右下記録
    public Vector3 Move_Aria_FRB
    {
        get { return MoveAriaRightBottom; }
        set { MoveAriaRightBottom = value; }
    }
    public bool GetGrapLing
    {
        get { return GrapLing; }
    }
    //========================================================
    // 橋の上に乗ったときのプレイヤー処理
    //========================================================
    IEnumerator InBridge()
    {
        while (GrapLing)
        {
            yield return new WaitForEndOfFrame();
        }
        rb.isKinematic = true;
        var bridgeSc = BridgeObj.GetComponent<bridgeScript>();

        //橋を渡るので、元居た箱の方の
        //橋ベース用コライダ . 地面用コライダ . 不透過壁用コライダ   を非アクティブ化しておく
        float P = (transform.parent.position - bridgeSc.BasePrev.transform.position).sqrMagnitude;
        float N = (transform.parent.position - bridgeSc.BaseNext.transform.position).sqrMagnitude;
        //渡した側に近い時
        if (P < N)
            bridgeSc.BridgeCross(true);
        //渡された側に近い時
        else
            bridgeSc.BridgeCross(false);
        bridgeSc.BaseNext.GetComponent<CapsuleCollider>().enabled = false;
        bridgeSc.BasePrev.GetComponent<CapsuleCollider>().enabled = false;

        float timer = 0;
        var ppos = transform.parent.position;//player position
        var Spos = transform.parent.position;//橋の移動基準となるポジションにするやつ
        Vector3 epos;
        Vector3 range = BridgeObj.GetComponent<MeshRenderer>().bounds.extents;

        //横向きの橋
        if (range.x > range.y)
        {
            Spos = new Vector3(transform.parent.position.x, BridgeObj.transform.position.y, transform.parent.position.z);
            //Playerが橋の左側
            if (Spos.x < BridgeObj.transform.position.x)
                epos = BridgeObj.transform.position + new Vector3(range.x + (Player_verticalhorizontal.x + 0.03f), 0);
            else
                epos = BridgeObj.transform.position - new Vector3(range.x + (Player_verticalhorizontal.x + 0.03f), 0);
        }
        //縦向きの橋
        else
        {
            Spos = new Vector3(BridgeObj.transform.position.x, transform.parent.position.y, transform.parent.position.z);
            //Playerが橋の下側
            if (Spos.y < BridgeObj.transform.position.y)
                epos = BridgeObj.transform.position + new Vector3(0, range.y + (Player_verticalhorizontal.y + 0.03f));
            else
                epos = BridgeObj.transform.position - new Vector3(0, range.y + (Player_verticalhorizontal.y + 0.03f));
        }


        //橋の中心線との距離を得て、橋の中心線に向けて移動させる
        Move_Anim(true);
        var distance = (ppos - Spos).magnitude;

        if (distance != 0) distance = 1 / distance;

        while (distance != 0)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime * distance;
            transform.parent.position = Vector3.Lerp(ppos, Spos, timer);

            if (timer >= 1)
                break;
        }

        NowPlayerMovePoint = transform.parent.position = Spos;


        //橋の向こう側に向けて移動させる
        timer = 0;
        while (true)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            transform.parent.position = Vector3.Lerp(Spos, epos, timer);

            if (timer >= 1)
                break;
        }

        horizontalPlayerMovePoint = Vector3.zero;
        NowPlayerMovePoint = transform.parent.position = epos;
        Move_Anim(false);
        if (BridgeObj)
            BridgeObj.transform.GetComponent<bridgeScript>().OnBridgeCollider();
        //----------------------------------------------------
        //橋の外にたどり着いたので各種セット
        // playerを範囲内に含む橋ベースオブジェクト探索

        foreach (GameObject obj in G_Data.Bases)
        {
            //移動先
            Vector3 extents_vec = obj.transform.parent.GetComponent<MeshRenderer>().bounds.extents;

            Vector3 FLTvec = obj.transform.parent.position + new Vector3(
                    -Mathf.Abs(extents_vec.x),
                     Mathf.Abs(extents_vec.y));

            Vector3 BRBvec = obj.transform.parent.position + new Vector3(
                     Mathf.Abs(extents_vec.x),
                    -Mathf.Abs(extents_vec.y));

            //間違い防止策
            if (FLTvec.x > BRBvec.x)
            {
                float sub = BRBvec.x;
                BRBvec.x = FLTvec.x;
                FLTvec.x = sub;
            }

            if (FLTvec.y < BRBvec.y)
            {
                float sub = BRBvec.y;
                BRBvec.y = FLTvec.y;
                FLTvec.y = sub;
            }

            if (FLTvec.x < epos.x && epos.x < BRBvec.x)
                if (BRBvec.y < epos.y && epos.y < FLTvec.y)
                    nextBase = obj;
        }

        if (nextBase != null)
        {
            //箱を登録
            var sideBox = nextBase.transform.parent.GetComponent<SideColorBoxScript>();
            SetNextBox(sideBox);
            //移動範囲を再びセット
            sideBox.SetBoxPos(this);
            G_Data.RedLine = sideBox.transform.root.localScale.x;

            if (G_Data.P_Now_Box != sidebox)
            {
                camM.SetNextBox(sideBox);
                G_Data.P_Now_Box = sidebox.gameObject;
            }
        }
        gameManager.CheckBridgeGoal();
        bridgestate = BridgeAriaState.ignore;
        Moving = true;
    }
    //========================================================
    // グラップリングしたときのプレイヤー処理
    //========================================================
    IEnumerator Graplinger(Vector3 point)
    {
        rb.isKinematic = true;
        yield return new WaitForSeconds(1.0f);

        SphereCollider sCollider = transform.parent.GetComponent<SphereCollider>();
        sCollider.enabled = false;
        Vector3 Ppos = transform.parent.position;
        float timer = 0;
        Move_Anim(true);
        while (!OnBridge)
        {
            transform.parent.position = NowPlayerMovePoint = Vector3.Lerp(Ppos, point, timer);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();

            if (timer >= 1) break;

            if (OnBridge)
            {
                GrapLing = false;
                sCollider.enabled = true;
                yield break;
            }
        }
        rb.isKinematic = false;
        NowPlayerMovePoint = transform.parent.position = point;
        Move_Anim(false);
        sCollider.enabled = true;
        GrapLing = false;
    }

    //========================================================
    // グラップアタックしたときのプレイヤー処理
    //========================================================
    IEnumerator GrapAttack()
    {
        float timer = 0;

        while (true)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();

            if (timer > 1) break;
        }
        GrapLing = false;
    }
    public void SceneEndBridgeBreak()
    {
        if (BridgeObj)
        {
            Destroy(BridgeObj);
        }
    }
    public void SceneEndBridgeFall()
    {
        if (BridgeObj) BridgeObj.SendMessage("RollDestroy", 1);
    }


    //========================================================
    // グラップリングできるかどうか判定のための処理
    //========================================================
    //足場の上か否か
    public bool OnGroundGraplingJudge()
    {
        if (!GrapLing && Moving)
        {
            if (rb.isKinematic || rb.velocity.y == 0) return true;

            if (GetPlayerOnGround()) return true;
            if (sidebox.NPWall != null)
                foreach (GameObject ground in sidebox.NPWall)
                {
                    //画像の縦横をとり、地面(不透過壁)が横広のとき、グラップリング対象とするtio
                    Vector3 exvec = ground.GetComponent<SpriteRenderer>().bounds.extents;
                    //正面に来ている地面のみ
                    if (ground.transform.position.z <= sidebox.transform.position.z - exvec.z)
                    {
                        if (exvec.x > exvec.y)
                        {
                            //地面幅のなかにプレイヤーが存在するなら
                            if (ground.transform.position.x - exvec.x < transform.parent.position.x && transform.parent.position.x < ground.transform.position.x + exvec.x)
                            {
                                //その地面の上にプレイヤーがいるなら
                                if (transform.parent.position.y - Player_verticalhorizontal.y - 0.1f < ground.transform.position.y + exvec.y
                                    && ground.transform.position.y < transform.parent.position.y)
                                    return true;
                            }
                        }
                    }
                }
        }
        return false;
    }
    /// <summary>
    /// グラップリングタイプ判定
    /// true = GrapAttack
    /// false = GrapLing
    /// </summary>
    /// <returns></returns>
    public int GrapGimmickType()
    {
        if (!GrapLing)
        {
            GameObject land = null;
            GameObject onebridgebase = null;

            foreach (GameObject ground in sidebox.BoxInGround)
            {
                if (ground.transform.position.y > NowPlayerMovePoint.y)
                {
                    //画像の縦横をとり、地面が横広のとき、グラップリング対象とする
                    Vector3 exvec = ground.GetComponent<SpriteRenderer>().bounds.extents;
                    if (exvec.x > exvec.y)
                    {
                        //正面に来ている足場のみ
                        if (ground.transform.position.z <= sidebox.transform.position.z - exvec.z)
                        {
                            //足場幅のなかにプレイヤーが存在するなら
                            if (ground.transform.position.x - exvec.x < transform.parent.position.x && transform.parent.position.x < ground.transform.position.x + exvec.x)
                            {
                                if (land == null)
                                    land = ground;
                                else
                                if (land.transform.position.y > ground.transform.position.y)
                                    land = ground;
                            }
                        }
                    }
                }
            }

            //間に足場がないとき
            if (land == null)
            {
                foreach (GameObject obj in sidebox.GetBridgeLine)
                {
                    if (NowPlayerMovePoint.y < obj.transform.position.y)
                    {
                        Vector3 exvec = obj.GetComponent<SpriteRenderer>().bounds.extents;
                        if (exvec.x > exvec.y)//横広
                            if (obj.transform.position.z <= sidebox.transform.position.z - exvec.z)//前面
                            {
                                //幅内
                                if (obj.transform.position.x - exvec.x < NowPlayerMovePoint.x && NowPlayerMovePoint.x < obj.transform.position.x + exvec.x)
                                    onebridgebase = obj;//足場の中で最も近い足場を得ている
                            }
                    }
                }
            }

            //グラップリング処理
            //ただし移動できない壁後付けするため注意。
            if (rb.isKinematic == true || rb.velocity.y == 0)
            {
                Vector3 instantpos;
                Vector3 target;
                //不透過壁があるか否か
                //存在しないとき箱左上で返す
                target = sidebox.sideWall();


                //橋の下限の位置
                if (BridgeObj)
                {
                    instantpos = BridgeObj.transform.position;
                    var ran = BridgeObj.GetComponent<MeshRenderer>().bounds.extents;
                    instantpos.y -= ran.y;
                    if (NowPlayerMovePoint.y < instantpos.y)
                        if (instantpos.y < target.y)
                            if (BridgeObj.transform.position.x - ran.x < transform.parent.position.x && transform.parent.position.x < BridgeObj.transform.position.x + ran.x)
                                target = instantpos;

                }
                //足場の位置
                if (land)
                {
                    instantpos = land.transform.position;
                    var ran = land.GetComponent<SpriteRenderer>().bounds.extents;
                    instantpos.y -= ran.y;
                    if (land.transform.position.y > NowPlayerMovePoint.y)
                        if (instantpos.y < target.y)
                            target = instantpos;
                }
                //橋ベースの位置
                if (onebridgebase)
                {
                    instantpos = onebridgebase.transform.position;
                    var ran = onebridgebase.GetComponent<SpriteRenderer>().bounds.extents;
                    instantpos.y -= ran.y;
                    if (NowPlayerMovePoint.y < onebridgebase.transform.position.y)
                        if (instantpos.y < target.y)
                            if (onebridgebase.transform.position.x - ran.x < NowPlayerMovePoint.x && NowPlayerMovePoint.x < onebridgebase.transform.position.x + ran.x)
                                target = instantpos;
                }
                //不透過壁の位置
                //存在しないとき箱左上で返す
                instantpos = sidebox.UnPassWallPos();
                if (instantpos.y != Front_LeftTop.y)
                    if (instantpos.y < target.y)
                        target = instantpos;


                //---Gimmick---

                //回転スイッチの位置(仮)
                //存在しないとき箱左上で返す
                instantpos = sidebox.FindBoxRollerSwitch();
                if (instantpos.y < target.y)
                    return 0;

                //不透過壁の位置再計算
                instantpos = sidebox.UnPassWallPos();
                if (instantpos.y != Front_LeftTop.y)
                    if (instantpos.y <= target.y)
                        return 1;
                instantpos = sidebox.sideWall();
                if (instantpos.y <= target.y)
                    if (instantpos.y != Front_LeftTop.y)
                        return 1;
            }
        }
        return 2;
    }


    //---------------------------------------------------------
    //**      ** 下を押したときの判定
    //---------------------------------------------------------
    public bool CheckSlipDown()
    {
        //すり抜けられる床かどうか
        GameObject ground = GetPlayerOnGround();
        if (ground)
        {
            return true;
        }
        else
        {
            if (CheckRedAria())
            {
                GameObject obj = GetPlayerInBridgeBase();
                if (obj)
                    return true;
                else
                    if (BridgeObj)
                {
                    //記述
                    //橋の上にいるときの判定が要る
                    Vector3 ex = BridgeObj.GetComponent<MeshRenderer>().bounds.extents;
                    Vector3 pos = BridgeObj.transform.position;
                    if (pos.x - ex.x < NowPlayerMovePoint.x && NowPlayerMovePoint.x < pos.x + ex.x)
                        if (pos.y + ex.y < NowPlayerMovePoint.y && NowPlayerMovePoint.y < pos.y + ex.y + Player_verticalhorizontal.y * 2)
                            return true;
                }
            }
            //ここに下回転する前の下移動の処理を書く
            if (sideState != SideState.left_right_bottom_wall &&
                sideState != SideState.left_bottom_wall &&
                sideState != SideState.right_bottom_wall &&
                sideState != SideState.bottom_wall)
                return true;
        }
        return false;
    }
    //---------------------------------------------------------
    //  橋を作れるエリア(橋ベース)か否か
    //---------------------------------------------------------
    //  橋を作れるエリア内
    public bool CheckBridgeBaseAria()
    {
        GameObject obj = GetPlayerInBridgeBase();
        if (obj) return true;
        return false;
    }
    //  橋を作れるエリア(橋ベース)か否か
    public bool CheckBridgeMakeAria()
    {
        GameObject obj = GetPlayerInBridgeBase();
        if (obj)
        {
            //取得全ベースから現在の箱と同じ箱についている橋のベース以外で一番近い橋Baseを得る
            nextBase = null;
            float distance = float.MaxValue;
            Vector3 posA = obj.transform.position;

            bool check;
            Color colord = obj.GetComponent<SpriteRenderer>().color;
            foreach (GameObject target in G_Data.Bases)//全べース探索
            {
                check = true;
                for (int i = 0; i < sidebox.GetBridgeLine.Length; i++)
                {
                    if (sidebox.GetBridgeLine[i].gameObject == target)
                    {
                        check = false;
                        break;
                    }
                }
                if (!check) continue;
                //色とレイヤーが異なるかどうか
                if (colord != target.GetComponent<SpriteRenderer>().color || target.layer != target.layer)
                    continue;


                Vector3 posB = target.transform.position;
                //橋ベースがy軸方向の方が長いかどうか
                if (target.transform.localScale.x < target.transform.localScale.y)
                    posB.y = posA.y = 0;//longY
                else
                    posB.x = posA.x = 0;//longX

                float dis = Mathf.Abs(Vector3.Distance(posA, posB));

                //橋ベース同士の距離比較
                if (dis < distance)
                {
                    //playerがその橋ベースの幅内にあるとき。
                    Vector3 target_extents = target.transform.GetComponent<SpriteRenderer>().bounds.extents;

                    if (
                    (target.transform.position.x - target_extents.x < NowPlayerMovePoint.x && NowPlayerMovePoint.x < target.transform.position.x + target_extents.x) ||
                    (target.transform.position.y - target_extents.y < NowPlayerMovePoint.y && NowPlayerMovePoint.y < target.transform.position.y + target_extents.y))
                    {
                        distance = dis;
                        nextBase = target;
                    }
                }
            }

            //橋基地同士の距離が橋以下のとき
            Vector3 bounds = Bridge.GetComponent<MeshRenderer>().bounds.extents * 2;

            if (bounds.x > bounds.y) bounds.y = bounds.x;//橋の長さは長いほうをとる

            //生成方向準備
            if (distance - 0.001f <= bounds.y)
            {
                //ここで枠つくるか？
                return true;
            }
        }
        return false;
    }

    //--------------------------------------------------------------------------------

    /*
    * グラップリング時の舌の表示用関数
    * trueで表示,falseで非表示
    */
    private void DeepGrap(float fYNorm, GrapType gType, bool DrawFrag)
    {
        if (DrawFrag)
        {
            if (SoundObj)
                SoundObj.GrapSE();
            int DeepNum = (int)fYNorm;
            if (DeepNum >= 5)
            {
                DeepNum += 3;
            }
            else if (DeepNum >= 3)
            {
                DeepNum += 2;
            }
            else
            {
                DeepNum += 1;
            }

            for (int i = 0; i < DeepNum; i++)
            {
                if (i < Deeplist.Length/*i < gDeepObj.Length*/)
                {
                    // 遅延しながら表示
                    StartCoroutine(DrawDeep(i));
                }
            }
        }
        else
        {
            StartCoroutine(AnDrawDeep(gType));
        }
    }

    /*
    * 舌の表示用Iterator
    */
    IEnumerator DrawDeep(int nDeep)
    {
        Vector3 DeepPos = new Vector3(this.transform.position.x + 0.3f, this.transform.position.y + (0.75f * nDeep), this.transform.position.z);
        yield return new WaitForSeconds(fInitDeepTime);
        // 遅延しながら非表示になるように
        //gDeepObj[nDeep].SetActive(true);
        Deeplist[nDeep].transform.position = DeepPos;
        Deeplist[nDeep].transform.rotation = gIDeep.transform.rotation;
        Deeplist[nDeep].SetActive(true);
    }

    /*
    * 舌の非表示用Iterator
    */
    IEnumerator AnDrawDeep(GrapType gType)
    {
        if (gType == GrapType.NormalGrap)
        {
            yield return new WaitForSeconds(fDestDeepTime_Normal_Wait);
            for (int i = 0; i < Deeplist.Length; i++)
            {
                if (Deeplist[i] && Deeplist[i].activeSelf)
                {
                    yield return new WaitForSeconds(fDestDeepTime_Normal_Interval);
                    Deeplist[i].SetActive(false);
                }
            }
        }
        else if (gType == GrapType.Button)
        {
            yield return new WaitForSeconds(fDestDeepTime_Button_Wait);
            for (int i = Deeplist.Length - 1; i >= 0; i--)
            {
                yield return new WaitForSeconds(fDestDeepTime_Button_Interval);
                Deeplist[i].SetActive(false);
            }
            sidebox.RollSwitchAction();
        }
        //if (gType == GrapType.NormalGrap)
        //{
        //    yield return new WaitForSeconds(fDestDeepTime_Normal_Wait);
        //    for (int i = gDeepObj.Length - 1; i >= 0; i--)
        //    {
        //        if (gDeepObj[i] && gDeepObj[i].activeSelf)
        //        {
        //            yield return new WaitForSeconds(fDestDeepTime_Normal_Interval);
        //            gDeepObj[i].SetActive(false);
        //        }
        //    }
        //}
        //else if (gType == GrapType.Button)
        //{
        //    yield return new WaitForSeconds(fDestDeepTime_Button_Wait);
        //    for (int i = gDeepObj.Length - 1; i >= 0; i--)
        //    {
        //        if (gDeepObj[i])
        //        {
        //            yield return new WaitForSeconds(fDestDeepTime_Button_Interval);
        //            gDeepObj[i].SetActive(false);
        //        }
        //    }
        //}
        yield return null;
    }

    //--------------------------------------------------------------------------------

    void CheckOnGround()
    {
        if (follState == FollState.Landing)
        {
            follState = FollState.Stand;
            groundSmokes[0].Play();
            groundSmokes[1].Play();
        }

        if (follState == FollState.Foll)
        {
            if (rb.velocity.y < -0.3f)
            {
                follState = FollState.Folling;
                if (SoundObj)
                    SoundObj.FollSE();
            }
        }
    }

    public void InClearBox(Vector3 CBoxPosition)
    {
        Moving = false;
        if (transform.position.x <= CBoxPosition.x)
        {
            //画像反転用
            transform.GetChild(0).localScale = Vector3.one;
            //足跡反転用
            var romain = FootStamp.main;
            romain.startRotation = new ParticleSystem.MinMaxCurve(1.396f, 1.745f);
            // サウンド再生
            if (AnimCount++ > AnimCountSet)
            {
                if (SoundObj)
                    SoundObj.MoveSE();
                AnimCount = 0;
            }
            //アニメーション
            Move_Anim(true);

        }
        else
        {
            //画像反転用
            transform.GetChild(0).localScale = new Vector3(-1, 1, 1);
            //足跡反転用
            var romain = FootStamp.main;
            romain.startRotation = new ParticleSystem.MinMaxCurve(-1.396f, -1.745f);
            // サウンド再生
            if (AnimCount++ > AnimCountSet)
            {
                if (SoundObj)
                    SoundObj.MoveSE();
                AnimCount = 0;
            }
            //アニメーション
            Move_Anim(true);
        }
    }
}
