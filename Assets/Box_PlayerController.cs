using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_PlayerController : MonoBehaviour
{
    [SerializeField, Range(0.1f, 1)]
    float Speed = 0.4f;
    bool _bControll = false;

    //プレイヤーの縦横の半分を記録
    Vector2 Player_verticalhorizontal;
    ///プレイヤーの縦横移動範囲設定、壁移動のたび再記録
    Vector3 MoveAriaLeftTop, MoveAriaRightBottom;

    [SerializeField, Header("橋の長さ,橋の幅")]
    Vector2 BridgeSpace;

    CameraManager camM;
    Vector3 m_Vec;
    BoxSurfaceScript boxwall;
    SideColorBoxScript sidebox;

    void Start()
    {
        Player_verticalhorizontal = transform.GetComponent<SpriteRenderer>().bounds.extents;
        camM = Camera.main.GetComponent<CameraManager>();
    }
    
    
    void Update()
    {
        if(_bControll)
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
                var Ppos = transform.position;
                Debug.Log("EriaOut");
                _bControll = false;
                var rollways = 0;
                //上下左右
                if (Ppos.y > Front_LeftTop.y) rollways = 1;
                if (Ppos.y < Front_RightBottom.y) rollways = 2;
                if (Ppos.x < Front_LeftTop.x) rollways = 3;
                if (Ppos.x > Front_RightBottom.x) rollways = 4;
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
        //範囲内のとき
        if (MoveAriaLeftTop.x <= transform.position.x && transform.position.x <= MoveAriaRightBottom.x)
            if (MoveAriaRightBottom.y <= transform.position.y && transform.position.y <= MoveAriaLeftTop.y)
                return true;
        return false;
    }
    //=======================================================================
    // プレイヤー移動
    //=======================================================================
    //this->
    void Move(float horizontal,float vartical)
    {
        if (horizontal > 0)
        {
            transform.localPosition += Vector3.right * Speed * 0.01f;
        }
        else
        if (horizontal < 0)
        {
            transform.localPosition -= Vector3.right * Speed * 0.01f;
        }
        if (vartical > 0)
        {
            transform.localPosition += Vector3.up * Speed * 0.01f;
        }
        else
        if (vartical < 0)
        {
            transform.localPosition -= Vector3.up * Speed * 0.01f;
        }
    }

    //=======================================================================
    // 橋
    //=======================================================================
    //this->
    void MakeBridge()
    {
        //vec2 (BridgeSpace)
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
            var Ppos = transform.position;
            if (MoveAriaLeftTop.x >= Ppos.x)
                Ppos.x = MoveAriaLeftTop.x + 0.1f;
            if (Ppos.y >= MoveAriaLeftTop.y)
                Ppos.y = MoveAriaLeftTop.y - 0.1f;
            transform.position = Ppos;
            //Debug.Log("LT" + value +"P"+  Ppos);
        }
    }
    public Vector3 Front_RightBottom
    {
        get { return MoveAriaRightBottom; }
        set
        {
            MoveAriaRightBottom = value;
            var Ppos = transform.position;
            if (Ppos.x >= MoveAriaRightBottom.x)
                Ppos.x = MoveAriaRightBottom.x - 0.1f;
            if (MoveAriaRightBottom.y >= Ppos.y)
                Ppos.y = MoveAriaRightBottom.y + 0.1f;
            transform.position = Ppos;
            //Debug.Log("RB" + value + "P" +Ppos);
        }
    }
}
