﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G_WallScript : MonoBehaviour
{
    void Start()
    {
        var ren = transform.parent.GetComponent<MeshRenderer>().bounds.extents;
        var ppos = transform.parent.position;
        var pos = transform.position;
        if (pos.x == ppos.x + ren.x) pos.x += 0.001f;
        if (pos.x == ppos.x - ren.x) pos.x -= 0.001f;
        if (pos.y == ppos.y + ren.y) pos.y += 0.001f;
        if (pos.y == ppos.y - ren.y) pos.y -= 0.001f;
        if (pos.z == ppos.z + ren.z) pos.z += 0.001f;
        if (pos.z == ppos.z - ren.z) pos.z -= 0.001f;
        transform.position = pos;
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Player")
        {
            if (transform.up == Vector3.up || transform.up == -Vector3.up)
            {
                if (collision.transform.position.x < transform.position.x)
                    collision.transform.position -= Vector3.right;
                else
                    collision.transform.position += Vector3.right;
            }
        }
    }
}
