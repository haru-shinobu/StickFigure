using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_PlayerController : MonoBehaviour
{
    [SerializeField, Range(1, 5)]
    float Speed = 3;
    bool _bControll = false;

    //プレイヤーの縦横の半分を記録
    Vector2 Player_verticalhorizontal;
    ///プレイヤーの縦横移動範囲設定、壁移動のたび再記録
    Vector3 MoveAriaLeftTop, MoveAriaRightBottom;

    [SerializeField, Header("橋の長さ,橋の幅")]
    Vector2 BridgeSpace;

    CameraManager camM;
    BoxManager BManager;
    Vector3 m_Vec;
    BoxSurfaceScript boxwall;


    void Start()
    {
        BManager = GameObject.FindWithTag("BoxManager").GetComponent<BoxManager>();
        Player_verticalhorizontal = transform.GetComponent<SpriteRenderer>().bounds.extents;
        camM = Camera.main.GetComponent<CameraManager>();
    }
    
    
    void Update()
    {
        if(_bControll)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vartical = Input.GetAxis("Vertical");
            var Ppos = transform.position;
            // プレイヤー移動範囲チェック
            if (boxwall.CheckPPos(Ppos))
                this.Move(horizontal, vartical);//移動
            else
            {
                _bControll = false;
                boxwall.ChangeWalls(this.transform);
            }

            if (Input.GetButton("Jump"))
            {
                //橋の判定など
                MakeBridge();
            }
        }
    }

    //=======================================================================
    // プレイヤー移動
    //=======================================================================
    void Move(float horizontal,float vartical)
    {
        if (horizontal > 0)
        {
            transform.localPosition += Vector3.right * Speed * 0.01f;
        }
        if (horizontal < 0)
        {
            transform.localPosition -= Vector3.right * Speed * 0.01f;
        }
        if (vartical > 0)
        {
            transform.localPosition += Vector3.up * Speed * 0.01f;
        }
        if (vartical < 0)
        {
            transform.localPosition -= Vector3.up * Speed * 0.01f;
        }
    }

    //=======================================================================
    // 橋
    //=======================================================================
    void MakeBridge()
    {
        //vec2 (BridgeSpace)
    }
    /// <summary>
    /// プレイヤー移動範囲計算
    /// </summary>
    //BoxScript WallInAria()->
    public void WallInAria()
    {
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
    /// <param name="nextwall"></param>
    public void SetNextWall(BoxSurfaceScript nextwall)
    {
        boxwall = nextwall;
    }
    /// <summary>
    /// プレイヤー行動許可
    /// </summary>
    /// <param name="flag"></param>
    public void SetMoving(bool flag)
    {
        _bControll = flag;
    }
    /// <summary>
    /// 箱を移ったとき
    /// </summary>
    /// <param name="nextBox"></param>
    public void SetNextBox(BoxScript nextBox)
    {
        camM.SetNextBox(nextBox);
    }
}
