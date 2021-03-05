using System.Collections;
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

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "PlayerBase")
    //    {
    //        var ex = transform.GetComponent<SpriteRenderer>().bounds.extents;
    //        if (ex.x < ex.y)
    //        {
    //            ex.y = ex.z = 0;
    //            var pos = transform.position - other.transform.position;
    //            pos.y = pos.z = 0;
    //            other.transform.GetChild(0).SendMessage("playerMovePoint",
    //                (other.transform.position.x < transform.position.x) ?
    //            -ex : ex
    //            );
    //        }
    //    }
    //}

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("IN coll");
        if (collision.transform.tag == "PlayerBase")
        {
            //Debug.Log("IN if");
            var ex = transform.GetComponent<SpriteRenderer>().bounds.extents;
            if (ex.x < ex.y)
            {
                ex.y = ex.z = 0;
                var pos = transform.position - collision.transform.position;
                pos.y = pos.z = 0;
                collision.transform.GetChild(0).SendMessage("playerMovePoint",
                    (collision.transform.position.x < transform.position.x) ?
                -ex : ex
                );
            }
        }
    }
}
