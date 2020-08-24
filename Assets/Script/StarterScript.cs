using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//==================================================================
// ゲームスタート時のみ使用するスクリプト。役割終わったら破棄している
//==================================================================
public class StarterScript : MonoBehaviour
{
    [SerializeField, Header("スタートボックス")]
    GameObject StartBox;
    GameData G_data;

    //==================================================================
    // ゲームスタート時のプレイヤーの開始壁をセット
    //==================================================================
    void Awake()
    {
        Box_PlayerController BoxPlayer = GameObject.FindWithTag("Player").GetComponent<Box_PlayerController>();
        
        var sideBox = StartBox.GetComponent<SideColorBoxScript>();
        BoxPlayer.SetNextBox(sideBox);
        sideBox.SetStartPos(BoxPlayer);
        
        //REDline設定
        G_data = GetComponent<GameData>();
        G_data.RedLine = sideBox.transform.root.localScale.x;
        //StartBox設定
        G_data.P_Now_Box = StartBox.transform.root.gameObject;

        G_data.Bases = GameObject.FindGameObjectsWithTag("BridgeBase");
    }
    
    void Start()
    {
        Destroy(this);
    }
}
