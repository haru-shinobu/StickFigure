using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeLineScript : MonoBehaviour
{

    [SerializeField, Header("対応レイヤー")]
    LayerMask pairlayer;

    void Awake()
    {
        gameObject.layer = pairlayer;
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
        //float LineOffset = GameObject.FindWithTag("BoxManager").GetComponent<GameData>().RedLine;
        //Vector3 vec = transform.parent.GetComponent<MeshRenderer>().bounds.extents;
        //transform.localScale = new Vector3((ren.x - 1) * 50, 50 / 2, 1);
    }
    
}
