using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Transform player;
    Rigidbody rb;

    [SerializeField, Range(1, 5)]
    float Speed = 3;
    
    bool m_bJump = false;
    bool m_bControll = true;

    bool m_bWallSide = false;
    
    enum WAY // simadawallで現在使用
    {
        RIGHT = -1,
        NORMAL = 0,
        LEFT = 1,
    }
    WAY way = WAY.NORMAL;
    float DistanceLimit;
    Vector3 MoveAriaLeftTop, MoveAriaRightBottom;
    Vector3 m_Vec;
    [Header("付けなくていい")]
    public RelayWallScript WallScript;
    StageManager stage;
    float ChangeWallSpeed;

    bool _bAccess = false;
    GameObject Access;

    void Start()
    {
        player = this.gameObject.transform;
        rb = player.GetComponent<Rigidbody>();
        m_bJump = false;
        player.transform.forward = WallScript.transform.forward;
        stage = GameObject.FindWithTag("Manager").GetComponent<StageManager>();
        DistanceLimit = WallScript.GetHalfX();
    }
    //==================================================================
    //プレイヤーの移動
    //==================================================================
    void Update()
    {
        if (m_bControll)
        {
            var leftright = Input.GetAxis("Horizontal");
            if (leftright > 0)
            {
                player.localPosition += m_Vec * Speed * 0.01f;
                way = WAY.RIGHT;
            }
            else
                if (leftright < 0)
            {
                player.localPosition -= m_Vec * Speed * 0.01f;
                way = WAY.LEFT;
            }

            if (Input.GetButton("Jump"))
            {
                if (!m_bJump)
                {
                    m_bJump = true;
                    rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
                }
            }
            else
            if (rb.velocity.y == 0) m_bJump = false;

            PlayerOnStage();
            if (Input.GetButton("Access"))
                if (Access)
                {
                    if (_bAccess)
                    {
                        if (Access.tag == "Door")
                            Access.SendMessage("DoorAccess", true);
                        //if(Access.tag == "Step")
                        //    Access.SendMessage("DoorAccess", true);   
                    }
                }
        }
    }
    
    //==================================================================
    //プレイヤーの壁移りの判定
    //==================================================================
    void PlayerOnStage()
    {
        //プレイヤーと壁の水平距離を計算
        var Ppos = player.transform.position;
        var wallpos = WallScript.transform.position;
        Ppos.y = wallpos.y = 0;
        var distance = Vector3.Distance(Ppos, wallpos);

        //壁の端を超えていた場合
        if (distance >= DistanceLimit) 
        {
            //行動禁止命令
            stage.MovePermit(false);
            
            //壁から見て左右判定  right = true 
            var diff = player.transform.position - WallScript.transform.position;
            var axis = Vector3.Cross(WallScript.transform.forward, diff);
            var Sideflag = axis.y > 0 ? true : false;

            //現在の左右
            m_bWallSide = Sideflag;
            
            
            //移動先の壁が終端のとき
            var findwall = WallScript.FindWall(Sideflag);
            //先が存在するとき
            if (findwall)
                stage.WallEnd(m_bWallSide);
            else
            {
                //ステージ範囲を超えた部分を戻す
                Vector3 pos;
                if (m_bWallSide)
                    pos = WallScript.GetWallAriaRB();
                else
                    pos = WallScript.GetWallAriaLT();
                pos.y = player.position.y;
                player.transform.position = pos;

                if (m_bWallSide)
                    player.localPosition -= m_Vec * Speed * 0.1f;
                else
                    player.localPosition += m_Vec * Speed * 0.1f;
                stage.MovePermit(true);
            }
        }
    }

    //==================================================================
    //プレイヤーの移動範囲設定
    //==================================================================
    // StageManager WallStageChange() ->
    public void SetPlayerMoveLimit()
    {
        MoveAriaLeftTop = WallScript.GetWallAriaLT();
        MoveAriaRightBottom = WallScript.GetWallAriaRB();
        DistanceLimit = WallScript.GetHalfX();
        m_Vec = WallScript.transform.right;
    }

    //==================================================================
    //プレイヤーの壁移り
    //-----------------------------------------------------
    public void StageChange(GameObject TargetWall)
    {
        StartCoroutine(PlayerOnStageChange(TargetWall));
    }
    IEnumerator PlayerOnStageChange(GameObject Target)
    {
        //判定前にステージ範囲を超えた部分を戻す
        Vector3 Ppos;
        if (m_bWallSide)
            Ppos = WallScript.GetWallAriaRB();
        else
            Ppos = WallScript.GetWallAriaLT();
        Ppos.y = player.position.y;
        player.transform.position = Ppos;
        
        //rigidbody
        rb.isKinematic = true;
        //プレイヤー現在位置
        var pos = player.transform.position;
        //プレイヤー現在角度
        var rot = player.localRotation.eulerAngles.y;
        //移動先角度
        var RotY = Target.transform.localRotation.eulerAngles.y;

        //移動先壁の端判定
        Vector3 movePos;
        var targetscript = Target.transform.GetComponent<RelayWallScript>();
        // 元の壁の右端から入った場合
        if (m_bWallSide)
            movePos = Target.transform.right * -DistanceLimit * 0.98f;
        // 元の壁の左端から入った場合
        else
            movePos = Target.transform.right * DistanceLimit * 0.98f;
        movePos.y = player.transform.position.y;
        movePos = Target.transform.position + movePos;

        float timer = 0;
        while (timer < 1) 
        {
            yield return new WaitForEndOfFrame();
        
            timer += Time.deltaTime * ChangeWallSpeed;
        
            //プレイヤーのポジション移動
            player.position = Vector3.Lerp(pos,movePos,timer);
            //移動先壁の向きを合わせる
            player.localRotation = Quaternion.Lerp(Quaternion.Euler(0, rot, 0), Quaternion.Euler(0, RotY, 0), timer);
        }
        player.position = movePos;
        player.localRotation = Quaternion.Euler(0, RotY, 0);
        //コントロール許可願い
        stage.MoveReStart(1);//プレイヤーは1を渡すこと
        rb.isKinematic = false;
    }
    //-----------------------------------------------------
    //==================================================================

    //=================================================================
    // アクティブ化したとき呼び出される
    //==================================================================
    void OnEnable()
    {
        //stage.SetNow(this.gameObject);
        //Debug.Log("2DPlayer!");
    }

    //==================================================================
    // コライダ(主にドア、階段)
    //==================================================================
    void OnTriggerEnter(Collider other)
    {
        if (m_bControll)
            if (other.tag == "Door")
            {
                _bAccess = true;
                Access = other.gameObject;
            }
        //other.SendMessage("DoorAccess", true);//3D->2D
    }
    void OnTriggerExit(Collider other)
    {
        //if (other.tag == "Door")
        //    Debug.Log("出た");
        _bAccess = false;
    }

    //==================================================================
    //以下設定受け渡し
    //==================================================================
    //コントロール
    public void ControllJudge(bool flag)
    {
        m_bControll = flag;
    }
    //移動の向き
    public int Getways()
    {
        return (int)way;
    }
    public void SetChangeWallSpeed(float val)
    {
        ChangeWallSpeed = val;
    }
}
