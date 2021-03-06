﻿/*
* @brief    サウンド管理クラス
*           シングルトン
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    //private static SoundManager _instance = null;
    //public static SoundManager _Instance { get { return _instance; } }


    [SerializeField] private GameObject soundManager;

    // BGM
    [SerializeField] private AudioClip titleBGM;         //タイトル
    [SerializeField] private AudioClip mainBGM;          //ゲーム
    [SerializeField] private AudioClip overBGM;      //ゲームオーバー
    [SerializeField] private AudioClip clearBGM;     //ゲームクリア

    // SE
    [SerializeField] private AudioClip jump;    //ジャンプ
    [SerializeField] private AudioClip move;    //移動
    [SerializeField] private AudioClip action;  //アクション
    [SerializeField] private AudioClip needle;  //ニードル
    [SerializeField] private AudioClip folling; //落下
    [SerializeField] private AudioClip button;  //ボタン
    [SerializeField] private AudioClip IronSteam;//アイロンスチーム
    [SerializeField] private AudioClip fire;    //炎
    [SerializeField] private AudioClip popper;  //クラッカー
    [SerializeField] private AudioClip prop;    //橋がはがれるとき
    public bool isBGM;

    private AudioSource BGMSource;

    private AudioSource OneShotSource;

    string sceneName;

    //private void Awake()
    //{
    //    if (_instance != null)
    //    {
    //        Destroy(this.gameObject);
    //    }
    //    else
    //    {
    //        _instance = this;
    //        DontDestroyOnLoad(this.gameObject);
    //    }
    //}

    private void Start()
    {
        //Debug.Log("SoundState");
        int soundPlay = FindObjectsOfType<SoundManager>().Length;
        DontDestroyOnLoad(soundManager);
        if (soundPlay > 1) { Destroy(gameObject); }
        BGMSource = GetComponent<AudioSource>();
        OneShotSource = GetComponent<AudioSource>();
        isBGM = false;
    }

    
    public void BGMState()
    {
        //Debug.Log("SoundState");
        DontDestroyOnLoad(soundManager);

        sceneName = SceneManager.GetActiveScene().name;//SceneManager.sceneCount;

        BGMSource = GetComponent<AudioSource>();
        BGMSource.Stop();
        isBGM = false;
    }

    // Update is called once per frame
    void Update()
    {
        BGM();
    }

    //=========================================================
    //BGM
    private void BGM()
    {
        if (!isBGM)
        {
            switch (sceneName)
            {
                case "Title": BGMSource.clip = titleBGM; break;
                case "GameMainScene": BGMSource.clip = mainBGM; break;
                case "GameOver": BGMSource.clip = overBGM; break;
                case "Clear": BGMSource.clip = clearBGM; break;
            }
//            Debug.Log("BGM" + sceneName);
            BGMSource.Play();
            isBGM = true;
        }
    }

    //=========================================================
    //SE

    // jump
    public void GrapSE()
    {
        //Debug.Log("jumpSE");
        OneShotSource.PlayOneShot(jump);
    }

    // move
    public void MoveSE()
    {
        //Debug.Log("MoveSE");
        OneShotSource.PlayOneShot(move);
    }

    // actionta
    public void ActionSE()
    {
        //Debug.Log("ActionSE");
        OneShotSource.PlayOneShot(action);
    }
    public void NeedleOutSE()
    {
        OneShotSource.PlayOneShot(needle);
    }
    public void FollSE()
    {
        OneShotSource.PlayOneShot(folling);
    }
    public void PushButtonSE()
    {
        OneShotSource.PlayOneShot(button);
    }
    public void IronSteamSE()
    {
        OneShotSource.PlayOneShot(IronSteam);
    }
    public void PoperSE()
    {
        OneShotSource.PlayOneShot(popper);
    }
    public void PropSE()
    {
        OneShotSource.PlayOneShot(prop);
    }
}