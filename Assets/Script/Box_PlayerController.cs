using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_PlayerController : MonoBehaviour
{
    [SerializeField, Range(0.1f, 10)] float MoveRange = 0.4f;
    [SerializeField, Range(0.1f, 10)] float DropSpeed = 1;

    Vector3 NowPlauerMovePoint;
    int MoveCount = 0;
    [SerializeField] int FlameCount = 10;

    [SerializeField] GameObject _Canvas;
    private UIScript _UICanvas;

    bool _bControll = false;

    bool OnBridge = false;

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

    enum SideRedLine
    {
        T, B, L, R, Non,
    }
    SideRedLine RedSide = SideRedLine.Non;
    IEnumerator nowIE;

    void Start()
    {
        rb = transform.parent.GetComponent<Rigidbody>();

        var sprvec = transform.GetChild(0).GetComponent<MeshRenderer>();
        Player_verticalhorizontal = sprvec.bounds.extents;
        camM = Camera.main.GetComponent<CameraManager>();
        G_Data = GameObject.FindWithTag("BoxManager").GetComponent<GameData>();

        _UICanvas = _Canvas.GetComponent<UIScript>();
        _UICanvas.ChangeNum(nDCount);
        NowPlauerMovePoint = transform.parent.position;
    }


    void Update()
    {
        
        //落下速度制限
        Vector3 vel = rb.velocity;
        if (vel.y < -DropSpeed * 10) vel.y = -DropSpeed * 10;
        rb.velocity = vel;

        if (Moving)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vartical = Input.GetAxis("Vertical");

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

                        GameObject wind = Instantiate(G_Data.WindPrefab, sidebox.transform.position, Quaternion.identity);
                        //上下左右
                        var rollways = 0;
                        if (T > Front_LeftTop.y) { rollways = 1; wind.transform.rotation = Quaternion.Euler(0, 0, 90); }
                        if (B < Front_RightBottom.y) { rollways = 2; wind.transform.rotation = Quaternion.Euler(0, 0, -90); }
                        if (L < Front_LeftTop.x) { rollways = 3; wind.transform.rotation = Quaternion.Euler(0, 0, 180); }
                        if (R > Front_RightBottom.x) { rollways = 4; wind.transform.rotation = Quaternion.Euler(0, 0, 0); }
                        if (BridgeObj) Destroy(BridgeObj);
                        sidebox.ChangeBoxRoll(rollways);
                    }
                    else
                    {
                        // 一定フレームで処理
                        if (FlameCount < MoveCount++)
                        {
                            MoveCount = 0;
                            // プレイヤー移動範囲チェック
                            RePositionMoveAria();
                            this.Move(horizontal, vartical);
                        }
                        else
                        {
                            if (NowPlauerMovePoint.y != transform.parent.position.y) NowPlauerMovePoint.y = transform.parent.position.y;
                            transform.parent.position = Vector3.Lerp(transform.parent.position, NowPlauerMovePoint, MoveCount / FlameCount);
                        }
                    }

                    if (Input.GetButton("Jump"))
                    {
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

    //移動範囲確認(箱)
    void RePositionMoveAria()
    {
        var pos = transform.parent.position;
        var T = pos.y + Player_verticalhorizontal.y;
        var B = pos.y - Player_verticalhorizontal.y;
        var R = pos.x + Player_verticalhorizontal.x;
        var L = pos.x - Player_verticalhorizontal.x;
        
        if (Move_Aria_FLT.x >= L) transform.parent.position += Vector3.right * MoveRange;
        if (R >= Move_Aria_FRB.x) transform.parent.position -= Vector3.right * MoveRange;
        if (T >= Move_Aria_FLT.y) transform.parent.position -= Vector3.up * MoveRange;
        if (Move_Aria_FRB.y == RollAriaRightBottom.y)
        {
            if (Move_Aria_FRB.y + G_Data.RedLine / 2 >= B)
            {
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }
        else
            if (Move_Aria_FRB.y >= B)
        {
            NowPlauerMovePoint = transform.parent.position += new Vector3(0, Mathf.Abs(Move_Aria_FRB.y - B));
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
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

        if (B <= Move_Aria_FRB.y + MoveRange)
            rb.isKinematic = true;
        else
            rb.isKinematic = false;

        if (RollAriaLeftTop.x < L && R < RollAriaRightBottom.x)
            if (RollAriaRightBottom.y < B && T < RollAriaLeftTop.y)
            {
                return false;
            }
        return true;
    }


    //移動範囲確認(橋)
    bool CheckMoveBridgeAria()
    {
        if (BridgeObj)
        {
            if (BridgeObj.transform.forward == Vector3.forward || BridgeObj.transform.forward == -Vector3.forward)
            {
                var pos = transform.parent.position;
                var T = pos.y + Player_verticalhorizontal.y;
                var B = pos.y - Player_verticalhorizontal.y;
                var R = pos.x + Player_verticalhorizontal.x;
                var L = pos.x - Player_verticalhorizontal.x;

                //範囲内に触れたとき（プレイヤーが橋の上にいる）
                if (BridgeAriaLT.x <= R && L <= BridgeAriaBR.x)
                    if (BridgeAriaBR.y <= T && B <= BridgeAriaLT.y)
                    {
                        NowPlauerMovePoint = transform.parent.position;
                        pos.z = BridgeAriaLT.z;
                        transform.parent.position = pos;
                        camM.Bridge = OnBridge = true;
                        

                        return OnBridge;
                    }
            }
        }
        camM.Bridge = OnBridge = false;
        return OnBridge;
    }


    //辺の赤範囲判定
    /// <summary>
    /// OnRedLine = true;
    /// </summary>
    public bool CheckRedAria()
    {
        //G_Data.RedLineは赤線の幅
        if (RollAriaLeftTop.x + G_Data.RedLine <= transform.parent.position.x - Player_verticalhorizontal.x && transform.parent.position.x + Player_verticalhorizontal.x <= RollAriaRightBottom.x - G_Data.RedLine)
            if (RollAriaRightBottom.y + G_Data.RedLine <= transform.parent.position.y - Player_verticalhorizontal.y && transform.parent.position.y + Player_verticalhorizontal.y <= RollAriaLeftTop.y - G_Data.RedLine)
            {
                RedSide = SideRedLine.Non;
                return false;
            }
        //左右を優先
        if (RollAriaLeftTop.x + G_Data.RedLine > transform.parent.position.x - Player_verticalhorizontal.x)
            RedSide = SideRedLine.L;
        else if (transform.parent.position.x + Player_verticalhorizontal.x > RollAriaRightBottom.x - G_Data.RedLine)
            RedSide = SideRedLine.R;
        else
        if (RollAriaRightBottom.y + G_Data.RedLine > transform.parent.position.y - Player_verticalhorizontal.y)
            RedSide = SideRedLine.B;
        else if (transform.parent.position.y - Player_verticalhorizontal.y > RollAriaLeftTop.y - G_Data.RedLine)
            RedSide = SideRedLine.T;
        return true;
    }


    //=======================================================================
    // プレイヤー移動
    //=======================================================================
    //this->
    void Move(float horizontal, float vartical)
    {
        if (horizontal > 0)
        {
            NowPlauerMovePoint += Vector3.right * MoveRange;
        }
        else
        if (horizontal < 0)
        {
            NowPlauerMovePoint -= Vector3.right * MoveRange;
        }
        //*********************************************************
        //**      ** 下を押したときの処理はここに描く    **      **
        //*********************************************************
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
                            if (transform.parent.position.y - Player_verticalhorizontal.y - 0.1f < ground.transform.position.y &&
                                ground.transform.position.y < transform.parent.position.y)
                            {
                                ground.GetComponent<GroundScript>().SlipDown();
                                slide = true;
                            }
                    }
                }
            }

            if (!slide)
            {
                if (CheckRedAria())
                {
                    bool bflag = false;
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

                                _vec = new Vector3(
                                     Mathf.Abs(bridgebaseline.x),
                                    -Mathf.Abs(bridgebaseline.y),
                                    -Mathf.Abs(bridgebaseline.z));
                                Vector3 BRB = sidebox.GetBridgeLine[i].transform.position + _vec;

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
                                Vector3 pos = transform.parent.position;
                                float T = pos.y + Player_verticalhorizontal.y;
                                float B = pos.y - Player_verticalhorizontal.y;
                                float R = pos.x + Player_verticalhorizontal.x;
                                float L = pos.x - Player_verticalhorizontal.x;
                                //範囲内のとき
                                if (FLT.x <= R && L <= BRB.x)
                                    if (BRB.y <= T && B <= FLT.y)
                                    {
                                        bflag = true;
                                        sidebox.GetBridgeLine[i].SendMessage("SlipdroundLine");
                                        if (BridgeObj)
                                        {
                                            Vector3 checkpos = new Vector3(BridgeObj.transform.position.x, BridgeAriaLT.y, BridgeObj.transform.position.z);
                                            if (FLT.x <= checkpos.x && checkpos.x <= BRB.x)
                                                if (BRB.y <= checkpos.y && checkpos.y <= FLT.y)
                                                    BridgeObj.GetComponent<bridgeScript>().second_NoCollider();
                                        }
                                    }
                            }
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
        /*
        if (vartical > 0)
            transform.parent.localPosition += Vector3.up * Speed;
        else
        if (vartical < 0)
            transform.parent.localPosition -= Vector3.up * Speed;
        */
    }


    //=======================================================================
    // 橋
    //=======================================================================
    //this->
    public void MakeBridgeCheck()
    {
        GameObject target = null;

        if (CheckRedAria())
        {
            /*
            if (RollAriaLeftTop.x + G_Data.RedLine <= transform.parent.position.x - Player_verticalhorizontal.x &&
            transform.parent.position.x + Player_verticalhorizontal.x <= RollAriaRightBottom.x - G_Data.RedLine)
            if (RollAriaRightBottom.y + G_Data.RedLine <= transform.parent.position.y - Player_verticalhorizontal.y && transform.parent.position.y + Player_verticalhorizontal.y <= RollAriaLeftTop.y - G_Data.RedLine)
             */
            //Debug.Log("LT:"+RollAriaLeftTop + " RB:" + RollAriaRightBottom + " PL" + transform.parent.position);
            //Debug.Log("RedAria?");
            //Debug.Log("PR" + transform.parent.position.x + Player_verticalhorizontal.x + "R" + (RollAriaRightBottom.x - G_Data.RedLine));
        }

        if (RedSide != SideRedLine.Non)
        {
            //橋ベースの長い方向を記録する用
            int BridgeWay = 0;

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

                        _vec = new Vector3(
                             Mathf.Abs(bridgebaseline.x),
                            -Mathf.Abs(bridgebaseline.y),
                            -Mathf.Abs(bridgebaseline.z));
                        Vector3 BRB = sidebox.GetBridgeLine[i].transform.position + _vec;

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
                        var T = pos.y + Player_verticalhorizontal.y;
                        var B = pos.y - Player_verticalhorizontal.y;
                        var R = pos.x + Player_verticalhorizontal.x;
                        var L = pos.x - Player_verticalhorizontal.x;

                        //範囲内のとき
                        if (FLT.x <= R && L <= BRB.x)
                            if (BRB.y <= T && B <= FLT.y)
                            {
                                target = sidebox.GetBridgeLine[i].gameObject;

                                //橋ベースがy軸方向の方が長いかどうか
                                if (target.transform.localScale.x < target.transform.localScale.y)
                                    BridgeWay = -1;
                                else
                                    BridgeWay = 1;
                            }
                    }
                }
            }

            bool MakeOk = false;

            //橋のベースの範囲内にプレイヤーがいたとき、そのベースがnullでなくなる
            if (target != null)
            {
                //取得全ベースから現在の箱と同じ箱についている橋のベース以外で一番近い橋Baseを得る
                nextBase = null;
                float distance = float.MaxValue;
                Vector3 posA = target.transform.position;
                bool check;

                foreach (GameObject obj in G_Data.Bases)
                {
                    check = true;
                    for (int i = 0; i < sidebox.GetBridgeLine.Length; i++)
                    {
                        if (sidebox.GetBridgeLine[i].gameObject == obj)
                        {
                            check = false;
                            break;
                        }
                    }

                    if (target.layer != obj.layer)
                        check = false;

                    if (check)
                    {
                        Vector3 posB = obj.transform.position;

                        if (BridgeWay == 1)//longX
                            posB.x = posA.x = 0;
                        else if (BridgeWay == -1)//longY
                            posB.y = posA.y = 0;

                        float dis = Mathf.Abs(Vector3.Distance(posA, posB));

                        //橋ベース同士の距離が規定橋距離以下のとき(橋をかけれるとき)
                        if (dis < distance)
                        {
                            //playerがその橋ベースの幅内にあるとき。
                            Vector3 obj_extents = obj.transform.GetComponent<SpriteRenderer>().bounds.extents;

                            if (
                            (obj.transform.position.x - obj_extents.x < transform.parent.position.x && transform.parent.position.x < obj.transform.position.x + obj_extents.x) ||
                            (obj.transform.position.y - obj_extents.y < transform.parent.position.y && transform.parent.position.y < obj.transform.position.y + obj_extents.y))
                            {
                                distance = dis;
                                nextBase = obj;
                            }
                        }
                    }
                }

                //橋基地同士の距離が橋以下のとき
                Vector3 bounds = Bridge.GetComponent<MeshRenderer>().bounds.extents * 2;

                if (bounds.x > bounds.y) bounds.y = bounds.x;

                //生成方向準備
                if (distance - 0.001f <= bounds.y)
                {
                    MakeOk = true;
                    /*
                    Vector3 once = target.GetComponent<SpriteRenderer>().bounds.extents;
                    if (once.y < once.x)
                    {
                        if (transform.parent.position.y < target.transform.position.y)
                            RedSide = SideRedLine.T;
                        else
                            RedSide = SideRedLine.B;
                    }
                    else
                    {
                        if (transform.parent.position.x < target.transform.position.x)
                            RedSide = SideRedLine.R;
                        else
                            RedSide = SideRedLine.L;
                    }*/
                }
                //橋基地同士の距離が橋以上のとき
                else
                    GrapLinger();
            }

            if (MakeOk)
                MakeBridge(target, nextBase);
        }

        //ターゲット橋ベースが無い場合。グラップリング用
        if (target == null)
            GrapLinger();
    }


    //this.MakeBridgeCheck()->
    void MakeBridge(GameObject prevBB, GameObject nextBB)
    {

        //前回の橋が残っているとき破棄
        if (BridgeObj)
            Destroy(BridgeObj);

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
        //生成
        BridgeObj = Instantiate(Bridge, _vec, Quaternion.Euler(180, 0, _Angle));
        nDCount--;
        if (nDCount > 0)
            _UICanvas.ChangeNum(nDCount);
        // ゲームオーバーか判定
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

            _vec = new Vector3(
                Mathf.Abs(mesh.bounds.extents.x),
                -Mathf.Abs(mesh.bounds.extents.y),
                -Mathf.Abs(mesh.bounds.extents.z));
            Vector3 BRB = bridgePos + _vec;

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
        bool GrapOK = false;

        if (!GrapLing)
        {
            GameObject land = null;
            GameObject onebridgebase = null;

            foreach (GameObject ground in sidebox.BoxInGround)
            {
                //画像の縦横をとり、地面が横広のとき、グラップリング対象とする
                Vector3 exvec = ground.GetComponent<SpriteRenderer>().bounds.extents;
                //正面に来ている地面のみ
                if (ground.transform.position.z <= sidebox.transform.position.z - exvec.z)
                {
                    if (exvec.x > exvec.y)
                    {
                        //地面幅のなかにプレイヤーが存在するなら
                        if (ground.transform.position.x - exvec.x < transform.parent.position.x && transform.parent.position.x < ground.transform.position.x + exvec.x)
                        {

                            if (transform.parent.position.y < ground.transform.position.y)
                                if (land == null)
                                    land = ground;
                                else
                                if (land.transform.position.y < ground.transform.position.y)
                                    land = ground;

                            //その地面の上にプレイヤーがいるなら
                            if (transform.parent.position.y - Player_verticalhorizontal.y - 0.1f < ground.transform.position.y + exvec.y
                                && ground.transform.position.y < transform.parent.position.y)
                                GrapOK = true;
                        }
                    }
                }
            }

            //間に地面がないとき
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
                                onebridgebase = obj;
                            }
                        }
                    }
                }
            }



            //グラップリング処理
            //ただし移動できない壁後付けするため注意。（製作途中）
            if (rb.isKinematic == true || rb.velocity.y == 0 || GrapOK)
            {
                if (!GrapLing)
                {

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
                    //地面の位置
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
                                sidebox.RollSwitchAction();

                                if (BridgeObj) Destroy(BridgeObj);

                                this.Moving = false;
                                GrapLing = true;
                                nowIE = GrapAttack();
                                StartCoroutine(nowIE);
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
                                    Vector3 point = new Vector3(transform.parent.position.x, BridgeObj.transform.position.y - bridgeextent.y - Player_verticalhorizontal.y, transform.parent.position.z);
                                    nowIE = Graplinger(point);
                                    StartCoroutine(nowIE);
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
                                    Vector3 point = new Vector3(transform.parent.position.x, land.transform.position.y + exvec.y + Player_verticalhorizontal.y, transform.parent.position.z);
                                    nowIE = Graplinger(point);
                                    StartCoroutine(nowIE);
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
                                Vector3 point = new Vector3(transform.parent.position.x, onebridgebase.transform.position.y - exvec.y + Player_verticalhorizontal.y, transform.parent.position.z);
                                nowIE = Graplinger(point);
                                StartCoroutine(nowIE);
                                GrapLing = true;
                            }
                        }
                    }
                    if (!GrapLing)
                    {
                        //間に何もないとき
                        if (Front_LeftTop.y <= pos.y && Front_LeftTop.y <= pos5.y)
                        {
                            //上限まで伸ばす
                            Vector3 point = new Vector3(transform.parent.position.x, RollAriaLeftTop.y - Player_verticalhorizontal.y + 0.1f, transform.parent.position.z);
                            nowIE = Graplinger(point);
                            StartCoroutine(nowIE);
                            GrapLing = true;
                        }
                    }
                }
            }
        }
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
        set { _bControll = value; /*rb.isKinematic = false; */}
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

             NowPlauerMovePoint = transform.parent.position = PPos;

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
            NowPlauerMovePoint = transform.parent.position = PPos;
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
                epos = BridgeObj.transform.position + new Vector3(range.x + (Player_verticalhorizontal.x + 0.01f), 0);
            else
                epos = BridgeObj.transform.position - new Vector3(range.x + (Player_verticalhorizontal.x + 0.01f), 0);
        }
        //縦向きの橋
        else
        {
            Spos = new Vector3(BridgeObj.transform.position.x, transform.parent.position.y, transform.parent.position.z);
            //Playerが橋の下側
            if (Spos.y < BridgeObj.transform.position.y)
                epos = BridgeObj.transform.position + new Vector3(0, range.y + (Player_verticalhorizontal.y + 0.01f));
            else
                epos = BridgeObj.transform.position - new Vector3(0, range.y + (Player_verticalhorizontal.y + 0.01f));
        }

        //橋の中心線との距離を得て、橋の中心線に向けて移動させる
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

        NowPlauerMovePoint = transform.parent.position = Spos;

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

        NowPlauerMovePoint = transform.parent.position = epos;

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
        Moving = true;
        //rb.isKinematic = true;のままで！(仮)
    }
    //========================================================
    // グラップリングしたときのプレイヤー処理
    //========================================================
    IEnumerator Graplinger(Vector3 point)
    {
        SphereCollider sCollider = transform.parent.GetComponent<SphereCollider>();
        sCollider.enabled = false;
        Vector3 Ppos = transform.parent.position;
        float timer = 0;

        while (!OnBridge)
        {
            transform.parent.position = Vector3.Lerp(Ppos, point, timer);
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
        NowPlauerMovePoint = transform.parent.position = point;
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
}
