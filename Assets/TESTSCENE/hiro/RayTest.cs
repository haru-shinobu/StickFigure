using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RayTest : MonoBehaviour
{
    public BoxSurfaceScript BChecker;
    public GameObject Text;
    public const float NOTHING = -1;
    public float maxDistance = 30;
    public float distance;

    //Vector3 LeftTop;
    //Vector3 RightBottom;

    void Start()
    {
            // BChecker=gameObject.GetComponent<BoxSurfaceScript>();
    }
    void Update()
    {
        Vector3 fwd = transform.TransformDirection(-1,-1,-10f);
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(transform.position,fwd,out hit,maxDistance))
        {
            distance = hit.distance;
        }
        else
        {
            distance = NOTHING;
        }
       Debug.DrawRay(transform.position,fwd, Color.red, 5);
        //テキスト消える
        if (distance<=3)
        {
            Text.SetActive(false);
        }
        //テキスト出てくる
        if (distance>=3)
        {
            Text.SetActive(true);
            if (Input.GetKey(KeyCode.Space))
            {
//舌の出す
            }
        }
    }
}
