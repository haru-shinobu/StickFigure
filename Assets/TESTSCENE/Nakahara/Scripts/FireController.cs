using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    // 生存時間
    [SerializeField]
    float _lifeTime;

    // 経過時間
    private float _elapsedTime = 0f;

    // 生存フラグ
    public bool _lifeFlag = true;

    // コンポーネント
    private SpriteRenderer _spriteRenderer;

    //===========================================================
    // コンストラクタ
    //===========================================================
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //===========================================================
    // 更新
    //===========================================================
    void Update()
    {
        // 点滅処理
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > _lifeTime && _lifeFlag) 
        {
            _lifeFlag = false;
            _elapsedTime = 0f;
            _spriteRenderer.color = new Color(1, 1, 1, 0);
        }
        else if(_elapsedTime > _lifeTime && !_lifeFlag)
        {
            _lifeFlag = true;
            _elapsedTime = 0f;
            _spriteRenderer.color = new Color(1, 1, 1, 1);
        }
    }
}
