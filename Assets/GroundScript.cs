using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundScript : MonoBehaviour
{
    void Start()
    {
        var ren = transform.parent.GetComponent<MeshRenderer>().bounds.extents;
        var ppos = transform.parent.position;
        var pos = transform.position;
        //右に等しい
        if (pos.x == ppos.x + ren.x) pos.x += 0.001f;
        //左に等しい
        if (pos.x == ppos.x - ren.x) pos.x -= 0.001f;
        //上に等しい
        if (pos.y == ppos.y + ren.y) pos.y += 0.001f;
        //下に等しい
        if (pos.y == ppos.y - ren.y) pos.y -= 0.001f;
        //奥に等しい
        if (pos.z == ppos.z + ren.z) pos.z += 0.001f;
        //前に等しい
        if (pos.z == ppos.z - ren.z) pos.z -= 0.001f;
        
        transform.position = pos;
    }
    public void SlipDown()
    {
        GetComponent<CapsuleCollider>().enabled = false;
        Invoke("Slipout", 1);
    }
    void Slipout()
    {
        GetComponent<CapsuleCollider>().enabled = true;
    }
}
