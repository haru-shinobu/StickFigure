﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeLineScript : MonoBehaviour
{

    [SerializeField, Header("対応カラー")]
    Color color;

    bool b_enabled = false;
    SideColorBoxScript side;
    public Color GetColor { get { return GetComponent<SpriteRenderer>().color; } }

    void Awake()
    {
        color = GetComponent<SpriteRenderer>().color;
        //やや箱表面から浮かせないとめり込む。
        var ren = transform.parent.GetComponent<MeshRenderer>().bounds.extents;
        var ppos = transform.parent.position;
        var pos = transform.position;
        if (pos.x == ppos.x + ren.x)  pos.x += 0.001f; 
        if (pos.x == ppos.x - ren.x)  pos.x -= 0.001f; 
        if (pos.y == ppos.y + ren.y)  pos.y += 0.001f; 
        if (pos.y == ppos.y - ren.y)  pos.y -= 0.001f; 
        if (pos.z == ppos.z + ren.z)  pos.z += 0.001f; 
        if (pos.z == ppos.z - ren.z)  pos.z -= 0.001f; 
        transform.position = pos;
        if (transform.up == Vector3.up || transform.up == -Vector3.up)
        { if (b_enabled) b_enabled = false; }
        else
            if (!b_enabled)  b_enabled = true;

        side = transform.parent.GetComponent<SideColorBoxScript>();
    }
    void Update()
    {
        if (!side.RollActiveFlag)
        {
            if (transform.position.z <= side.GetFLT.z)
            {
                //橋基礎が横長
                if (transform.up == Vector3.up || transform.up == -Vector3.up)
                {
                    if (!b_enabled)
                    {
                        transform.up = Vector3.up;
                        GetComponent<CapsuleCollider>().enabled = true;
                        b_enabled = true;
                    }
                }
                else
                    if (b_enabled)
                {
                    GetComponent<CapsuleCollider>().enabled = false;
                    b_enabled = false;
                }
            }
        }
    }

    public void SlipdroundLine()
    {
        GetComponent<CapsuleCollider>().enabled = false;
        Invoke("ReGround", 1f);
    }
    void ReGround()
    {
        GetComponent<CapsuleCollider>().enabled = true;
    }
    public void DownBridge()
    {
        GetComponent<CapsuleCollider>().enabled = false;
        StartCoroutine("WaitToFall");
        
    }
    IEnumerator WaitToFall()
    {
        yield return new WaitForSeconds(1);
        Box_PlayerController player = GameObject.FindWithTag("Player").GetComponent<Box_PlayerController>();
        while (!player.Moving)
            yield return new WaitForEndOfFrame();

        if (transform.position.y < player.transform.position.y)
            GetComponent<CapsuleCollider>().enabled = true;
        else
        {
            Invoke("ReGround", 1);
        }

    }
    
    public void enable(bool value)
    {
        b_enabled = value;
    }
    public void Antienable()
    {
        b_enabled = !b_enabled;
    }
    public void ResetFaceWay()
    {
        transform.forward = Vector3.forward;
    }
}
