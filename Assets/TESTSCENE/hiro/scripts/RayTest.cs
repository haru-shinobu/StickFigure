using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RayTest : MonoBehaviour
{
    public Box_PlayerController BChecker;
    public GameData G_Data;
    public GameObject Text;
    public GameObject tape;
   // public GameObject Player;
    public const float NOTHING = -1;
    public float maxDistance = 30;
    public float distance;
    Vector3 MoveAriaLeftTop, MoveAriaRightBottom;


    void Start()
    {
        Text.SetActive(false);
        tape.SetActive(false);
    }
    void Update()
    {
        BChecker.CheckRedAria();
        if (BChecker.CheckRedAria() == true)
        {
            Vector3 fwd = transform.TransformDirection(0, 0, -10);
            //Rayを飛ばすtransform.TransformDirection(x,y,z);
            //上向き
            if (MoveAriaRightBottom.y + G_Data.RedLine <= transform.position.y)
            {
                fwd = transform.TransformDirection(0, 20, 5);
                Debug.Log("上");
                if (Input.GetKey(KeyCode.Space))
                {
                    tape.SetActive(true);
                    //tape.transform.position=Player.transform.position;
                    Debug.Log("いっちゃってる");
                }
            }
                //下向き
                if (MoveAriaLeftTop.y - G_Data.RedLine >= transform.position.y)
                {
                    fwd = transform.TransformDirection(0, -20, 5);
                    Debug.Log("下");
                }
                //右向き
                if (MoveAriaLeftTop.x + G_Data.RedLine <= transform.position.x)
                {
                    fwd = transform.TransformDirection(20, 0, 5);
                Debug.Log("右");
                }
                //左向き
                if (MoveAriaRightBottom.x - G_Data.RedLine >= transform.position.x)
                {
                    fwd = transform.TransformDirection(-20, 0, 5);
                    Debug.Log("左");
                }

                //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(transform.position-transform.forward, fwd, out hit, maxDistance))
                {
                    distance = hit.distance;
                }
                else
                {
                    distance = NOTHING;
                }
                Debug.DrawRay(transform.position-transform.forward, fwd, Color.red, 5);

                //テキスト消える
                if (distance <= 3)
                {
                    Text.SetActive(false);
                    //Debug.Log("Delete");
                }
                //テキスト出てくる
                if (distance >= 3)
                {
                    Text.SetActive(true);
                    Debug.Log("Push");
                    if (Input.GetKey(KeyCode.Space))
                    {
                    Debug.Log("いけるよ");
                    }
                }
            }
        }
    }
