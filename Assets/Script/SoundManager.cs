/*
* @brief    サウンド管理クラス
*           シングルトン
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance = null;
    public static SoundManager _Instance { get { return _instance; } }


    [SerializeField] private GameObject soundManager;

    // BGM
    [SerializeField] private AudioClip titleBGM;         //タイトル
    [SerializeField] private AudioClip mainBGM;          //ゲーム
    [SerializeField] private AudioClip overBGM;      //ゲームオーバー
    [SerializeField] private AudioClip clearBGM;     //ゲームクリア

    // SE
    [SerializeField] private AudioClip jump;          //ジャンプ
    [SerializeField] private AudioClip move;   //移動
    [SerializeField] private AudioClip action;  //アクション


    public bool isBGM;

    private AudioSource[] audioSource;

    string sceneName;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start()
    {
        Debug.Log("SoundState");
        int soundPlay = FindObjectsOfType<SoundManager>().Length;
        DontDestroyOnLoad(soundManager);
        if (soundPlay > 1) { Destroy(gameObject); }
        audioSource = GetComponents<AudioSource>();
        isBGM = false;
    }

    
    public void BGMState()
    {
        Debug.Log("SoundState");
        DontDestroyOnLoad(soundManager);

        sceneName = SceneManager.GetActiveScene().name;//SceneManager.sceneCount;

        audioSource = GetComponents<AudioSource>();
        audioSource[0].Stop();
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
                case "TitleScene": audioSource[0].clip = titleBGM; break;
                case "MainScene": audioSource[0].clip = mainBGM; break;
                case "GameOver": audioSource[0].clip = overBGM; break;
                case "GameClear": audioSource[0].clip = clearBGM; break;
            }
            Debug.Log("BGM" + sceneName);
            audioSource[0].Play();
            isBGM = true;
        }
    }

    //=========================================================
    //SE

    // jump
    public void jumpSE()
    {
        Debug.Log("jumpSE");
        audioSource[1].PlayOneShot(jump);
    }

    // move
    public void MoveSE()
    {
        Debug.Log("MoveSE");
        audioSource[1].PlayOneShot(move);
    }

    // action
    public void ActionSE()
    {
        Debug.Log("ActionSE");
        audioSource[1].PlayOneShot(action);
    }
    
}