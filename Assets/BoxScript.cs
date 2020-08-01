using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxScript : MonoBehaviour
{
    Transform[] faces;

    //壁移動時の切り替え速度
    ///(他スクリプトから値を渡す事。カメラや物体の変化速度と合わせるため)
    [Header("壁切替速度"),Range(0.5f,2)]
    private float ChangeSpeed = 1;

    GameObject frontwall;
    int rollway;
    //============================================
    // スケール次第で色変更
    //============================================
    void Start()
    {
        faces = new Transform[6];
        faces[0] = transform.GetChild(0).transform;
        faces[1] = transform.GetChild(1).transform;
        faces[2] = transform.GetChild(2).transform;
        faces[3] = transform.GetChild(3).transform;
        faces[4] = transform.GetChild(4).transform;
        faces[5] = transform.GetChild(5).transform;

        if (transform.localScale.x != 1)//仮置き１以外で色変化
        {
            faces[2].GetComponent<SpriteRenderer>().color = Color.green;
            faces[3].GetComponent<SpriteRenderer>().color = Color.green;
        }
        if (transform.localScale.y != 1)
        {
            faces[4].GetComponent<SpriteRenderer>().color = Color.green;
            faces[5].GetComponent<SpriteRenderer>().color = Color.green;
        }
        if (transform.localScale.z != 1)
        {
            faces[0].GetComponent<SpriteRenderer>().color = Color.green;
            faces[1].GetComponent<SpriteRenderer>().color = Color.green;
        }
        BoxSurfaceScript[] boxsp = {
        faces[0].GetComponent<BoxSurfaceScript>(),
        faces[1].GetComponent<BoxSurfaceScript>(),
        faces[2].GetComponent<BoxSurfaceScript>(),
        faces[3].GetComponent<BoxSurfaceScript>(),
        faces[4].GetComponent<BoxSurfaceScript>(),
        faces[5].GetComponent<BoxSurfaceScript>()
        };
    }

    //======================================================
    /// <summary>
    /// ブロックの回転(
    /// int 1~4(各上下左右),
    /// GameObject 現在のプレイヤーのいる壁,
    /// GameObject 次にプレイヤーのいく壁)
    /// </summary>
    //======================================================
    //BoxSurfaceScript ChangeWalls(Transform Ptrs)->
    public void RollBlocks(int rollways, GameObject nowwalls, GameObject nextwalls)
    {
        frontwall = nextwalls;
        rollway = rollways;
        Vector3 a = transform.position; 
        Vector3 b = nowwalls.transform.position; 
        Vector3 c = nextwalls.transform.position;
        Vector3 _vec = Vector3.Cross(b - a, c - a);
        _vec /= _vec.magnitude;
        StartCoroutine("BlockRoller",_vec);
    }
    IEnumerator BlockRoller(Vector3 way_vec)
    {
        var player = GameObject.FindWithTag("Player");
        float minAngle = 0.0f;
        float maxAngle = 90.0f;
        float timer = 0;
        //ブロックの親の現在角を取得
        var nowrot = transform.root.localEulerAngles;
        way_vec = -way_vec;
        //指定スピードで回転
        while (true)
        {
            timer += Time.deltaTime * ChangeSpeed;
            float angle = Mathf.LerpAngle(minAngle, maxAngle, timer);
            transform.root.localEulerAngles = nowrot + angle * way_vec;
            yield return new WaitForEndOfFrame();
            if (timer >= 1)
            {
                transform.root.localEulerAngles = nowrot + maxAngle * way_vec;
                break;
            }
        }
        //僅かにズレたときのため修正
        var euAngle = transform.root.localEulerAngles;

        if (euAngle.x != 0)
            if (euAngle.x % 90 != 0)
                euAngle.x -= euAngle.x % 90;
        if (euAngle.z != 0)
            if (euAngle.y % 90 != 0)
                euAngle.y -= euAngle.y % 90;
        if (euAngle.z != 0)
            if (euAngle.z % 90 != 0)
                euAngle.z -= euAngle.z % 90;
        
        transform.root.localEulerAngles = euAngle;

        //ブロックの親からブロックを解除
        var rootTrs = transform.root;
        transform.SetParent(null);
        //解除した元ブロックの親を初期位置へ戻す
        rootTrs.eulerAngles = nowrot;
        //再びブロックの親に指定
        transform.SetParent(rootTrs);

        
        //プレイヤーとの親子関係解除
        var wa = player.transform.parent.transform;
        player.transform.SetParent(null);
        //プレイヤー回転ズレ防止
        player.transform.up = Vector3.up;
        player.transform.forward = Vector3.forward;
        //プレイヤーと壁のｚ座標一致化
        var pos = player.transform.position;
        pos.z = wa.position.z;
        player.transform.position = pos;


        //箱範囲内に収めさせる
        var PSc = player.GetComponent<Box_PlayerController>();
        PSc.WallInAria();
        yield return new WaitForEndOfFrame();
        //行動許可・移動範囲計算
        PSc.Moving = true;
        frontwall.GetComponent<BoxSurfaceScript>().came_to_front();
    }

    /// <summary>
    /// 移動先の壁を取得(
    /// GameObject現在いる壁,
    /// int 1~4(各上下左右))
    /// </summary>
    //BoxSurfaceScript ChangeWalls(Transform Ptrs)->
    public GameObject WallLocation(GameObject wall, int ways)
    {
        //現在プレイヤーのいる面
        GameObject returnObj = wall;
        //-----------------------------------------------------
        //現在プレイヤーのいる反対側の面を取得
        int num = 5;
        while (wall != faces[num].gameObject)
        {
            num--;
            if (num < -1)
                break;
        }
        int val = 0;
        if (num % 2 == 0)
            val = num + 1;
        else
            val = num - 1;
        //-----------------------------------------------------

        num = 5;
        
        while (returnObj == wall)
        {
            for(num = 5; num >= 0; num--)
            {
                //同じでなく、反対側の壁でない
                if (faces[num].gameObject == wall || faces[num] == faces[val])
                    continue;
                for (int subnum = 5; subnum >= 0; subnum--)
                {
                    //同じでなく、反対側の壁でない
                    if (faces[subnum].gameObject == wall || faces[subnum] == faces[val])
                        continue;
                    //way上下左右
                    if (ways == 1)
                    {
                        if (returnObj.transform.position.y < faces[subnum].position.y)//最高部
                            returnObj = faces[subnum].gameObject;
                    }
                    else
                    if (ways == 2)
                    {
                        if (returnObj.transform.position.y > faces[subnum].position.y)//最低部
                            returnObj = faces[subnum].gameObject;
                    }
                    else
                    if (ways == 3)
                    {
                        if (returnObj.transform.position.x > faces[subnum].position.x)//最左部
                            returnObj = faces[subnum].gameObject;
                    }
                    else
                    if (ways == 4)
                    {
                        if (returnObj.transform.position.x < faces[subnum].position.x)//最右部
                            returnObj = faces[subnum].gameObject;
                    }

                }
            }
        }
        
        //移動方向:現在壁=>次の壁Debug.Log("roll:" + ways + " " + wall.name + "=>" + returnObj);
        
        return returnObj;
    }
    //=======================================================================
    //壁移動時の切り替え速度
    //=======================================================================
    void SetChangeSpeed(float speed)
    {
        ChangeSpeed = speed;
    }
}
