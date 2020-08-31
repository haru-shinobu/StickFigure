﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    private GameObject ImageObj;

    private Image image_10;
    private Image image_01;

    [SerializeField]
    Sprite[] spritePrefab;

    // Start is called before the first frame update
    void Start()
    {
        //_PManager = PlayerMObj.GetComponent<PlayerManager>();
        // 何故か要素数が受け取れない
        //spriteTimer = Resources.LoadAll<Sprite>("Canvas/Sprites/NChip");
        ImageObj = GameObject.Find("Image_10");
        image_10 = ImageObj.GetComponent<Image>();
        ImageObj = GameObject.Find("Image_01");
        image_01 = ImageObj.GetComponent<Image>();
    }

    
    // 一定フレームで処理
    private void FixedUpdate()
    {
        // PlayerScriptから数値を取得
        int nDeepCount = 12;

        image_10.sprite = spritePrefab[nDeepCount / 10];
        image_01.sprite = spritePrefab[nDeepCount % 10];
    }
}