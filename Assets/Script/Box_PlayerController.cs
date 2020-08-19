using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_PlayerController : MonoBehaviour
{
    [SerializeField, Range(0.1f, 10)]
    float Speed = 0.4f;
    bool _bControll = false;
    bool _bRedLine = false;
    bool OnBridge = false;
    private float offset = 0.05f;
    //プレイヤーの縦横の半分を記録
    Vector2 Player_verticalhorizontal;
    ///プレイヤーの縦横移動範囲設定、壁移動のたび再記録
    Vector3 MoveAriaLeftTop, MoveAriaRightBottom;
    Vector3 BridgeAriaLeftTop, BridgeAriaRightBottom;

    [SerializeField, Header("テープ")]
    GameObject Bridge;
    [SerializeField, Header("テープの個数"), Range(1, 4)]
    private string bridgetag = "BridgeBase";
    
    CameraManager camM;
    Vector3 m_Vec;
    GameObject BridgeObj;
    
    SideColorBoxScript sidebox;
    GameData G_Data;
    GameObject nextBox;
    enum SideRedLine
    {
        T,B,L,R,Non,
    }
    SideRedLine RedSide = SideRedLine.Non;
    void Start()
    {
        var sprvec = transform.GetComponent<SpriteRenderer>();
        Player_verticalhorizontal = sprvec.bounds.extents;
        camM = Camera.main.GetComponent<CameraManager>();
        G_Data = GameObject.FindWithTag("BoxManager").GetComponent<GameData>();
        //以下2文はどのような形の画像でも１＊１の正方形にする処理。
        transform.localScale = new Vector3(1 / Player_verticalhorizontal.x / 2, 1 / Player_verticalhorizontal.y / 2, 1);
        Player_verticalhorizontal = sprvec.bounds.extents;
    }
    
    
    void Update()
    {
        if (Moving)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vartical = Input.GetAxis("Vertical");

            // プレイヤー移動範囲チェック
            //橋の上でないとき
            if (!CheckMoveBridgeAria())
            {
                // プレイヤー移動範囲チェック
                if (CheckMoveAria())
                {
                    this.Move(horizontal, vartical);
                }
                else
                {
                    Debug.Log("AriaOut");
                    this.Moving = false;
                    var T = transform.position.y + Player_verticalhorizontal.y;
                    var B = transform.position.y - Player_verticalhorizontal.y;
                    var L = transform.position.x - Player_verticalhorizontal.x;
                    var R = transform.position.x + Player_verticalhorizontal.x;

                    //上下左右
                    var rollways = 0;
                    if (T > Front_LeftTop.y) rollways = 1;
                    if (B < Front_RightBottom.y) rollways = 2;
                    if (L < Front_LeftTop.x) rollways = 3;
                    if (R > Front_RightBottom.x) rollways = 4;
                    if (BridgeObj) Destroy(BridgeObj);
                    sidebox.ChangeBoxRoll(transform, rollways);
                }

                //プレイヤーが箱の色幅にいるときは //camM.Side = true;
                //プレイヤーが箱の色幅にいないとき //camM.Side = false;

                
                if (Input.GetButton("Jump"))
                {
                    //橋の判定など
                    MakeBridge();
                }
            }
            else
            //橋の上のとき
            {
                if (Moving)
                {
                    StartCoroutine("InBridge");
                    this.Moving = false;
                }
            }
        }
    }
    //移動範囲確認(箱)
    bool CheckMoveAria()
    {
        var T = transform.position.y + Player_verticalhorizontal.y;
        var B = transform.position.y - Player_verticalhorizontal.y;
        var R = transform.position.x + Player_verticalhorizontal.x;
        var L = transform.position.x - Player_verticalhorizontal.x;

        //範囲内のとき
        if (MoveAriaLeftTop.x <= L && R <= MoveAriaRightBottom.x)
            if (MoveAriaRightBottom.y <= B && T <= MoveAriaLeftTop.y)
            {
                OnRedLine = false;
                return true;
            }
        OnRedLine = true;
        return false;
    }
    //移動範囲確認(橋)
    bool CheckMoveBridgeAria()
    {
        //プレイヤーが橋の上にいるときは //camM.Bridge = true;
        //プレイヤーが橋の上にいないとき //camM.Bridge = false;
        if (BridgeObj)
        {
            if (BridgeObj.transform.forward == Vector3.forward || BridgeObj.transform.forward == -Vector3.forward)
            {   
                var pos = transform.position;
                var T = transform.position.y + Player_verticalhorizontal.y;
                var B = transform.position.y - Player_verticalhorizontal.y;
                var R = transform.position.x + Player_verticalhorizontal.x;
                var L = transform.position.x - Player_verticalhorizontal.x;
                //範囲内に触れたとき
                if (BridgeAriaLT.x <= R && L <= BridgeAriaBR.x)
                    if (BridgeAriaBR.y <= T && B <= BridgeAriaLT.y)
                    {
                        pos.z = BridgeAriaLT.z;
                        transform.position = pos;
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
        if (MoveAriaLeftTop.x + G_Data.RedLine <= transform.position.x && transform.position.x <= MoveAriaRightBottom.x - G_Data.RedLine)
            if (MoveAriaRightBottom.y + G_Data.RedLine <= transform.position.y && transform.position.y <= MoveAriaLeftTop.y - G_Data.RedLine)
            {
                RedSide = SideRedLine.Non;
                return false;
            }
        //左右を優先
        if (MoveAriaLeftTop.x + G_Data.RedLine > transform.position.x)
            RedSide = SideRedLine.L;
        else if(transform.position.x > MoveAriaRightBottom.x - G_Data.RedLine)
            RedSide = SideRedLine.R;
        else
        if (MoveAriaRightBottom.y + G_Data.RedLine > transform.position.y)
            RedSide = SideRedLine.B;
        else if (transform.position.y > MoveAriaLeftTop.y - G_Data.RedLine)
            RedSide = SideRedLine.T;
        return true;
    }
    
    //=======================================================================
    // プレイヤー移動
    //=======================================================================
    //this->
    void Move(float horizontal,float vartical)
    {
        if (horizontal > 0)
        {
            transform.localPosition += Vector3.right * Speed;
        }
        else
        if (horizontal < 0)
        {
            transform.localPosition -= Vector3.right * Speed;
        }
        if (vartical > 0)
        {
            transform.localPosition += Vector3.up * Speed;
        }
        else
        if (vartical < 0)
        {
            transform.localPosition -= Vector3.up * Speed;
        }
    }

    //=======================================================================
    // 橋
    //=======================================================================
    //this->
    public void MakeBridge()
    {
        GameObject target = null;
        //前面にある橋のベースを探索
        for (int i = 0; i < sidebox.GetBridgeLine.Length; i++)
        {
            if (sidebox.transform.GetChild(i).tag == bridgetag)
            {
                if (sidebox.transform.GetChild(i).forward == Vector3.forward)
                {
                    var bridgebaseline = sidebox.transform.GetChild(i).GetComponent<SpriteRenderer>().bounds.extents;
                    Vector3 _vec = new Vector3(
                        -Mathf.Abs(bridgebaseline.x),
                         Mathf.Abs(bridgebaseline.y),
                         Mathf.Abs(bridgebaseline.z));
                    Vector3 FLT = sidebox.transform.GetChild(i).transform.position + _vec;

                    _vec = new Vector3(
                         Mathf.Abs(bridgebaseline.x),
                        -Mathf.Abs(bridgebaseline.y),
                        -Mathf.Abs(bridgebaseline.z));
                    Vector3 BRB = sidebox.transform.GetChild(i).transform.position + _vec;

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
                    var pos = transform.position;
                    var T = transform.position.y + Player_verticalhorizontal.y;
                    var B = transform.position.y - Player_verticalhorizontal.y;
                    var R = transform.position.x + Player_verticalhorizontal.x;
                    var L = transform.position.x - Player_verticalhorizontal.x;
                    //範囲内のとき
                    if (FLT.x <= R && L <= BRB.x)
                        if (BRB.y <= T && B <= FLT.y)
                        {
                            target = sidebox.transform.GetChild(i).gameObject;
                        }
                }
            }
        }
        bool MakeOk = false;
        //橋のベースの範囲内にプレイヤーがいたとき、そのベースがnullでなくなる
        if (target)
        {
            //全ベース取得
            GameObject[] objs = GameObject.FindGameObjectsWithTag("BridgeBase");
            nextBox = null;
            float distance = float.MaxValue;
            Vector3 posA = target.transform.position;
            bool check;
            //現在の箱と同じ箱についている橋のベース以外で一番近い橋Baseを得る
            foreach (GameObject obj in objs)
            {
                check = true;
                for (int i = 0; i < sidebox.GetBridgeLine.Length; i++)
                    if (sidebox.transform.GetChild(i).gameObject == obj)
                    {
                        check = false;
                        break;
                    }

                if (check)
                {
                    Vector3 posB = obj.transform.position;
                    float dis = Mathf.Abs(Vector3.Distance(posA, posB));
                    if (dis < distance)
                    {
                        distance = dis;
                        nextBox = obj;
                    }
                }
            }

            //橋基地同士の距離が橋以下のとき
            Vector3 bounds = Bridge.GetComponent<MeshRenderer>().bounds.extents * 2;
            if (bounds.x > bounds.y) bounds.y = bounds.x;
            //生成
            if (distance - 0.001f <= bounds.y) 
            {
                MakeOk = true;
            }
        }
        if (RedSide == SideRedLine.Non)
            MakeOk = true;

        if (MakeOk)
        {
            //前回の橋が残っているとき破棄
            if (BridgeObj)
                Destroy(BridgeObj);

            //各生成場所セット
            Vector3 _vec = transform.position;
            float _Angle = 0f;
            //プレイヤー位置を基準とするのではなく、赤ラインの半分(0.5f分)を基準とする。(橋の役割とき)
            switch (RedSide)
            {
                case SideRedLine.Non:
                    //プレイヤー足元位置を基準として上方向に配置(梯子の役割のとき)
                    _vec = transform.position + new Vector3(0, -Player_verticalhorizontal.y);
                    break;
                case SideRedLine.T:
                    //プレイヤーのいる赤位置判定からの場所と方向決定
                    _vec = new Vector3(transform.position.x, Front_LeftTop.y - G_Data.RedLine / 2);
                    break;
                case SideRedLine.B:
                    //プレイヤーのいる赤位置判定からの場所と方向決定
                    _vec = new Vector3(transform.position.x, Front_RightBottom.y + G_Data.RedLine / 2);
                    _Angle = 180f;
                    break;
                case SideRedLine.L:
                    //プレイヤーのいる赤位置判定からの場所と方向決定
                    _vec = new Vector3(Front_LeftTop.x + G_Data.RedLine / 2, transform.position.y);
                    _Angle = 90f;
                    break;
                case SideRedLine.R:
                    //プレイヤーのいる赤位置判定からの場所と方向決定
                    _vec = new Vector3(Front_RightBottom.x - G_Data.RedLine / 2, transform.position.y);
                    _Angle = 270f;
                    break;
            }
            //めり込むため位置微調整
            _vec.z = transform.position.z - 0.01f;
            //生成
            BridgeObj = Instantiate(Bridge, _vec, Quaternion.Euler(180, 0, _Angle));
        
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
                var bridgePos = BridgeObj.transform.position;
                MeshRenderer mesh = BridgeObj.transform.GetComponent<MeshRenderer>();

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
        set { _bControll = value; }
    }
    public bool OnRedLine
    {
        get { return _bRedLine; }
        set { _bRedLine = value; }
    }

    //行動範囲左上記録
    public Vector3 Front_LeftTop
    {
        get { return MoveAriaLeftTop; }
        set
        {
            MoveAriaLeftTop = value;

            var PPos = transform.position;
            var T = PPos.y + Player_verticalhorizontal.y;
            var L = PPos.x - Player_verticalhorizontal.x;

            if (MoveAriaLeftTop.x >= L)
                PPos.x = MoveAriaLeftTop.x + Player_verticalhorizontal.x + offset;
            if (T >= MoveAriaLeftTop.y)
                PPos.y = MoveAriaLeftTop.y - Player_verticalhorizontal.y - offset;

            transform.position = PPos;

        }
    }
    //行動範囲右下記録
    public Vector3 Front_RightBottom
    {
        get { return MoveAriaRightBottom; }
        set
        {
            MoveAriaRightBottom = value;

            var PPos = transform.position;
            var B = PPos.y - Player_verticalhorizontal.y;
            var R = PPos.x + Player_verticalhorizontal.x;
            
            if (R >= MoveAriaRightBottom.x)
                PPos.x = MoveAriaRightBottom.x - Player_verticalhorizontal.x - offset;
            if (MoveAriaRightBottom.y >= B)
                PPos.y = MoveAriaRightBottom.y + Player_verticalhorizontal.y + offset;
            transform.position = PPos;
        }
    }
    //========================================================
    // 橋の上に乗ったときのプレイヤー処理
    //========================================================
    IEnumerator InBridge()
    {
        float timer = 0;
        var ppos = transform.position;
        var Spos = transform.position;
        Vector3 epos;
        Vector3 range = BridgeObj.GetComponent<MeshRenderer>().bounds.extents;
        //横向きの橋
        if (range.x > range.y)
        {
            Spos = new Vector3(transform.position.x, BridgeObj.transform.position.y, transform.position.z);
            //Playerが橋の左側
            if (Spos.x < BridgeObj.transform.position.x)
                epos = BridgeObj.transform.position + new Vector3(range.x + Player_verticalhorizontal.x + 0.01f, 0);
            else
                epos = BridgeObj.transform.position - new Vector3(range.x + Player_verticalhorizontal.x + 0.01f, 0);
        }
        //縦向きの橋
        else
        {
            Spos = new Vector3(BridgeObj.transform.position.x, transform.position.y, transform.position.z);
            //Playerが橋の下側
            if (Spos.y < BridgeObj.transform.position.y)
                epos = BridgeObj.transform.position + new Vector3(0, range.y + Player_verticalhorizontal.y + 0.01f);
            else
                epos = BridgeObj.transform.position - new Vector3(0, range.y + Player_verticalhorizontal.y + 0.01f);
        }
        
        //橋の中心線との距離を得て、橋の中心線に向けて移動させる
        var distance = (ppos - Spos).magnitude;
        if (distance != 0)
            distance = 1 / distance;
        while (distance != 0)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime * distance;
            transform.position = Vector3.Lerp(ppos, Spos, timer);
            if(timer >= 1)
            break;
        }
        //橋の向こう側に向けて移動させる
        timer = 0;
        while (true)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(Spos, epos, timer);
            if (timer >= 1)
                break;
        }
        //----------------------------------------------------
        //橋の外にたどり着いたので各種セット
        //----------------------------------------------------
        //箱を登録
        var sideBox = nextBox.transform.parent.GetComponent<SideColorBoxScript>();
        SetNextBox(sideBox);
        //移動範囲を再びセット
        sideBox.SetBoxPos(this);
        G_Data.RedLine = sideBox.transform.root.localScale.x;
        
        if (G_Data.P_Now_Box != sidebox)
        {
            camM.SetNextBox(sideBox);
            G_Data.P_Now_Box = sidebox.transform.root.gameObject;
            Moving = true;
        }
    }
}
