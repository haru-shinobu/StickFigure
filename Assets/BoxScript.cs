using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxScript : MonoBehaviour
{
    Transform[] faces;
    //============================================
    // スケール次第で色変更
    //============================================
    void Start()
    {
        faces = new Transform[6];
        faces[0] = transform.GetChild(0).transform;
        faces[1] = transform.GetChild(1).transform;
        faces[2] = transform.GetChild(2).transform;
        faces[3] = transform.GetChild(3).transform;
        faces[4] = transform.GetChild(4).transform;
        faces[5] = transform.GetChild(5).transform;

        if (transform.localScale.x != 1)//仮置き１以外で色変化
        {
            faces[2].GetComponent<SpriteRenderer>().color = Color.green;
            faces[3].GetComponent<SpriteRenderer>().color = Color.green;
        }
        if (transform.localScale.y != 1)
        {
            faces[4].GetComponent<SpriteRenderer>().color = Color.green;
            faces[5].GetComponent<SpriteRenderer>().color = Color.green;
        }
        if (transform.localScale.z != 1)
        {
            faces[0].GetComponent<SpriteRenderer>().color = Color.green;
            faces[1].GetComponent<SpriteRenderer>().color = Color.green;
        }
        BoxSurfaceScript[] boxsp = {
        faces[0].GetComponent<BoxSurfaceScript>(),
        faces[1].GetComponent<BoxSurfaceScript>(),
        faces[2].GetComponent<BoxSurfaceScript>(),
        faces[3].GetComponent<BoxSurfaceScript>(),
        faces[4].GetComponent<BoxSurfaceScript>(),
        faces[5].GetComponent<BoxSurfaceScript>()
        };
        boxsp[0].SetFourWall(
            faces[4].gameObject,
            faces[5].gameObject,
            faces[2].gameObject,
            faces[3].gameObject);
        boxsp[1].SetFourWall(
            faces[4].gameObject,
            faces[5].gameObject,
            faces[3].gameObject,
            faces[2].gameObject);
        boxsp[2].SetFourWall(
            faces[4].gameObject,
            faces[5].gameObject,
            faces[1].gameObject,
            faces[0].gameObject);
        boxsp[3].SetFourWall(
            faces[4].gameObject,
            faces[5].gameObject,
            faces[0].gameObject,
            faces[1].gameObject);
        boxsp[4].SetFourWall(
            faces[0].gameObject,
            faces[1].gameObject,
            faces[3].gameObject,
            faces[2].gameObject);
        boxsp[5].SetFourWall(
            faces[1].gameObject,
            faces[0].gameObject,
            faces[3].gameObject,
            faces[2].gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
