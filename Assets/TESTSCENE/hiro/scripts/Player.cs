using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField, Range(0.1f, 10)]
    float Speed = 0.4f;
    bool _bControll = false;
    bool _bRedLine = false;
    private float offset = 0.05f;
    //プレイヤーの縦横の半分を記録
    Vector2 Player_verticalhorizontal;
    ///プレイヤーの縦横移動範囲設定、壁移動のたび再記録
    Vector3 MoveAriaLeftTop, MoveAriaRightBottom;

    [SerializeField, Header("テープ")]
    GameObject Bridge;
    [SerializeField, Header("テープの個数"), Range(1, 4)]
    int BridgeMaxNum;
    int BridgeNum = 0;
    GameObject[] Bridges;

    CameraManager camM;
    Vector3 m_Vec;

    BoxSurfaceScript boxwall;
    SideColorBoxScript sidebox;
    GameData G_Data;

    enum SideRedLine
    {
        T, B, L, R, Non,
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
        Bridges = new GameObject[BridgeMaxNum];
        BridgeNum = 0;
    }


    void Update()
    {
        if (_bControll)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vartical = Input.GetAxis("Vertical");

            // プレイヤー移動範囲チェック
            /*//箱不使用
            if (boxwall.CheckPPos(Ppos))
                this.Move(horizontal, vartical);//移動
            else
            {
                _bControll = false;
                boxwall.ChangeWalls(this.transform);
            }
            */
            // プレイヤー移動範囲チェック
            if (CheckMoveAria())
                this.Move(horizontal, vartical);
            else
            {
                Debug.Log("AriaOut");
                this.Moving = false;
                var Ppos = transform.position;
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
                sidebox.ChangeBoxRoll(transform, rollways);
            }

            //プレイヤーが箱の色幅にいるときは //camM.Side = true;
            //プレイヤーが箱の色幅にいないとき //camM.Side = false;
            //プレイヤーが橋の上にいるときは //camM.Bridge = true;
            //プレイヤーが橋の上にいないとき //camM.Bridge = false;

            if (Input.GetButton("Jump"))
            {
                //橋の判定など
                MakeBridge();
            }
        }
    }
    //移動範囲確認
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
        else if (transform.position.x > MoveAriaRightBottom.x - G_Data.RedLine)
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
    void Move(float horizontal, float vartical)
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
    void MakeBridge()
    {
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
        if (Bridges[BridgeNum])
            Destroy(Bridges[BridgeNum]);
        _vec.z = transform.position.z - 0.01f;
        Bridges[BridgeNum] = Instantiate(Bridge, _vec, Quaternion.Euler(180, 0, _Angle));

        switch (RedSide)
        {
            case SideRedLine.Non: Bridges[BridgeNum].transform.position += new Vector3(0, (Bridges[BridgeNum].GetComponent<MeshRenderer>().bounds.extents.y)); break;
            case SideRedLine.T: Bridges[BridgeNum].transform.position += new Vector3(0, (Bridges[BridgeNum].GetComponent<MeshRenderer>().bounds.extents.y)); break;
            case SideRedLine.B: Bridges[BridgeNum].transform.position -= new Vector3(0, (Bridges[BridgeNum].GetComponent<MeshRenderer>().bounds.extents.y)); break;
            case SideRedLine.L: Bridges[BridgeNum].transform.position -= new Vector3((Bridges[BridgeNum].GetComponent<MeshRenderer>().bounds.extents.x), 0); break;
            case SideRedLine.R: Bridges[BridgeNum].transform.position += new Vector3((Bridges[BridgeNum].GetComponent<MeshRenderer>().bounds.extents.x), 0); break;
        }
        BridgeNum++;
        if (BridgeMaxNum <= BridgeNum)
            BridgeNum = 0;
    }
    /// <summary>
    /// プレイヤー移動範囲計算
    /// </summary>
    //BoxScript WallInAria()->
    public void WallInAria()
    {//箱不使用
        boxwall.came_to_front();
        var Ppos = transform.position;
        if (!boxwall.CheckPPos(Ppos))
        {
            boxwall.PosInWall(transform);
        }
    }

    //=======================================================================
    // 各種設定受け渡し
    //=======================================================================
    /// <summary>
    /// 次の壁を指定
    /// </summary>
    public void SetNextWall(BoxSurfaceScript nextwall)
    {//箱不使用
        boxwall = nextwall;
    }
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
    /// <summary>
    /// 箱を移ったとき
    /// </summary>
    //    public void SetNextBox(BoxScript nextBox)
    //    {
    //        camM.SetNextBox(nextBox);
    //    }

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
}
