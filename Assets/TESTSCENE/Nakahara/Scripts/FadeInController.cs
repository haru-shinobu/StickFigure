using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInController : MonoBehaviour
{
    // フェードスピード
    private float _fadeSpeed = 0.001f;

    // 三原色
    private float _red, _green, _blue;

    // 透明度
    private float _alfa = 1f;

    //===========================================================
    // コンストラクタ
    //===========================================================
    void Start()
    {
        _red = GetComponent<Image>().color.r;
        _green = GetComponent<Image>().color.g;
        _blue = GetComponent<Image>().color.b;
    }

    //===========================================================
    // 更新
    //===========================================================
    void Update()
    {
        GetComponent<Image>().color = new Color(_red, _green, _blue, _alfa);
        _alfa -= _fadeSpeed;

        // 破棄
        if (_alfa < 0f)
        {
            Destroy(this.gameObject);
        }
    }
}
