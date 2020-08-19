using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeLineScript : MonoBehaviour
{
    void Awake()
    {
        //やや箱表面から浮かせないとめり込む。
        var ren = transform.parent.GetComponent<MeshRenderer>().bounds.extents;
        var pos = transform.position;
        if (pos.x == ren.x) pos.x += 0.001f;
        else
        if (pos.x == -ren.x) pos.x -= 0.001f;
        else
        if (pos.y == ren.y) pos.y += 0.001f;
        else
        if (pos.y == -ren.y) pos.y -= 0.001f;
        else
        if (pos.z == ren.z) pos.z += 0.001f;
        else
        if (pos.z == -ren.z) pos.z -= 0.001f;
        transform.position = pos;
        //橋ベースの大きさ調整
        
        transform.localScale = new Vector3((ren.x - 1) * 50, 50 / 2, 1);
    }
}
