using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(-1)]
public class BoxSurfaceScript : MonoBehaviour
{
    //各壁がNoneのとき、Playerは壁範囲内に行動制限
    GameObject TopWall;
    GameObject BottomWall;
    GameObject LeftWall;
    GameObject RightWall;

    Vector3 LeftTop;
    Vector3 RightBottom;
    float Sprite_half_DistanceX;
    float Sprite_half_DistanceY;

    SpriteRenderer Sr;
    Vector2 extentsHalfPos;
    //==================================================================
    // 
    //==================================================================
    void Start()
    {
        Sr = GetComponent<SpriteRenderer>();
        var _sprite = Sr.sprite;
        var _halfX = _sprite.bounds.extents.x;
        var _halfY = _sprite.bounds.extents.y;
        
        extentsHalfPos = new Vector2(_halfX, _halfY);

        //壁の原点から壁の端の距離を算出
        var _vec = new Vector3(-extentsHalfPos.x, extentsHalfPos.y, 0f);
        var _vec2 = new Vector3(extentsHalfPos.x, -extentsHalfPos.y, 0f);
        var LeftLine = Sr.transform.TransformPoint(_vec);
        var RightLine = Sr.transform.TransformPoint(_vec2);
        var sLeftLine = LeftLine;
        var sRightLine = RightLine;
        LeftLine.y = RightLine.y = 0;
        Sprite_half_DistanceX = Vector3.Distance(LeftLine, RightLine) * 0.5f;
        sLeftLine.x = sRightLine.x = 0;
        Sprite_half_DistanceY = Vector3.Distance(sLeftLine, sRightLine) * 0.5f;
    }

    
    void Update()
    {
        
    }
    //==================================================================
    // その面が正面に来た時に位置を記録
    //==================================================================
    void came_to_front()
    {
        //面が回転しているときとしていないときの違いで処理が変わる…
        if (transform.root.up == Vector3.up || transform.root.up == -Vector3.up) {
            //各壁のLeftTopを記録
            var _vec = new Vector3(-extentsHalfPos.x, extentsHalfPos.y, 0f);
            LeftTop = Sr.transform.TransformPoint(_vec);

            //各壁のRightBottomを記録
            var _vec2 = new Vector3(extentsHalfPos.x, -extentsHalfPos.y, 0f);
            RightBottom = Sr.transform.TransformPoint(_vec2);
        }
        else
        {
            var _vec = new Vector3(-extentsHalfPos.y, extentsHalfPos.x, 0f);
            LeftTop = Sr.transform.TransformPoint(_vec);

            //各壁のRightBottomを記録
            var _vec2 = new Vector3(-extentsHalfPos.y, extentsHalfPos.x, 0f);
            RightBottom = Sr.transform.TransformPoint(_vec2);
        }
    }
    //==================================================================
    // ターゲットが範囲内かどうか判定
    //==================================================================
    public bool CheckPPos(Vector3 Ppos)
    {
        //範囲内のとき
        if(LeftTop.x < Ppos.x && Ppos.x < RightBottom.x)
            if (RightBottom.y < Ppos.y && Ppos.y < LeftTop.y)
                return true;
        return false;
    }
    //==================================================================
    // ターゲットがどの位置か判定
    //==================================================================
    public void ChangeWalls(Transform Ptrs)
    {
        var Ppos = Ptrs.position;
        var playSc = Ptrs.GetComponent<Box_PlayerController>();
        
        if(transform.up == Vector3.up)
        {
            //上下左右
            if (Ppos.y > LeftTop.y) playSc.MoveWall(TopWall);
            if (Ppos.y < RightBottom.y) playSc.MoveWall(BottomWall);
            if (Ppos.x < LeftTop.x) playSc.MoveWall(LeftWall);
            if (Ppos.x > RightBottom.x) playSc.MoveWall(RightWall);
        }
        if (transform.up == Vector3.right)
        {
            //上下左右
            if (Ppos.y > LeftTop.y) playSc.MoveWall(RightWall);
            if (Ppos.y < RightBottom.y) playSc.MoveWall(LeftWall);
            if (Ppos.x < LeftTop.x) playSc.MoveWall(TopWall);
            if (Ppos.x > RightBottom.x) playSc.MoveWall(BottomWall);
        }
        if (transform.up == Vector3.left)
        {
            //上下左右
            if (Ppos.y > LeftTop.y) playSc.MoveWall(LeftWall);
            if (Ppos.y < RightBottom.y) playSc.MoveWall(RightWall);
            if (Ppos.x < LeftTop.x) playSc.MoveWall(BottomWall);
            if (Ppos.x > RightBottom.x) playSc.MoveWall(TopWall);
        }
        if (transform.up == Vector3.down)
        {
            //上下左右
            if (Ppos.y > LeftTop.y) playSc.MoveWall(BottomWall);
            if (Ppos.y < RightBottom.y) playSc.MoveWall(TopWall);
            if (Ppos.x < LeftTop.x) playSc.MoveWall(RightWall);
            if (Ppos.x > RightBottom.x) playSc.MoveWall(LeftWall);
        }
    }
    //==================================================================
    // 以下設定受け渡し
    //  表壁
    //----------------------------------------------
    //壁をセット
    /// <summary>
    /// top, bottom ,left ,right
    /// </summary>
    public void SetFourWall(GameObject top,GameObject bottom,GameObject left,GameObject right)
    {
        TopWall = top;
        BottomWall = bottom;
        LeftWall = left;
        RightWall = right;
    }

    //ワールド座標での画像の左上位置
    public Vector3 GetWallAriaLT()
    {
        return LeftTop;
    }
    //ワールド座標での画像の右下位置
    public Vector3 GetWallAriaRB()
    {
        return RightBottom;
    }
    //上の面
    /// <returns>GameObject</returns>
    public GameObject GetTopWall()
    {
        return TopWall;
    }
    //下の面
    /// <returns>GameObject</returns>
    public GameObject GetBottomWall()
    {
        return BottomWall;
    }
    //左の面
    /// <returns>GameObject</returns>
    public GameObject GetLeftWall()
    {
        return LeftWall;
    }
    //右の面
    /// <returns>GameObject</returns>
    public GameObject GetRightWall()
    {
        return RightWall;
    }
    //画像の中心から左右の端までの距離
    public float GetHalfX()
    {
        return Sprite_half_DistanceX;
    }
    //画像の中心から上下の端までの距離
    public float GetHalfY()
    {
        return Sprite_half_DistanceY;
    }
    ///<summary>
    ///移動先の壁の有無確認
    ///int 0=top,1=bottom,2=left,3=right
    ///</summary> 
    public bool FindWall(int direction)
    {
        switch (direction)
        {
            case 0:
                if (TopWall)
                    return true;
                break;
            case 1:
                if (BottomWall)
                    return true;
                break;
            case 2:
                if (LeftWall)
                    return true;
                break;
            case 3:
                if (RightWall)
                    return true;
                break;
        }
        return false;
    }
    //==================================================================
}
