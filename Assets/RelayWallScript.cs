using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelayWallScript : MonoBehaviour
{
    [SerializeField,Header("左壁")]
    GameObject LeftWall;
    [SerializeField, Header("右壁")]
    GameObject RightWall;

    Vector3 LeftTop;
    Vector3 RightBottom;
    float Sprite_half_DistanceX;
    
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
        LeftLine.y = 0;
        RightLine.y = 0;
        Sprite_half_DistanceX = Vector3.Distance(LeftLine, RightLine) * 0.5f;
        
    }

    //==================================================================
    //以下設定受け渡し
    //==================================================================
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
}
