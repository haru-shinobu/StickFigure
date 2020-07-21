using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_PlayerController : MonoBehaviour
{
    [SerializeField, Range(1, 5)]
    float Speed = 3;
    bool _bControll = true;

    //プレイヤーの縦横の半分を記録
    Vector2 Player_verticalhorizontal;
    ///プレイヤーの縦横移動範囲設定、壁移動のたび再記録
    Vector3 MoveAriaLeftTop, MoveAriaRightBottom;

    [SerializeField, Header("橋の長さ,橋の幅")]
    Vector2 BridgeSpace;

    //移動判定するための接触壁
    GameObject decision_object;

    //壁移動時の切り替え速度
    ///(他スクリプトから値を渡す事。カメラや物体の変化速度と合わせるため)
    [Header("壁切替速度")]
    private float ChangeSpeed;

    BoxManager BManager;
    Vector3 m_Vec;
    BoxSurfaceScript boxwall;

    //=======================================================================
    //初期設定
    //=======================================================================
    void Start()
    {
        BManager = GameObject.FindWithTag("BoxManager").GetComponent<BoxManager>();
        Player_verticalhorizontal = transform.GetComponent<SpriteRenderer>().bounds.extents;
    }

    
    //基本的に操作を主に置く。判定系は別のブロックで行うようにしている
    void Update()
    {
        if(_bControll)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vartical = Input.GetAxis("Vertical");
            var Ppos = transform.position;
            // プレイヤー移動範囲
            if (boxwall.CheckPPos(Ppos))
                this.Move(horizontal, vartical);
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
            //壁際の判定など
        }

    }

    //=======================================================================
    // プレイヤー移動
    //=======================================================================
    void Move(float horizontal,float vartical)
    {
        if (horizontal > 0)
        {
            transform.localPosition += m_Vec * Speed * 0.01f;
        }
        if (horizontal < 0)
        {
            transform.localPosition -= m_Vec * Speed * 0.01f;
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
    // 壁移動コルーチンへ
    //=======================================================================
    public void MoveWall(GameObject nextwall)
    {
        StartCoroutine("WallShifter", nextwall);
    }

    //=======================================================================
    // 橋
    //=======================================================================
    void MakeBridge()
    {
        //vec2 (BridgeSpace)
    }

    //=======================================================================
    //壁移動時の切り替え速度
    //=======================================================================
    void SetChangeSpeed(float speed)
    {
        ChangeSpeed = speed;
    }
    IEnumerator WallShifter(GameObject nextwall)
    {
        yield return new WaitForEndOfFrame();

    }
}
