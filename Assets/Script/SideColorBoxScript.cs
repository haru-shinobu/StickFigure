using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SideColorBoxScript : MonoBehaviour
{
    [SerializeField, Header("回転速度"), Range(0.5f, 2)]
    float ChangeSpeed = 1;
    Vector3 F_LeftTop, F_RightBottom;
    public Vector3 GetFLT { get { return F_LeftTop; } }
    MeshRenderer mesh;
    Box_PlayerController PSc;
    //箱内橋オブジェクト
    GameObject[] BridgeBaseLines;
    public GameObject[] GetBridgeLine
    {
        get { return BridgeBaseLines; }
    }
    //箱内地面オブジェクト
    GameObject[] GroundLines;
    public GameObject[] BoxInGround
    {
        get { return GroundLines; }
        set { GroundLines = value; }
    }
    //箱内回転スイッチ
    [Header("回転スイッチは1箱に2つまで")]
    GameObject RollSwitch1 = null, RollSwitch2 = null;
    public GameObject CollRollSwitch1
    {
        get { return RollSwitch1; }
        set { RollSwitch1 = value; }
    }
    public GameObject CollRollSwitch2
    {
        get { return RollSwitch2; }
        set { RollSwitch2 = value; }
    }
    GameObject Switch;
    //箱内不透過壁
    GameObject[] NoPassWall;
    public GameObject[] NPWall
    {
        get { return NoPassWall; }
        set { NoPassWall = value; }
    }
    int BoxAroundWallLayer = 0;
    int BoxinWallLayer;
    RayScript ray = null;
    void Awake()
    {
        
        //箱枠レイヤー
        BoxAroundWallLayer = 1 << LayerMask.NameToLayer("sideWall");
        BoxinWallLayer = 1 << LayerMask.NameToLayer("UnPassWall");

        var player = GameObject.FindWithTag("Player");
        PSc = player.GetComponent<Box_PlayerController>();
        if (!mesh)
            mesh = transform.GetComponent<MeshRenderer>();

        ray = PSc.transform.parent.GetComponent<RayScript>();
        int num1 = 0;
        int num2 = 0;
        for (int i = 0; transform.childCount > i; i++)
        {
            // 1箱内 にある橋オブジェクトを確保
            if (transform.GetChild(i).tag == "BridgeBase")
                num1++;
            // 1箱内 にある不透過壁オブジェクトを確保
            if (transform.GetChild(i).tag == "NoPassWall")
                num2++;
        }

        if (num1 != 0)
        {
            BridgeBaseLines = new GameObject[num1];
            int count = 0;
            for (int i = 0; transform.childCount > i; i++)
                if (transform.GetChild(i).tag == "BridgeBase")
                {
                    BridgeBaseLines[count] = transform.GetChild(i).gameObject;
                    count++;
                }
        }
        if (num2 != 0)
        {
            NPWall = new GameObject[num2];
            int count = 0;
            for (int i = 0; transform.childCount > i; i++)
                if (transform.GetChild(i).tag == "NoPassWall")
                {
                    NPWall[count] = transform.GetChild(i).gameObject;
                    count++;
                }
        }
        //試し。リストから変換してみた
        //1箱内 地面オブジェクトを確保
        //子オブジェクト、タグgroundをすべて探索
        IEnumerable<Transform> childIEnu = GetComponentsInChildren<Transform>(true).Where(t => t.tag == "ground");
        List<GameObject> list = new List<GameObject>();
        foreach (var no in childIEnu)
        {
            list.Add(no.gameObject);
        }
        BoxInGround = new GameObject[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            BoxInGround[i] = list[i];
        }
        list.Clear();


    }
    //=======================================================================
    /// <summary>
    /// 箱回転指令
    /// Transform = Player.transform,
    /// int = 1~4(上下左右)
    /// </summary>
    //=======================================================================
    //Box_PlayerController Update()->
    public void ChangeBoxRoll(int type)
    {
        PSc.transform.parent.SetParent(transform);
        RollBlocks(type);
    }
    //=======================================================================
    /// <summary>
    /// ブロックの回転(
    /// int 1~4(各上下左右)
    /// </summary>
    //=======================================================================
    //this.ChangeBoxRoll(Transform PTrs,int type)->
    public void RollBlocks(int rollways)
    {
        Vector3 _vec = Vector3.zero;
        switch (rollways)
        {
            case 1: _vec = -Vector3.right; break;
            case 2: _vec = Vector3.right; break;
            case 3: _vec = -Vector3.up; break;
            case 4: _vec = Vector3.up; break;
        }
        StartCoroutine("BlockRoller", _vec);
    }
    IEnumerator BlockRoller(Vector3 way_vec)
    {
        PSc.FootStampActive = false;
        float minAngle = 0.0f;
        float maxAngle = 90.0f;
        float timer = 0;
        //ブロックの親の現在角を取得
        var nowrot = transform.root.localEulerAngles;
        //指定スピードで回転
        while (true)
        {
            timer += Time.deltaTime * ChangeSpeed;
            float angle = Mathf.LerpAngle(minAngle, maxAngle, timer);
            transform.root.localEulerAngles = nowrot + angle * way_vec;
            yield return new WaitForEndOfFrame();
            if (timer >= 1)
            {
                transform.root.localEulerAngles = nowrot + maxAngle * way_vec;
                break;
            }
        }
        //僅かにズレたときのため修正
        var euAngle = transform.root.localEulerAngles;

        if (euAngle.x != 0)
            if (euAngle.x % 90 != 0)
                euAngle.x -= euAngle.x % 90;
        if (euAngle.z != 0)
            if (euAngle.y % 90 != 0)
                euAngle.y -= euAngle.y % 90;
        if (euAngle.z != 0)
            if (euAngle.z % 90 != 0)
                euAngle.z -= euAngle.z % 90;

        transform.root.localEulerAngles = euAngle;


        //ブロックの親子を解除
        var rootTrs = transform.root;
        transform.SetParent(null);
        //解除した親を初期位置へ戻す
        rootTrs.eulerAngles = nowrot;
        //再びブロックの親に指定
        transform.SetParent(rootTrs);


        foreach (GameObject obj in GetBridgeLine)
        {
            Vector3 exvec = obj.GetComponent<SpriteRenderer>().bounds.extents;
            if (obj.transform.position.z <= transform.position.z - exvec.z)
            {
                Vector3 rot = obj.transform.localRotation.eulerAngles;
                var x = Mathf.Abs(rot.x % 90);
                var y = Mathf.Abs(rot.y % 90);
                var z = Mathf.Abs(rot.z % 90);
                rot.x += (x < 45) ? -x : x;
                rot.y += (y < 45) ? -y : y;
                rot.z += (z < 45) ? -z : z;
                obj.transform.localRotation = Quaternion.Euler(rot);
            }
        }

        //プレイヤーとの親子関係解除
        PSc.transform.parent.SetParent(null);
        //プレイヤー回転ズレ防止
        PSc.transform.parent.up = Vector3.up;
        PSc.transform.parent.forward = Vector3.forward;

        //プレイヤーのｚ座標を箱前面と統一、箱範囲内に収めさせる
        SetAria(PSc);

        //BridgeBaseLines.
        //念のため1拍おいた
        yield return new WaitForEndOfFrame();
        //行動許可
        PSc.FootStampActive = true;
        PSc.Moving = true;
    }
    //==================================================================
    // 箱前面の位置を記録・プレイヤーZ座標を箱前面と統一
    //==================================================================
    //this.SetBoxPos(Box_PlayerController bPSc)->
    //this.IEnumerator BlockRoller(Vector3 way_vec)->
    void SetAria(Box_PlayerController PSc)
    {
        if (!mesh)
        {
            mesh = transform.GetComponent<MeshRenderer>();
        }

        Vector3 _vec = new Vector3(
            -Mathf.Abs(mesh.bounds.extents.x),
            Mathf.Abs(mesh.bounds.extents.y),
            Mathf.Abs(mesh.bounds.extents.z));
        Vector3 FLT = transform.position + _vec;

        _vec = new Vector3(
            Mathf.Abs(mesh.bounds.extents.x),
            -Mathf.Abs(mesh.bounds.extents.y),
            -Mathf.Abs(mesh.bounds.extents.z));
        Vector3 BRB = transform.position + _vec;

        if (FLT.x > BRB.x)
        {
            var sub = BRB.x;
            BRB.x = FLT.x;
            FLT.x = sub;
        }
        if (FLT.y < BRB.y)
        {
            var sub = BRB.y;
            BRB.y = FLT.y;
            FLT.y = sub;
        }
        if (FLT.z < BRB.z)
            BRB.z = FLT.z;
        else
            FLT.z = BRB.z;
        //回転に関わる範囲
        var pos = PSc.transform.parent.position;
        pos.z = -mesh.bounds.extents.z;
        PSc.transform.parent.position = pos;

        PSc.Front_LeftTop = F_LeftTop = FLT;
        PSc.Front_RightBottom = F_RightBottom = BRB;
        
        //プレイヤーの移動に関わる範囲
        PSc.sidewallstate(0);

        var position = transform.position;
        position.z = -mesh.bounds.extents.z;
        if (!ray || ray == null)
            ray = PSc.transform.parent.GetComponent<RayScript>();
        if (BoxAroundWallLayer == 0)
            BoxAroundWallLayer = 1 << LayerMask.NameToLayer("sideWall");
        RaycastHit rayhitvecT = ray.Ray(position, Vector3.up, 10, BoxAroundWallLayer);
        RaycastHit rayhitvecB = ray.Ray(position, -Vector3.up, 10, BoxAroundWallLayer);
        RaycastHit rayhitvecR = ray.Ray(position, Vector3.right, 10, BoxAroundWallLayer);
        RaycastHit rayhitvecL = ray.Ray(position, -Vector3.right, 10, BoxAroundWallLayer);
        PSc.TopCollidersideState = false;
        if (rayhitvecR.collider != null) {if(rayhitvecR.transform.parent == transform){ BRB.x = rayhitvecR.transform.position.x - rayhitvecR.transform.GetComponent<CapsuleCollider>().radius; PSc.sidewallstate(1); }}
        if (rayhitvecL.collider != null) {if(rayhitvecL.transform.parent == transform){ FLT.x = rayhitvecL.transform.position.x + rayhitvecL.transform.GetComponent<CapsuleCollider>().radius; PSc.sidewallstate(-1); }}
        if (rayhitvecB.collider != null) {if(rayhitvecB.transform.parent == transform){ BRB.y = rayhitvecB.transform.position.y + rayhitvecB.transform.GetComponent<CapsuleCollider>().radius; PSc.sidewallstate(2); }}
        if (rayhitvecT.collider != null) {if(rayhitvecT.transform.parent == transform){ FLT.y = rayhitvecT.transform.position.y - rayhitvecT.transform.GetComponent<CapsuleCollider>().radius; PSc.TopCollidersideState = true; } }
        

        PSc.Move_Aria_FLT = FLT;
        PSc.Move_Aria_FRB = BRB;
        
    }
    //==================================================================
    //BoxRollerSwitchからの信号用
    //==================================================================
    // Box_PlayerController.MakeBridge()->
    public void On_RollerSwitch(bool flag)
    {
        StartCoroutine("BlockRoller", flag);
    }
    //this.OnRollerSwitch()->
    IEnumerator BlockRoller(bool flag)
    {
        PSc.FootStampActive = false;
        //プレイヤーを箱の子に。
        PSc.transform.parent.SetParent(transform);
        //プレイヤーの移動禁止
        PSc.P_rb.velocity = Vector3.zero;
        PSc.P_rb.isKinematic = true;
        Vector3 way_vec;
        if (flag)
            way_vec = -Vector3.forward;
        else
            way_vec = Vector3.forward;

        yield return new WaitForEndOfFrame();

        float minAngle = 0.0f;
        float maxAngle = 90.0f;
        float timer = 0;
        //ブロックの親の現在角を取得
        Vector3 nowrot = transform.root.localEulerAngles;
        //指定スピードで回転
        while (true)
        {
            timer += Time.deltaTime * ChangeSpeed;
            float angle = Mathf.LerpAngle(minAngle, maxAngle, timer);
            transform.root.localEulerAngles = nowrot + angle * way_vec;
            PSc.transform.parent.up = Vector3.up;
            yield return new WaitForEndOfFrame();
            if (timer >= 1)
            {
                transform.root.localEulerAngles = nowrot + maxAngle * way_vec;
                break;
            }
        }
        //僅かにズレたときのため修正
        var euAngle = transform.root.localEulerAngles;

        if (euAngle.x != 0)
            if (euAngle.x % 90 != 0)
                euAngle.x -= euAngle.x % 90;
        if (euAngle.z != 0)
            if (euAngle.y % 90 != 0)
                euAngle.y -= euAngle.y % 90;
        if (euAngle.z != 0)
            if (euAngle.z % 90 != 0)
                euAngle.z -= euAngle.z % 90;


        transform.root.localEulerAngles = euAngle;

        //ブロックの親子を解除
        var rootTrs = transform.root;
        transform.SetParent(null);
        //解除した親を初期位置へ戻す
        rootTrs.eulerAngles = nowrot;

        //再びブロックの親に指定
        transform.SetParent(rootTrs);

        //プレイヤーとの親子関係解除
        PSc.transform.parent.SetParent(null);
        //プレイヤー回転ズレ防止
        PSc.transform.parent.up = Vector3.up;
        PSc.transform.parent.forward = Vector3.forward;
        //プレイヤーのｚ座標を箱前面と統一、箱範囲内に収めさせる
        SetAria(PSc);

        //念のため1拍おいた
        yield return new WaitForEndOfFrame();
        //行動許可
        PSc.P_rb.isKinematic = false;
        PSc.FootStampActive = true;
        PSc.Moving = true;
    }
    //==================================================================
    // ターゲットが範囲内かどうか判定(テキストalpha用)
    //==================================================================
    public bool CheckArea(Vector3 Ppos)
    {
        //範囲内のとき
        if (F_LeftTop.x + 1f < Ppos.x && Ppos.x < F_RightBottom.x - 1f)
            if (F_RightBottom.y + 1f < Ppos.y && Ppos.y < F_LeftTop.y - 1f)
                return true;
        return false;
    }
    //=======================================================================
    //壁移動時の切り替え速度
    //=======================================================================
    void SetChangeSpeed(float speed)
    {
        ChangeSpeed = speed;
    }

    //=======================================================================
    // 箱移動後の設定用
    //=======================================================================
    public void SetBoxPos(Box_PlayerController bPSc)
    {
        SetAria(bPSc);
    }
    //=======================================================================
    // 箱に回転スイッチが存在したときの位置
    //=======================================================================
    public Vector3 FindBoxRollerSwitch()
    {
        float PposX = PSc.transform.parent.position.x;

        
        if (CollRollSwitch1) {
            if (CollRollSwitch1.transform.position.z <= F_LeftTop.z)
            {
                if (PSc.transform.parent.position.y < CollRollSwitch1.transform.position.y) 
                {
                    var swichwide1 = CollRollSwitch1.GetComponent<SpriteRenderer>().bounds.extents;
                    if (CollRollSwitch1.transform.position.x - swichwide1.x <= PposX && PposX <= CollRollSwitch1.transform.position.x + swichwide1.x)
                    {
                        Switch = CollRollSwitch1;

                        return Switch.transform.position;
                    }
                }
            }
        }

        if (CollRollSwitch2)
            if (CollRollSwitch2.transform.position.z <= F_LeftTop.z)
            {
                if (PSc.transform.parent.position.y < CollRollSwitch2.transform.position.y)
                {
                    var swichwide2 = CollRollSwitch2.GetComponent<SpriteRenderer>().bounds.extents;
                    if (CollRollSwitch2.transform.position.x - swichwide2.x <= PposX && PposX <= CollRollSwitch2.transform.position.x + swichwide2.x)
                    {
                        Switch = CollRollSwitch2;

                        return Switch.transform.position;
                    }
                }
            }
        return F_LeftTop;
    }

    //スイッチの回転方向
    public bool GetRollSwitchWay()
    {
        return Switch.GetComponent<BoxRollerSwitchScript>().GetRollWay;
    }
    //=======================================================================
    // 回転スイッチへ動作指令
    //=======================================================================
    public void RollSwitchAction()
    {
        Switch.GetComponent<BoxRollerSwitchScript>().OnRoll(this);
    }
    //=======================================================================
    // 不透過壁
    //=======================================================================
    public Vector3 UnPassWallPos()
    {
        //var ray = PSc.transform.parent.GetComponent<RayScript>();
        var hit = ray.Ray(PSc.transform.position, Vector3.up, (int)(F_LeftTop.y - PSc.transform.position.y)+1, BoxinWallLayer);
        if (hit.collider != null)
        {
            return hit.point;
        }
        return F_LeftTop;
    }
    public Vector3 sideWall()
    {
        //var ray = PSc.transform.parent.GetComponent<RayScript>();
        var rayhitvecT = ray.Ray(PSc.transform.position, Vector3.up, (int)(F_LeftTop.y - F_RightBottom.y), BoxAroundWallLayer);
        //Debug.DrawRay(PSc.transform.position,Vector3.up* (int)(F_LeftTop.y - F_RightBottom.y), Color.red, 100);
        if (rayhitvecT.collider != null)
        {
            return rayhitvecT.point;
        }
        return F_LeftTop;
        
    }
}
