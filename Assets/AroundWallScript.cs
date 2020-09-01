using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AroundWallScript : MonoBehaviour
{
    void Start()
    {
        var ren = transform.parent.GetComponent<MeshRenderer>().bounds.extents;
        var ppos = transform.parent.position;
        var pos = transform.position;
        Vector3 vec = Vector3.zero;

        if (pos.x == ppos.x + ren.x) vec.x += 0.001f;
        
        if (pos.x == ppos.x - ren.x) vec.x -= 0.001f;
        
        if (pos.y == ppos.x+ ren.y) vec.y += 0.001f;
        
        if (pos.y == ppos.x - ren.y) vec.y -= 0.001f;
        
        if (pos.z == ppos.x+ren.z) vec.z += 0.001f;
        
        if (pos.z == ppos.x - ren.z) vec.z -= 0.001f;
        //transform.position = pos + vec;

        transform.GetChild(0).transform.position += vec;
        transform.GetChild(1).transform.position += vec;
    }
}
