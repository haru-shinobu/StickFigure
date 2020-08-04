using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField,Header("ゴール")]
    GameObject GoalObj;
    [SerializeField, Header("スタート")]
    GameObject StartObj;
    [SerializeField,Range(0.1f,10)]
    float StartCamSpeed = 1;
    GameObject Player;
    bool bMoveOK = false;
    bool bSide_edge = false;
    bool bBridge = false;
    [SerializeField]
    Vector3 StartCamera_Distance;
    [SerializeField]
    Vector3 Camera_Distance;
    GameObject NowBox;

    void Start()
    {
        NowBox = StartObj;
        Player = GameObject.FindWithTag("Player");
        if (GoalObj && StartObj)
        {
            var Cam_StartPos = GoalObj.transform.position;
            var Cam_EndPos = StartObj.transform.position;
            
            StartCoroutine(Starter(Cam_StartPos, Cam_EndPos));
        }
    }
    //===========================================================
    // プレイヤー移動と共に移動する処理(Y軸のみ追従)
    //===========================================================
    void Update()
    {
        //ステージ見渡しムービー終了・カメラ動作OK時
        if (bMoveOK)
        {
            //プレイヤーが壁の端か否か
            if (bSide_edge)
            {
                //プレイヤーが橋の上か否か
                if (bBridge)
                {

                }
                else
                {
                    //カメラ滑らかに移動とかしたほうがいいんだろうけど…めんどk
                    transform.position = Player.transform.position - Camera_Distance;
                }
            }
            else
            {
                //カメラ滑らかに移動とかしたほうがいいんだろうけど…めんどk
                transform.position = NowBox.transform.root.position - Camera_Distance;
                //transform.position = Vector3.Lerp(transform.position, NowBox.transform.root.position - Camera_Distance,);
            }
        }
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
        while (true)
        {
            transform.position = Vector3.Slerp(sphere.transform.position - StartCamera_Distance, sphere.transform.position - Camera_Distance, timer * cam_moveSpeed);
            timer += Time.deltaTime;
            if(timer*cam_moveSpeed > 1)
            {
                transform.position = sphere.transform.position - Camera_Distance;
                break;
            }
        }
        bMoveOK = true;
        Player.GetComponent<Box_PlayerController>().Moving = true;
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
    }
}
