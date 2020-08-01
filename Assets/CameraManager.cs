using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField,Header("ゴール")]
    GameObject GoalObj;
    [SerializeField, Header("スタート")]
    GameObject StartObj;
    [SerializeField,Range(1,10)]
    float StartCamSpeed = 1;
    GameObject Player;
    bool bFlag = false;
    bool bMoveOK = false;
    Vector3 Camera_Distance;


    void Start()
    {
        Camera_Distance = new Vector3(0, 0, -10);//仮
        if (GoalObj && StartObj)
        {
            var Cam_StartPos = GoalObj.transform.position;
            var Cam_EndPos = StartObj.transform.position;
            
            StartCoroutine(Starter(Cam_StartPos, Cam_EndPos));
        }
        Player = GameObject.FindWithTag("Player");

    }


    
    void Update()
    {
        //ステージ見渡しムービー終了・カメラ動作OK時
        if(bFlag && bMoveOK)
        {
            Pchase();
        }
    }
    //===========================================================
    // プレイヤー移動と共に移動する処理(Y軸のみ追従)
    //===========================================================
    void Pchase()
    {
        var pos = Player.transform.position;
        pos.x = 0;
        pos.z = 0;
        pos += Camera_Distance;
        transform.position = pos;
    }
    //===========================================================
    // ステージ見渡し処理
    // 
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
        var cam_moveSpeed = StartCamSpeed / length;// = 0.2
        var rotspeed = 1 / cam_moveSpeed;//1/0.2 = 5
        var variation = 360 / rotspeed;//360/5 = 72
        while (true)
        {
            sphere.transform.position = Vector3.Slerp(epos, spos, timer * cam_moveSpeed);
            //カメラの起点となるSphereを回転させる。(timerが１になったときy1回転終わってる状態で)
            sphere.transform.Rotate(0, variation * Time.deltaTime, 0);
            timer += Time.deltaTime;
            if (timer * cam_moveSpeed > 1)
            {
                sphere.transform.position = spos;
                sphere.transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
