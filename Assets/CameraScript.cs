using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    PlayerController player;
    GameObject wall;
    Vector3 Wall_Cam_distance;
    [SerializeField, Header("カメラ振り角度")]
    private float SwingWidth = 15f;
    float Rotangle;
    float BaseAngle;
    float timer = 0;
    float Inputway = 0;
    bool CamAction = true;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        ReTarget();
    }
    void ReTarget()
    {
        RaycastHit hit;

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out hit, LayerMask.GetMask("room")))
        {
            wall = hit.collider.gameObject;
            player.WallScript = wall.GetComponent<RelayWallScript>();
            //壁とカメラの距離
            Wall_Cam_distance = Camera.main.transform.localPosition - wall.transform.localPosition;
        }
    }
    void Update()
    {
        Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * 8, Color.red);

        if (CamAction)
        {
            //一時的にカメラ左右を覗く
            var Camroll = Input.GetAxis("Vertical");
            Camroll = Mathf.RoundToInt(Camroll);
            if (Inputway != Camroll)
                timer = 0;
            Debug.Log(BaseAngle);
            if (Camroll < 0)
            {
                Rotangle = BaseAngle + SwingWidth;
            }
            else
            if (Camroll > 0)
            {
                Rotangle = BaseAngle - SwingWidth;
            }
            else
            {
                Rotangle = BaseAngle;
            }

            timer += Time.deltaTime;
            float angle = Mathf.LerpAngle(Camera.main.transform.localRotation.eulerAngles.y, Rotangle, timer);
            Debug.Log(angle);
            transform.eulerAngles = new Vector3(15, angle, 0);
            Inputway = Camroll;
        }
    }

    public void UpdateTargetWall(GameObject obj)
    {
        CamAction = false;
        StartCoroutine(CamTargetChange(obj));       
    }
    IEnumerator CamTargetChange(GameObject obj)
    {
        
        float camtimer = 0;
        //カメラ現在位置
        var pos = Camera.main.transform.localPosition;
        //カメラと対象の距離
        var dis = Quaternion.Euler(0, obj.transform.localRotation.eulerAngles.y, 0) * Wall_Cam_distance;
        //カメラの移動先位置
        var ReCamPos = obj.transform.localPosition + dis;
        
        while (camtimer < 1)
        {
            yield return new WaitForEndOfFrame();

            camtimer += Time.deltaTime;
         
            //カメラのポジション移動
            Camera.main.transform.localPosition = Vector3.Lerp(pos, ReCamPos, camtimer);

            //元の対象と先の対象の間を補完
            var target = Vector3.Lerp(wall.transform.position, obj.transform.position, camtimer);

            //補完先を向く回転
            Camera.main.transform.LookAt(target);
        }
        Camera.main.transform.LookAt(obj.transform);
        BaseAngle = Camera.main.transform.localRotation.eulerAngles.y;
        ReTarget();
        CamAction = true;
    }
}
