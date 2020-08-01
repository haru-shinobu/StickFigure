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
    Vector3 Camera_Distance;
    BoxScript boxSc;

    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        //Player.GetComponent<Box_PlayerController>().SetMoving(false);
        Camera_Distance = new Vector3(0, 0, -10);//仮
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
                var pos = Player.transform.position;
                pos.x = 0;
                pos.z = 0;
                pos += Camera_Distance;
                transform.position = pos;
                //プレイヤーが橋の上か否か
                if (bBridge)
                {

                }
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
        bMoveOK = true;
        Player.GetComponent<Box_PlayerController>().SetMoving(true);
    }

    //===========================================================
    // 値受け渡し
    //===========================================================
    /// <summary>
    /// プレイヤーが箱の端にいるかどうか
    /// </summary>
    public void SetSideEdge(bool flag)
    {
        bSide_edge = flag;
    }
    /// <summary>
    /// プレイヤーが渡っているかどうか
    /// </summary>
    public void SetOn_Bridge(bool flag)
    {
        bBridge = false;
    }
    /// <summary>
    /// プレイヤーが次の箱に移ったとき
    /// </summary>
    public void SetNextBox(BoxScript nextboxSc)
    {
        boxSc = nextboxSc;
    }
}
