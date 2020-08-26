using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField, Header("ゴール")]
    GameObject GoalObj;
    [SerializeField, Header("スタート")]
    GameObject StartObj;
    [SerializeField, Range(0.1f, 10)]
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
    bool _bColutine_OnLine = false;
    bool _bColutine_OutLine = false;
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

            StartCoroutine(Starter(Cam_StartPos, Cam_EndPos));
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
                Vector3 Rootpos = transform.root.position;
                Vector3 Pscpos = PSc.transform.position;
                Rootpos.z = Pscpos.z = 0;
                Vector3 pos = (Rootpos - Pscpos);
                transform.position = Pscpos - pos - Camera_Distance;
                transform.LookAt(transform.root.position);
                /*
                //プレイヤーが壁の端か否か
                if (PSc.CheckRedAria())//ライン上true
                {
                    _bOnLine = true;
                    //ライン上に来た時、アウトラインコルーチンが動作していない
                    if (!_bColutine_OnLine)
                    {
                        _bCorutine_Action = false;
                        //アウトラインコルーチンが動作していたら停止、オンライン動作させる
                        if (_bColutine_OutLine)
                        {
                            StopCoroutine(routine);
                            routine = null;
                            _bColutine_OutLine = false;
                        }
                        routine = Cam_RedOnLine();
                        StartCoroutine(routine);
                    }
                    //コルーチン動作終了後処理
                    else
                    if (_bCorutine_Action)
                    {
                        Vector3 Rootpos = transform.root.position;
                        Vector3 Pscpos = PSc.transform.position;
                        Rootpos.z = Pscpos.z = 0;
                        Vector3 pos = (Rootpos - Pscpos);
                        transform.position = Pscpos - pos - Camera_Distance;
                        transform.LookAt(transform.root.position);
                    }
                }
                //ライン外false
                else
                {
                    _bOnLine = false;
                    if (PSc.OnRedLine)//箱回転中はライン外かつ
                    {
                        Vector3 Rootpos = transform.root.position;
                        Vector3 Pscpos = PSc.transform.position;
                        Rootpos.z = Pscpos.z = 0;
                        Vector3 pos = (Rootpos - Pscpos);
                        transform.position = PSc.transform.position - pos - Camera_Distance;
                        transform.LookAt(transform.root.position);
                    }
                    else
                    {
                        //ライン外に来た時、アウトラインコルーチンが動作していない
                        if (!_bColutine_OutLine)
                        {
                            _bCorutine_Action = false;
                            //オンラインコルーチンが動作していたら停止、アウトライン動作させる
                            if (_bColutine_OnLine)
                            {
                                StopCoroutine(routine);
                                routine = null;
                                _bColutine_OnLine = false;
                            }
                            routine = Cam_RedOutLine();
                            StartCoroutine(routine);
                        }
                        //コルーチン動作終了後処理
                        else
                        if (_bCorutine_Action)
                        {
                            transform.position = transform.transform.root.position - Camera_Distance;
                            transform.LookAt(transform.root.position);
                        }
                    }
                }
                */
            }
            else//橋の上
            {
                transform.position = PSc.transform.position - Camera_Distance;
                transform.LookAt(transform.root.position);
            }
        }
    }
    /*
    IEnumerator Cam_RedOnLine()
    {
        _bColutine_OnLine = true;
        float timer = 0;
        var Cpos = transform.position;
        Vector3 Rpos = transform.root.position;
        Rpos.z = 0;
        while (_bOnLine)
        {
            timer += Time.deltaTime * redline_cam_move_time;
            Vector3 Ppos = PSc.transform.position;
            Ppos.z = 0;
            transform.position = Vector3.Slerp(Cpos, Ppos - (Rpos - Ppos) - Camera_Distance, timer);
            transform.LookAt(transform.root.position);
            if (1 < timer)
                break;
            yield return new WaitForEndOfFrame();
        }
        _bCorutine_Action = true;
    }
    IEnumerator Cam_RedOutLine()
    {
        _bColutine_OutLine = true;
        float timer = 0;
        var Cpos = transform.position;
        while (!_bOnLine)
        {
            timer += Time.deltaTime * redline_cam_move_time;
            transform.position = Vector3.Slerp(Cpos, transform.root.transform.position - Camera_Distance, timer);
            transform.LookAt(transform.root.position);
            if (1 < timer)
                break;
            yield return new WaitForEndOfFrame();
        }
        _bCorutine_Action = true;
    }
    */
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
