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
    void Awake()
    {
        transform.GetChild(0).SetParent(null);
        var Sr = GetComponent<SpriteRenderer>();
        var _sprite = Sr.sprite;
        var _halfX = _sprite.bounds.extents.x;
        var _halfY = _sprite.bounds.extents.y;
        var _vec = new Vector3(-_halfX, _halfY, 0f);
        var _pos = Sr.transform.TransformPoint(_vec);
        LeftTop = _pos;

        var _vec2 = new Vector3(_halfX, -_halfY, 0f);
        var _pos2 = Sr.transform.TransformPoint(_vec2);
        RightBottom = _pos2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetWallAriaLT()
    {
        return LeftTop;
    }
    public Vector3 GetWallAriaRB()
    {
        return RightBottom;
    }
}
