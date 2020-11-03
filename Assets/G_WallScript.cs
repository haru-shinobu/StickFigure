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
    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "PlayerBase")
        {
            var ex = transform.GetComponent<SpriteRenderer>().bounds.extents;
            if (ex.x < ex.y)
            {
                var pos = transform.position - collision.transform.position;
                pos.y = 0;
                pos.z = 0;
                collision.transform.GetChild(0).SendMessage("playerMovePoint",
                    (collision.transform.position.x < transform.position.x) ?
                -pos : pos
                );
            }
        }
    }
}
