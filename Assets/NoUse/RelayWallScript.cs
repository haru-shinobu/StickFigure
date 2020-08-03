using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(-1)]
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

    public bool RearOn;


    [SerializeField]
    GameObject PrimitiveCube;
    
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

        //プリミティブ(原型)のCube生成 -> 床に加工
        PrimitiveCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        PrimitiveCube.AddComponent<BoxCollider>();
        PrimitiveCube.transform.forward = transform.forward;
        
        var PrimitiveCubepos = transform.position - transform.forward * PrimitiveCube.transform.lossyScale.z * 0.5f;
        PrimitiveCubepos.y = RightBottom.y;
        PrimitiveCube.transform.localPosition = PrimitiveCubepos;
        PrimitiveCube.transform.localScale = new Vector3(Sprite_half_DistanceX * 2, 1, 1);
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
        if (side)
        {
            if (RightWall)
                return true;
        }
        else
        {
            if (LeftWall)
                return true;
        }
        return false;
    }
    //----------------------------------------------
    // 床
    public float GetDepth()
    {
        return PrimitiveCube.transform.localScale.z * 0.5f;
    }
    //==================================================================


}
