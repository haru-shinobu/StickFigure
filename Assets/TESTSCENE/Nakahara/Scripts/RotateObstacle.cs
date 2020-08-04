using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObstacle : MonoBehaviour
{
    // 回転
    public bool _axisX;
    public bool _axisY;
    public bool _axisZ;
    
    private float _speed;

    //============================================================
    // コンストラクタ
    //============================================================
    void Start()
    {
        _speed = 0.05f;
    }

    //============================================================
    // 更新
    //============================================================
    void Update()
    {
        if (_axisX)
        {
            transform.Rotate(new Vector3(_speed, 0, 0));
        }
        else if (_axisY)
        {
            transform.Rotate(new Vector3(0, _speed, 0));
        }
        else if (_axisZ)
        {
            transform.Rotate(new Vector3(0, 0, _speed));
        }
    }
}
