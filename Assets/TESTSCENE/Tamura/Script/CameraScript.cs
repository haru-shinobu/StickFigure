using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    StageManager SManager;
    GameObject wall;
    Vector3 Wall_Cam_distance;
    private float SwingWidth = 15f;
    float Rotangle;
    float BaseAngle;
    float timer = 0;
    float Inputway = 0;
    bool CamAction = true;
    float ChangeWallSpeed;
    [SerializeField, Header("カメラ-壁間距離"), Range(0.5f, 1)]
    float Cam_Range = 1;

    void Awake()
    {
        SManager = GameObject.FindWithTag("Manager").GetComponent<StageManager>();
        wall = SManager.GetStartWall();

        //壁からカメラの距離
        Wall_Cam_distance = Camera.main.transform.localPosition - wall.transform.localPosition;
        
        var range = Wall_Cam_distance;
        Wall_Cam_distance *= Cam_Range;
        Camera.main.transform.localPosition -= range - Wall_Cam_distance;
        ReTarget(wall);
    }
    //==================================================================
    // カメラ先の壁登録と
    //==================================================================
    void ReTarget(GameObject obj)
    {
        wall = obj;
        //壁とカメラの距離
        Wall_Cam_distance = Camera.main.transform.localPosition - wall.transform.localPosition;
        //StageManagerの壁登録
        SManager.WallStageChange();
     
    }
    //==================================================================
    // カメラの移動
    //==================================================================
    void Update()
    {
        //ただの確認用（消去予定）
        Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * 10, Color.red);

        if (CamAction)
        {
            //一時的にカメラ左右を覗く
            var Camroll = Input.GetAxis("CamRoll");
            Camroll = Mathf.RoundToInt(Camroll);
            if (Inputway != Camroll)
                timer = 0;
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
            transform.eulerAngles = new Vector3(15, angle, 0);
            Inputway = Camroll;
        }
    }
    //==================================================================
    //カメラの壁移り
    //------------------------------------------------------------------
    public void UpdateTargetWall(GameObject obj)
    {
        StartCoroutine(CamTargetChange(obj));
    }
    IEnumerator CamTargetChange(GameObject obj)
    {
        float camtimer = 0;
        //カメラ現在位置
        var pos = Camera.main.transform.localPosition;
        //カメラと対象の角度
        var angle = obj.transform.localRotation.eulerAngles.y - BaseAngle;
        var dis = Quaternion.Euler(0, angle, 0) * Wall_Cam_distance;
        //カメラの移動先位置
        var ReCamPos = obj.transform.localPosition + dis;

        //カメラ現在視点先
        //カメラから壁の距離
        var Cam_Wall_dis = wall.transform.localPosition - Camera.main.transform.localPosition;
        //ベースとカメラ角差
        var Nowangle = Camera.main.transform.localRotation.eulerAngles.y - BaseAngle;
        var dist = Quaternion.Euler(0, Nowangle, 0) * Cam_Wall_dis;
        var lookpos = Camera.main.transform.localPosition + dist;

        Debug.DrawLine(Camera.main.transform.localPosition, lookpos, Color.blue, 10);

        while (camtimer < 1)
        {
            yield return new WaitForEndOfFrame();

            camtimer += Time.deltaTime * ChangeWallSpeed;

            //カメラのポジション移動
            Camera.main.transform.localPosition = Vector3.Lerp(pos, ReCamPos, camtimer);

            //元の対象と先の対象の間を補完
            var target = Vector3.Lerp(
                lookpos, obj.transform.position, camtimer);
            Debug.DrawLine(target, Camera.main.transform.localPosition, Color.green, 4);
            //補完先を向く回転
            Camera.main.transform.LookAt(target);
        }
        Camera.main.transform.localPosition = ReCamPos;
        Camera.main.transform.LookAt(obj.transform);
        BaseAngle = Camera.main.transform.localRotation.eulerAngles.y;

        ReTarget(obj);
        //コントロール許可願い
        SManager.MoveReStart(0);// 0はカメラ用
    }
    //------------------------------------------------------------------
    //==================================================================

    //==================================================================
    //以下設定受け渡し
    //==================================================================
    //カメラスイング幅を取得
    // StageManager Awake() ->
    public void SetCamSwing(float val)
    {
        SwingWidth = val;
    }
    public void SetNowWall(GameObject obj)
    {
        wall = obj;
    }
    //現在の壁
    public GameObject GetNowWall()
    {
        return wall;
    }
    //コントロール
    public void SetControllJudge(bool flag)
    {
        CamAction = flag;
    }
    public void SetChangeWallSpeed(float val)
    {
        ChangeWallSpeed = val;
    }
}
