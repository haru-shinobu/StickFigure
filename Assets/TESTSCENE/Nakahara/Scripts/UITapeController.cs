using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITapeController : MonoBehaviour
{
    // UIテープ
    [SerializeField]
    private GameObject[] _tape;

    // テープの数
    private int _numOfTapes = 5;

    // UIテープオブジェクト配列の添え字
    private int _tapeIndex;

    private bool _tapeFlag = false;

    //===========================================================
    // コンストラクタ
    //===========================================================
    void Start()
    {
        _tapeIndex = _numOfTapes - 1;
    }

    //===========================================================
    // 更新
    //===========================================================
    void Update()
    {
        UITapeUpdate();
    }

    //===========================================================
    // テープの消費と生産
    //===========================================================
    private void UITapeUpdate()
    {
        // ※条件式をプレイヤーがテープを消費・生産した時に変更※

        // テープの消費
        if (Input.GetKeyDown(KeyCode.A))
        {
            _tape[_tapeIndex].SetActive(false);
            _tapeIndex -= 1;
            if (_tapeIndex < 0)
            {
                _tapeIndex = 0;
                _tapeFlag = true;
            }
        }
        // テープの生産
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (_tapeFlag)
            {
                _tapeFlag = false;
            }
            else
            {
                _tapeIndex += 1;
            }
            
            if (_tapeIndex > _numOfTapes - 1) 
            {
                _tapeIndex = _numOfTapes - 1;
            }
            _tape[_tapeIndex].SetActive(true);
        }
    }
}
