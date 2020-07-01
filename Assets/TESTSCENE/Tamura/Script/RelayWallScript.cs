using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelayWallScript : MonoBehaviour
{
    //各壁がNoneのとき、Playerは壁範囲内に行動制限
    [SerializeField, Header("表左壁")]
    GameObject LeftWall;
    [SerializeField, Header("表右壁")]
    GameObject RightWall;
    
    Vector3 LeftTop;
    Vector3 RightBottom;

    float Sprite_half_DistanceX;


    bool InsideFind = false;
    [SerializeField, Header("奥壁")]
    GameObject InsideWall;

    Vector3 Inside_LeftTop;
    Vector3 Inside_RightBottom;

    float Inside_Sprite_half_DistanceX;
    [SerializeField, Header("表左壁")]
    GameObject Inside_LeftWall;
    [SerializeField, Header("表右壁")]
    GameObject Inside_RightWall;

    void Awake()
    {
        var Sr = GetComponent<SpriteRenderer>();
        var _sprite = Sr.sprite;
        var _halfX = _sprite.bounds.extents.x;
        var _halfY = _sprite.bounds.extents.y;
        //各壁のLeftTopを記録
        var _vec = new Vector3(-_halfX, _halfY, 0f);
        var _pos = Sr.transform.TransformPoint(_vec);
        LeftTop = _pos;
        //各壁のRightBottomを記録
        var _vec2 = new Vector3(_halfX, -_halfY, 0f);
        var _pos2 = Sr.transform.TransformPoint(_vec2);
        RightBottom = _pos2;

        //壁の原点から壁の端の距離を算出
        var LeftLine = LeftTop;
        var RightLine = RightBottom;
        LeftLine.y = RightLine.y = 0;
        Sprite_half_DistanceX = Vector3.Distance(LeftLine, RightLine) * 0.5f;
        if (InsideWall)
        {
            InsideFind = true;
            var InsideSr = InsideWall.GetComponent<SpriteRenderer>();
            var Inside_sprite = InsideSr.sprite;
            var Inside_halfX = Inside_sprite.bounds.extents.x;
            var Inside_halfY = Inside_sprite.bounds.extents.y;
            //裏壁のLeftTopを記録
            _vec = new Vector3(-Inside_halfX, Inside_halfY, 0f);
            _pos = InsideSr.transform.TransformPoint(_vec);
            Inside_LeftTop = _pos;
            //裏壁のRightBottomを記録
            _vec2 = new Vector3(Inside_halfX, -Inside_halfY, 0f);
            _pos2 = Sr.transform.TransformPoint(_vec2);
            Inside_RightBottom = _pos2;

            LeftLine = LeftTop;
            RightLine = RightBottom;
            LeftLine.y = RightLine.y = 0;
            Inside_Sprite_half_DistanceX = Vector3.Distance(LeftLine, RightLine) * 0.5f;
        }
    }

    //==================================================================
    // 以下設定受け渡し
    //  表壁
    //----------------------------------------------
    public Vector3 GetWallAriaLT()
    {
        return LeftTop;
    }
    public Vector3 GetWallAriaRB()
    {
        return RightBottom;
    }
    public GameObject GetLeftWall()
    {
        return LeftWall;
    }
    public GameObject GetRightWall()
    {
        return RightWall;
    }
    public float GetHalfX()
    {
        return Sprite_half_DistanceX;
    }
    public bool FindWall(bool side)
    {
        bool flag = false;
        if (side)
        {
            if (RightWall)
                flag = true;
        }
        else
        {
            if (LeftWall)
                flag = true;
        }
        return flag;
    }
    //----------------------------------------------
    //==================================================================


    //==================================================================
    // 以下設定受け渡し
    // 裏壁
    //----------------------------------------------
    public bool GetFindInsideWall()
    {
        return InsideFind;
    }

    public Vector3 GetInsideWallAriaLT()
    {
        return Inside_LeftTop;
    }
    public Vector3 GetInsideWallAriaRB()
    {
        return Inside_RightBottom;
    }
    public GameObject GetInsideLeftWall()
    {
        return Inside_LeftWall;
    }
    public GameObject GetInsideRightWall()
    {
        return Inside_RightWall;
    }
    public float GetInsideHalfX()
    {
        return Inside_Sprite_half_DistanceX;
    }
    public bool FindInsideWall(bool side)
    {
        bool flag = false;
        if (side)
        {
            if (Inside_RightWall)
                flag = true;
        }
        else
        {
            if (Inside_LeftWall)
                flag = true;
        }
        return flag;
    }
    //----------------------------------------------
    //==================================================================
}
