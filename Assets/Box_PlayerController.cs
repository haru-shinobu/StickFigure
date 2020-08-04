using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_PlayerController : MonoBehaviour
{
    [SerializeField, Range(0.1f, 10)]
    float Speed = 0.4f;
    bool _bControll = false;
    private float offset = 0.05f;
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
        var sprvec = transform.GetComponent<SpriteRenderer>();
        Player_verticalhorizontal = sprvec.bounds.extents;
        camM = Camera.main.GetComponent<CameraManager>();
        //以下2文はどのような形の画像でも１＊１の正方形にする処理。
        transform.localScale = new Vector3(1 / Player_verticalhorizontal.x / 2, 1 / Player_verticalhorizontal.y / 2, 1);
        Player_verticalhorizontal = sprvec.bounds.extents;
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
                Debug.Log("AriaOut");
                _bControll = false;
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
