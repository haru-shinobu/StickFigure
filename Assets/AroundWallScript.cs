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
        float x = 0, y = 0, z = 0;

        if (pos.x == ppos.x + ren.x) x += 0.001f;
        
        if (pos.x == ppos.x - ren.x) x -= 0.001f;
        
        if (pos.y == ppos.x+ ren.y) y += 0.001f;
        
        if (pos.y == ppos.x - ren.y) y -= 0.001f;
        
        if (pos.z == ppos.x+ren.z) z += 0.001f;
        
        if (pos.z == ppos.x - ren.z) z -= 0.001f;
        transform.position = pos + new Vector3(x, y, z);

        transform.GetChild(0).transform.position += new Vector3(x, y, z);
        transform.GetChild(1).transform.position += new Vector3(x, y, z);
    }
}
