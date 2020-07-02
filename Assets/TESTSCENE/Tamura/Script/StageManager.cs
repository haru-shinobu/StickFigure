using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(-1)]
public class StageManager : MonoBehaviour
{
    [SerializeField, Header("カメラの振り角度"), Range(0, 20)]
    float SwingWidth = 15f;
    [SerializeField, Header("壁移り速度"), Range(0.5f, 5)]
    float WallChageSpeed = 1;

    [SerializeField]
    GameObject StartWall;
    //壁移り終わり判定用
    bool CamChangeEnd = false;
    bool PlayerChangeEnd = false;

    //プレイヤーが3Dを操っているか//最初は3Dから‼
    bool m_bDimention = true;
    GameObject NowWall;

    CameraScript camSc;
    PlayerController Player;
    Player3DController Player3D;
    RelayWallScript WallSc;

    void Awake()
    {
        camSc = Camera.main.transform.GetComponent<CameraScript>();
        camSc.transform.parent = null;
        camSc.SetCamSwing(SwingWidth);

        //StartWall
        camSc.SetNowWall(StartWall);
        NowWall = StartWall;
        Player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        Player3D = GameObject.FindWithTag("Player3D").GetComponent<Player3DController>();
        
        //壁→壁への切り替え速度
        camSc.SetChangeWallSpeed(WallChageSpeed);
        Player.SetChangeWallSpeed(WallChageSpeed);
        Player3D.SetChangeWallSpeed(WallChageSpeed);
        //アクティブ変更
        Player3D.gameObject.SetActive(m_bDimention);
        Player.gameObject.SetActive(!m_bDimention);
    }
    //==================================================================
    // 壁が変更され終わったあとの現在壁を登録
    //==================================================================
    // CameraScript ReTarget() ->
    public void WallStageChange()
    {
        NowWall = camSc.GetNowWall();
        WallSc = NowWall.GetComponent<RelayWallScript>();
        if (m_bDimention)
        {
            Player3D.WallScript = WallSc;
            Player3D.SetPlayerMoveLimit();
        }
        else
        {
            Player.WallScript = WallSc;
            Player.SetPlayerMoveLimit();
        }
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
            if (m_bDimention)
                Player3D.StageChange(TargetWall);
            else
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
        if (m_bDimention)
            Player3D.ControllJudge(flag);
        else
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
            if (m_bDimention)
                Player3D.ControllJudge(true);
            else
                Player.ControllJudge(true);
            camSc.SetControllJudge(true);
            CamChangeEnd = PlayerChangeEnd = false;
        }
    }
    //==================================================================
    // 現在2Dか3Dか
    //==================================================================
    public bool GetJudge3D()
    {
        return m_bDimention;
    }
    //==================================================================
    // 2Dか3D操る次元切り替え
    //==================================================================
    public void SetChangeDimention(bool type)
    {
        m_bDimention = type;
    }
    //==================================================================
    // 始まりの壁
    //==================================================================
    public GameObject GetStartWall()
    {
        return StartWall;
    }
    //==================================================================
    // プレイヤーセット
    //==================================================================
    public void SetNow(GameObject playerobj)
    {
        ;
    }

}
