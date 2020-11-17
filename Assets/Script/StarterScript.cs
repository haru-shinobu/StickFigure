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

    GameObject SoundObj;

    [SerializeField]
    GameObject Wind;
    [SerializeField]
    GameObject SideWall_child;
    
    //==================================================================
    // ゲームスタート時のプレイヤーの開始壁をセット
    //==================================================================
    void Awake()
    {
        Box_PlayerController BoxPlayer = GameObject.FindWithTag("Player").GetComponent<Box_PlayerController>();

        var sideBox = StartBox.GetComponent<SideColorBoxScript>();
        BoxPlayer.SetNextBox(sideBox);
        BoxPlayer.transform.parent.position = sideBox.transform.position;

        sideBox.SetBoxPos(BoxPlayer);

        //REDline設定
        G_data = GetComponent<GameData>();
        G_data.RedLine = sideBox.transform.root.localScale.x;
        //StartBox設定
        G_data.P_Now_Box = StartBox.transform.root.gameObject;

        G_data.Bases = GameObject.FindGameObjectsWithTag("BridgeBase");
        G_data.WindPrefab = Wind;

        SoundObj = GameObject.Find("SoundObj");
        if (SoundObj)
            SoundObj.GetComponent<SoundManager>().BGMState();
        var ex = SideWall_child.GetComponent<SpriteRenderer>().bounds.extents;
        G_data.SideWall_Offset = (ex.x < ex.y) ? ex.x : ex.y;
    }

    void Start()
    {
        Destroy(this);
    }
}
