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
        boxsp[0].SetFourWall(
            faces[4].gameObject,
            faces[5].gameObject,
            faces[2].gameObject,
            faces[3].gameObject);
        boxsp[1].SetFourWall(
            faces[4].gameObject,
            faces[5].gameObject,
            faces[3].gameObject,
            faces[2].gameObject);
        boxsp[2].SetFourWall(
            faces[4].gameObject,
            faces[5].gameObject,
            faces[1].gameObject,
            faces[0].gameObject);
        boxsp[3].SetFourWall(
            faces[4].gameObject,
            faces[5].gameObject,
            faces[0].gameObject,
            faces[1].gameObject);
        boxsp[4].SetFourWall(
            faces[0].gameObject,
            faces[1].gameObject,
            faces[3].gameObject,
            faces[2].gameObject);
        boxsp[5].SetFourWall(
            faces[1].gameObject,
            faces[0].gameObject,
            faces[3].gameObject,
            faces[2].gameObject);

    }

    // Update is called once per frame
    void Update()
    {

    }

    //======================================================
    // ブロックの回転
    //======================================================
    //BoxSurfaceScript ChangeWalls(Transform Ptrs)->
    public void RollBlocks(int nextwall,GameObject nextwalls)
    {
        Vector3 _vec = new Vector3(0, 0, 0);
        switch (nextwall)
        {
            default: break;
            case 1://up
                _vec = -Vector3.right; 
                break;
            case 2://down
                _vec = Vector3.right;
                break;
            case 3://left
                _vec = -Vector3.up;
                break;
            case 4://right
                _vec = Vector3.up; 
                break;
        }
        StartCoroutine("BlockRoller",_vec);
    }
    IEnumerator BlockRoller(Vector3 way_vec)
    {
        var player = GameObject.FindWithTag("Player");
        float minAngle = 0.0f;
        float maxAngle = 90.0f;
        float timer = 0;
        while (true)
        {
            timer += Time.deltaTime * ChangeSpeed;
            float angle = Mathf.LerpAngle(minAngle, maxAngle, timer);
            transform.eulerAngles = angle * way_vec;
            
            yield return new WaitForEndOfFrame();
            if (timer >= 1) break;
        }
        
        //親子関係解除
        var wa = player.transform.parent.transform;
        player.transform.SetParent(null);
        player.transform.up = Vector3.up;
        player.transform.forward = Vector3.forward;
        var pos = player.transform.position;
        pos.z = wa.position.z;
        player.transform.position = pos;
        
        var PSc = player.GetComponent<Box_PlayerController>();

        yield return new WaitForEndOfFrame();
        //箱範囲内に収めさせる
        PSc.WallInAria();
        yield return new WaitForEndOfFrame();

        PSc.SetMoving(true);
    }
    //=======================================================================
    //壁移動時の切り替え速度
    //=======================================================================
    void SetChangeSpeed(float speed)
    {
        ChangeSpeed = speed;
    }
}
