using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSqript : MonoBehaviour
{
    GameObject sidewall;
    SpriteRenderer spr;
    Vector3 cam_Distance;
    PlayerController PlayerSc;
    bool OnceFlag = true;
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        cam_Distance = Camera.main.transform.localPosition;
        PlayerSc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (OnceFlag)
            if (other.gameObject.tag == "Player")//Wall tag
            {
                if ((int)PlayerSc.Getways() < 0)//right
                    ;
                OnceFlag = false;
                int way = PlayerSc.Getways();
                PlayerSc.ControllJudge(false);
                other.GetComponent<Rigidbody>().isKinematic = true;

                var sprite_halfX = (spr.sprite.bounds.extents.x);
                var diff = other.transform.localPosition - other.transform.localPosition;
                var axis = Vector3.Cross(other.transform.forward, diff);
                var angle = Vector3.Angle(other.transform.forward, diff) * (axis.y < 0 ? -1 : 1);
                if (angle > 0)
                    sprite_halfX = -sprite_halfX;
                var pos = spr.transform.TransformPoint(new Vector3(sprite_halfX, 0))
                    + new Vector3(0, other.transform.localPosition.y);
                var check = true;
                var radius = 1;


                while (check)
                {
                    //UnityEditor.EditorApplication.isPaused = true;

                    Collider[] targets = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("room"));
                    if (targets.Length == 0)
                    {
                        radius++;
                        if (radius > 10)
                        {
                            other.GetComponent<Rigidbody>().isKinematic = false;
                            PlayerSc.ControllJudge(true);
                            break;
                        }


                        continue;
                    }
                    check = false;

                    foreach (Collider obj in targets)
                    {
                        switch (way)
                        {
                            case -1:
                                if (obj.gameObject != gameObject)
                                    sidewall = gameObject;
                                else
                                    sidewall = obj.gameObject;
                                break;
                            case 0:
                                if (obj.gameObject != gameObject)
                                    sidewall = obj.gameObject;
                                else
                                    sidewall = gameObject;
                                break;
                            case 1:
                                if (obj.gameObject != gameObject)
                                    sidewall = obj.gameObject;
                                else
                                    sidewall = gameObject;
                                break;
                        }
                    }
                    StartCoroutine(Wallpoint(pos, other.gameObject, sidewall));
                }
            }
    }
    IEnumerator Wallpoint(Vector3 pos, GameObject player,GameObject wall)
    {
        var Ppos = player.transform.localPosition;
        var Prot = player.transform.localRotation;
        var Cpos = Camera.main.transform.localPosition;
        var count = 0f;
        while (count < 1)
        {
            yield return new WaitForEndOfFrame();
            count += Time.deltaTime * 2;
            player.transform.localPosition = Vector3.Lerp(Ppos, pos, count);
            player.transform.localRotation = Quaternion.Lerp(Prot, wall.transform.localRotation, count);
            Camera.main.transform.rotation = Quaternion.Lerp(Prot, wall.transform.localRotation, count);
            Camera.main.transform.localPosition = Vector3.Lerp(Cpos, wall.transform.forward, count);
        }
        player.transform.localPosition = pos;
        player.transform.localRotation = wall.transform.localRotation;
        Camera.main.transform.forward = wall.transform.forward;
        Camera.main.transform.localPosition = wall.transform.localPosition;
        //カメラの位置を記録、壁に対して正面方向でカメラを呼び出す。
        var angle =/* Camera.main.transform.localRotation.y -*/ wall.transform.localRotation.y;
        transform.RotateAround(wall.transform.localPosition, Vector3.up, angle);
        
        player.GetComponent<Rigidbody>().isKinematic = false;
        PlayerSc.ControllJudge(true);
    }

    void OnTriggerExit(Collider other)
    {
        var dist = Vector3.Distance(other.transform.localPosition, transform.localPosition);
        if (other.tag == "Player")
            if (dist
                < transform.localScale.x * 0.45f)
                if (!OnceFlag)
                    OnceFlag = true;
        Debug.Log("離断");
    }
}

