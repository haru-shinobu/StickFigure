using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//==================================================================
// ゲームスタート時のみ使用するスクリプト
//==================================================================
public class StarterScript : MonoBehaviour
{
    [SerializeField, Header("スタートの壁")]
    GameObject StartWall;

    //==================================================================
    // ゲームスタート時のプレイヤーの開始壁をセット
    //==================================================================
    void Awake()
    {
        Box_PlayerController BoxPlayer = GameObject.FindWithTag("Player").GetComponent<Box_PlayerController>();
        BoxPlayer.SetNextWall(StartWall.GetComponent<BoxSurfaceScript>());
        BoxPlayer.transform.position = StartWall.transform.position;
        GetComponent<GameData>().P_on_Box = StartWall.transform.root.gameObject;
    }
    //==================================================================
    // スタート正面に来た時に位置を記録させる
    //==================================================================
    void Start()
    {
        StartWall.GetComponent<BoxSurfaceScript>().came_to_front();
        Destroy(this);
    }
}
