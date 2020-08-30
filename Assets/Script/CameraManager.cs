using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField, Header("ゴール")]
    GameObject GoalObj;
    [SerializeField, Header("スタート")]
    GameObject StartObj;
    [SerializeField, Range(0.1f, 100)]
    float StartCamSpeed = 1;
    [SerializeField, Header("赤ライン内外カメラ移動(秒)"), Range(0.1f, 2)]
    float redline_cam_move_time;
    Box_PlayerController PSc;
    bool _bMoveOK = false;
    bool bSide_edge = false;
    bool bBridge = false;
    bool _bOnLine = false;


    bool _bCorutine_Action = false;
    IEnumerator routine;
    bool _bColutine_OnBox = false;
    bool _bColutine_OnBridge = false;
    [SerializeField]
    Vector3 StartCamera_Distance = new Vector3(0, 0, 40);
    [SerializeField]
    Vector3 Camera_Distance = new Vector3(0, 0, 20);
    GameObject NowBox;

    void Start()
    {
        NowBox = StartObj;
        redline_cam_move_time = (1 / redline_cam_move_time);
        PSc = GameObject.FindWithTag("Player").transform.GetComponent<Box_PlayerController>();
        if (GoalObj && StartObj)
        {
            var Cam_StartPos = GoalObj.transform.position;
            var Cam_EndPos = StartObj.transform.position;
            routine = Starter(Cam_StartPos, Cam_EndPos);
            StartCoroutine(routine);
        }
    }
    //===========================================================
    // プレイヤー移動によるカメラ処理
    //===========================================================
    void Update()
    {
        //ステージ見渡しムービー終了・カメラ動作OK時
        if (_bMoveOK)
        {
            //プレイヤーが橋の上か否か
            if (!Bridge)
            {
                if (!_bColutine_OnBox) {
                    _bCorutine_Action = false;
                    //コルーチンが動作していたら停止。動作させる。OnBoxコルーチンを動作させる
                    if (_bColutine_OnBridge)
                    {
                        StopCoroutine(routine);
                        routine = null;
                        _bColutine_OnBridge = false;
                    }
                    routine = OnBlockCam();
                    StartCoroutine(routine);
                }else
                if (_bCorutine_Action) {
                    Vector3 Rootpos = transform.root.position;
                    Vector3 Pscpos = PSc.transform.parent.position;
                    Rootpos.z = Pscpos.z = 0;
                    Vector3 pos = (Rootpos - Pscpos);
                    transform.position = Pscpos - pos - Camera_Distance;
                    transform.LookAt(transform.root.position);
                }
            }
            else//橋の上
            {
                if (!_bColutine_OnBridge)
                {
                    _bCorutine_Action = false;
                    //コルーチンが動作していたら停止。動作させる。OnBridgeコルーチンを動作させる
                    if (_bColutine_OnBox)
                    {
                        StopCoroutine(routine);
                        routine = null;
                        _bColutine_OnBox = false;
                    }
                    routine = OnBridgeCam();
                    StartCoroutine(routine);
                }
                else
                if (_bCorutine_Action) {
                    transform.position = PSc.transform.parent.position - Camera_Distance;
                    transform.LookAt(transform.root.position);
                }
            }
        }
    }

    IEnumerator OnBlockCam()
    {
        _bColutine_OnBox = true;
        Vector3 rootPos = transform.root.position;
        Vector3 PlayerPos = PSc.transform.parent.position;
        rootPos.z = PlayerPos.z = 0;
        Vector3 pos = rootPos - PlayerPos;
        Vector3 StartPos = transform.position;
        Vector3 EndPos = PlayerPos - pos - Camera_Distance;
        float timer = 0;
        while (timer>=1)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(StartPos, EndPos, timer);
            transform.LookAt(transform.root.position);
        }
        _bCorutine_Action = true;
    }

    IEnumerator OnBridgeCam()
    {
        _bColutine_OnBridge = true;
        Vector3 StartPos = transform.position;
        Vector3 EndPos = PSc.transform.parent.position - Camera_Distance;
        float timer = 0;
        while (timer >= 1)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            transform.position = Vector3.Lerp(StartPos, EndPos, timer);
            transform.LookAt(transform.root.position);
        }
        _bCorutine_Action = true;
    }
    
    //===========================================================
    // ステージ見渡し処理　：ゴールからスタートまでをY軸回転しながら見渡す
    // Starter(カメラゲームスタート位置,カメラゲームプレイ位置)
    //===========================================================
    //this.Start()->
    IEnumerator Starter(Vector3 spos,Vector3 epos)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(sphere.GetComponent<SphereCollider>());
        Destroy(sphere.GetComponent<MeshRenderer>());
        Destroy(sphere.GetComponent<MeshFilter>());
        sphere.transform.position = epos;
        Camera.main.transform.position = epos - StartCamera_Distance;
        transform.SetParent(sphere.transform);
        transform.LookAt(sphere.transform);
        Vector3 vec = epos - spos;
        float length = vec.magnitude;// = 5
        
        float timer = 0;
        //スタートからゴールまでの距離に応じてカメラ動作を遅く
        var cam_moveSpeed = StartCamSpeed / length;
        //一度に回転する角度
        var variation = 360 / (1 / cam_moveSpeed);
        while (true)
        {
            sphere.transform.position = Vector3.Slerp(spos, epos, timer * cam_moveSpeed);
            //カメラの起点となるSphereを回転させる。(timerが１になったときy1回転終わってる状態で)
            sphere.transform.Rotate(0, variation * Time.deltaTime, 0);
            timer += Time.deltaTime;
            if (timer * cam_moveSpeed > 1)
            {
                sphere.transform.position = epos;
                sphere.transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        timer = 0;
        var spherePos = sphere.transform.position;
        while (true)
        {
            transform.position = Vector3.Slerp(spherePos - StartCamera_Distance, spherePos - Camera_Distance, timer);
            timer += Time.deltaTime * cam_moveSpeed * 2;
            if(timer > 1)
            {
                transform.position = sphere.transform.position - Camera_Distance;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        _bMoveOK = true;
        PSc.gameObject.layer = LayerMask.NameToLayer("2D");
        PSc.Moving = true;
    }

    //===========================================================
    // 値受け渡し
    //===========================================================
    /// <summary>
    /// プレイヤーが箱の端にいるかどうか
    /// </summary>
    public bool Side
    {
        get { return bSide_edge; }
        set { bSide_edge = value; }
    }
    /// <summary>
    /// プレイヤーが渡っているかどうか
    /// </summary>
    public bool Bridge
    {
        get { return bBridge; }
        set { bBridge = value; }
    }
    /// <summary>
    /// プレイヤーが次の箱に移ったとき
    /// </summary>
    public void SetNextBox(SideColorBoxScript nextboxSc)
    {
        NowBox = nextboxSc.gameObject;
        var root = transform.root;
        transform.SetParent(null);
        root.position = NowBox.transform.position;
        transform.SetParent(root);
    }
}
