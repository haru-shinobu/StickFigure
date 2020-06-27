using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField, Header("カメラの振り角度"), Range(0, 20)]
    float SwingWidth = 15f;
    [SerializeField, Header("壁移り速度"), Range(1, 10)]
    float WallChageSpeed = 10;

    //壁移り終わり判定用
    bool CamChangeEnd = false;
    bool PlayerChangeEnd = false;

    GameObject NowWall;

    CameraScript camSc;
    PlayerController Player;
    RelayWallScript WallSc;
    void Awake()
    {
        camSc = Camera.main.transform.GetComponent<CameraScript>();
        camSc.transform.parent = null;
        camSc.SetCamSwing(SwingWidth);
        

        NowWall = camSc.GetNowWall();

        Player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }
    //==================================================================
    // 壁が変更されたとき
    //==================================================================
    // CameraScript ReTarget() ->
    public void WallStageChange()
    {
        NowWall = camSc.GetNowWall();
        WallSc = NowWall.GetComponent<RelayWallScript>();
        Player.WallScript = WallSc;
        Player.SetPlayerMoveLimit();
        
    }
    //==================================================================
    // カメラとPlayerの向かう先の壁をセット
    //==================================================================
    // PlayerController PlayerOnStage() ->
    public void WallEnd(bool Side)//True = Right
    {
        MovePermit(false);
        GameObject TargetWall;
        if (Side)
        {
            TargetWall = WallSc.GetRightWall();
        }
        else
        {
            TargetWall = WallSc.GetLeftWall();
        }
        if (TargetWall)
        {
            //Stage変更Player用
            Player.StageChange(TargetWall);
            //Stage変更カメラ用
            camSc.UpdateTargetWall(TargetWall);
        }
    }
    //==================================================================
    // 全キャラの行動の許否    [true = 許可,false = 拒否]
    //==================================================================
    public void MovePermit(bool flag)
    {
        //プレイヤー
        Player.ControllJudge(flag);
        //カメラ
        camSc.SetControllJudge(flag);
        
    }
    //==================================================================
    // カメラ・プレイヤーの移動終わり検知用
    //==================================================================
    // PlayerController IEnumerator PlayerOnStageChange() ->
    // CameraScript IEnumerator CamTargetChange() ->
    public void MoveReStart(int type)//0はカメラ用・1はプレイヤー用
    {
        if (type == 1)
        {
            PlayerChangeEnd = true;
        }
        else
        {
            CamChangeEnd = true;
        }
        if (CamChangeEnd && PlayerChangeEnd)
        {
            Player.ControllJudge(true);
            camSc.SetControllJudge(true);
            CamChangeEnd = PlayerChangeEnd = false;
        }
    }
}
