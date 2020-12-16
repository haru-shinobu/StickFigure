using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class faceWay_matScript : MonoBehaviour
{
    //向き変更時のキャラ画像用
    [SerializeField]
    Material mat_right, mat_left;
    MeshRenderer face;
    
    
    void Start()
    {
        face = transform.GetChild(1).GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale == Vector3.one)
            face.material = mat_left;
        else 
        if(transform.localScale == new Vector3(-1, 1, 1))
            face.material = mat_right;
    }
}
