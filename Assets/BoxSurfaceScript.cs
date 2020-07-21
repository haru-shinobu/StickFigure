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
    BoxScript boxroot;
    //==================================================================
    // 画像の縦横の半分計算
    //==================================================================
    void Awake()
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

        boxroot = transform.root.GetComponent<BoxScript>();
    }


    void Update()
    {

    }
    //==================================================================
    // その面が正面に来た時に位置を記録
    //==================================================================
    public void came_to_front()
    {
        //面が回転しているときとしていないときの違いで処理が変わる…
        if (transform.root.up == Vector3.up || transform.root.up == -Vector3.up)
        {
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
            var _vec2 = new Vector3(extentsHalfPos.y, -extentsHalfPos.x, 0f);
            RightBottom = Sr.transform.TransformPoint(_vec2);
        }
    }
    //==================================================================
    // ターゲットが範囲内かどうか判定
    //==================================================================
    //Box_PlayerController Update()->
    public bool CheckPPos(Vector3 Ppos)
    {
        //範囲内のとき
        if (LeftTop.x < Ppos.x && Ppos.x < RightBottom.x)
            if (RightBottom.y < Ppos.y && Ppos.y < LeftTop.y)
                return true;
        return false;
    }
    //==================================================================
    // ターゲットがどの位置か判定
    //==================================================================
    //Box_PlayerController Update()->
    public void ChangeWalls(Transform Ptrs)
    {
        var Ppos = Ptrs.position;
        var playSc = Ptrs.GetComponent<Box_PlayerController>();

        var rollways = 0;
        GameObject nextwalls = null;
        if (transform.up == Vector3.up)
        {
            //上下左右
            if (Ppos.y >= LeftTop.y)
            {
                nextwalls = TopWall;
                rollways = 1;
            }
            if (Ppos.y <= RightBottom.y)
            {
                nextwalls = BottomWall;
                rollways = 2;
            }
            if (Ppos.x <= LeftTop.x)
            {
                nextwalls = LeftWall;
                rollways = 3;
            }
            if (Ppos.x >= RightBottom.x)
            {
                nextwalls = RightWall;
                rollways = 4;
            }
        }
        if (transform.up == Vector3.right)
        {
            //上下左右
            if (Ppos.y >= LeftTop.y)
            {
                nextwalls = RightWall;
                rollways = 1;
            }
            if (Ppos.y <= RightBottom.y)
            {
                nextwalls = LeftWall;
                rollways = 2;
            }
            if (Ppos.x <= LeftTop.x)
            {
                nextwalls = TopWall;
                rollways = 3;
            }
            if (Ppos.x >= RightBottom.x)
            {
                nextwalls = BottomWall;
                rollways = 4;
            }
        }
        if (transform.up == Vector3.left)
        {
            //上下左右
            if (Ppos.y >= LeftTop.y)
            {
                nextwalls = LeftWall;
                rollways = 1;
            }
            if (Ppos.y <= RightBottom.y)
            {
                nextwalls = RightWall;
                rollways = 2;
            }
            if (Ppos.x <= LeftTop.x)
            {
                nextwalls = BottomWall;
                rollways = 3;
            }
            if (Ppos.x >= RightBottom.x)
            {
                nextwalls = TopWall;
                rollways = 4;
            }
        }
        if (transform.up == Vector3.down)
        {
            //上下左右
            if (Ppos.y >= LeftTop.y)
            {
                nextwalls = BottomWall;
                rollways = 1;
            }
            if (Ppos.y <= RightBottom.y)
            {
                nextwalls = TopWall;
                rollways = 2;
            }
            if (Ppos.x <= LeftTop.x)
            {
                nextwalls = RightWall;
                rollways = 3;
            }
            if (Ppos.x >= RightBottom.x)
            {

                nextwalls = LeftWall;
                rollways = 4;
            }
        }
        if (nextwalls != null)
        {
            Debug.Log(transform.name);
            Debug.Log(nextwalls.name);
            playSc.SetNextWall(nextwalls.GetComponent<BoxSurfaceScript>());
            Ptrs.SetParent(nextwalls.transform);
            //壁の回転
            boxroot.RollBlocks(rollways,nextwalls);
//            nextwalls.GetComponent<BoxSurfaceScript>().came_to_front();
        }
    }
    //==================================================================
    // 範囲内に強制的に収める
    //==================================================================
    //Box_PlayerController IEnumerator BlockRoller(Vector3 way_vec)->
    public void PosInWall(Transform player_Trs)
    {
        var Ppos = player_Trs.position;
        int a = 0;
        Debug.Log(Ppos);
        Debug.Log(LeftTop);
        Debug.Log(RightBottom);
        while (!CheckPPos(Ppos))
        {
            if (a++ > 40) break;
            if (LeftTop.x >= Ppos.x)
                Ppos.x = LeftTop.x + 0.1f;
            if (Ppos.x >= RightBottom.x)
                Ppos.x = RightBottom.x - 0.1f;
            if (RightBottom.y >= Ppos.y)
                Ppos.y = RightBottom.y + 0.1f;
            if (Ppos.y >= LeftTop.y)
                Ppos.y = LeftTop.y - 0.1f;
            player_Trs.position = Ppos;
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
