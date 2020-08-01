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
        Player3D = GameObject.FindWithTag("Player3D").transform.GetChild(0).GetComponent<Player3DController>();
        
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
        //if (m_bDimention)
            Player3D.ControllJudge(flag);
        //else
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
            MovePermit(true);
            CamChangeEnd = PlayerChangeEnd = false;
        }
    }
    //
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
    // Player2D・3D切り替え  bDimen = true(2D->3D)：false(3D->2D)
    //==================================================================
    // DoorScript DoorAccess(bool bDimention,GameObject DoorAdress)->
    public void SetNowWallAcsess(bool bDimen, GameObject nextDoor)
    {
        MovePermit(false);
        m_bDimention = bDimen;

        var nextwall = nextDoor.GetComponent<DoorScript>().GetWall();
        
        //ドアが壁から浮いていた場合のため、ドアの壁への垂線の交点を求める
        var pointA = nextwall.transform.position;
        var pointB = nextwall.transform.right;
        var pointP = nextDoor.transform.position;
        var point = pointA + Vector3.Project(pointP - pointA, pointB - pointA);
        Transform Target;
        
        //移動先の壁面上ポジションをセット
        //3Dドア//Player 2D -> 3D
        if (m_bDimention)
        {
            
            Target = Player3D.transform.root;
            var Mr = nextDoor.GetComponent<MeshRenderer>();
            var halfY = Mr.bounds.extents.y;
            var vec = new Vector3(0, -halfY, 0);
            var top = Mr.transform.TransformPoint(vec);

            var ThalfY = Target.GetChild(0).GetComponent<MeshRenderer>().bounds.extents.y;

            point.y = 0;

            var depth = nextwall.GetComponent<RelayWallScript>().GetDepth();

            point = point - nextwall.transform.forward * depth;
            
        }
        //2Dドア//Player 3D -> 2D
        else
        {
            Target = Player.transform;
            var Sr = nextDoor.GetComponent<SpriteRenderer>();
            var halfY = Sr.sprite.bounds.extents.y;
            var vec = new Vector3(0, -halfY, 0);
            var top = Sr.transform.TransformPoint(vec);

            var ThalfY = Target.GetComponent<SpriteRenderer>().sprite.bounds.extents.y;
            point.y = top.y + ThalfY;
        }
        Target.position = point;

        //壁との向きを統一
        Target.transform.forward = nextwall.transform.forward;
        if (m_bDimention)
        {
            Debug.Log(Target.transform.position);
            Debug.Log(Target.GetChild(0).transform.localPosition);
        }
        //Stage変更カメラ用
        camSc.UpdateTargetWall(nextwall);
        if (m_bDimention)
            Player3D.WallScript = nextwall.GetComponent<RelayWallScript>();
        else
            Player.WallScript = nextwall.GetComponent<RelayWallScript>();
        //アクティブ切り替え
        Player.gameObject.SetActive(!m_bDimention);
        Player3D.gameObject.SetActive(m_bDimention);
        StartCoroutine("Later");
    }
    IEnumerator Later()
    {
        yield return new WaitForFixedUpdate();
        //プレイヤーの分の行動許可
        MoveReStart(1);
    }
}
