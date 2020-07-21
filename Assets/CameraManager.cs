using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    GameObject GoalObj;
    [SerializeField]
    GameObject StartObj;

    GameObject Player;
    bool bFlag = false;
    bool bMoveOK = false;
    Vector3 Camera_Distance;

    void Start()
    {
        if(GoalObj && StartObj)
        {
            var Cam_StartPos = GoalObj.transform.position;
            var Cam_EndPos = StartObj.transform.position;
            StartCoroutine(Starter(Cam_StartPos, Cam_EndPos));
        }
        Player = GameObject.FindWithTag("Player");
        Camera_Distance = new Vector3(0, 0, -10);//仮
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
    //===========================================================
    IEnumerator Starter(Vector3 spos,Vector3 epos)
    {
        yield return new WaitForEndOfFrame();
        Debug.Log(spos);
        Debug.Log(epos);
        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer > 1)
                break;
        }
    }
}
