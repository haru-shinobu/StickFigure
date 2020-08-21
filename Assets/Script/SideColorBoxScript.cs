using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideColorBoxScript : MonoBehaviour
{
    [SerializeField, Header("壁切替速度"), Range(0.5f, 2)]
    float ChangeSpeed = 1;
    Vector3 F_LeftTop, F_RightBottom;
    MeshRenderer mesh;
    Box_PlayerController PSc;
    GameObject[] BridgeBaseLines;
    public GameObject[] GetBridgeLine
    {
        get { return BridgeBaseLines; }
    }
    void Awake()
    {
        var player = GameObject.FindWithTag("Player");
        PSc = player.GetComponent<Box_PlayerController>();
        if (!mesh)
            mesh = transform.GetComponent<MeshRenderer>();
        BridgeBaseLines = new GameObject[transform.childCount];
        for (int i = 0; transform.childCount > i; i++)
            BridgeBaseLines[i] = transform.GetChild(i).gameObject;
    }
    //=======================================================================
    /// <summary>
    /// 箱回転指令
    /// Transform = Player.transform,
    /// int = 1~4(上下左右)
    /// </summary>
    //=======================================================================
    //Box_PlayerController Update()->
    public void ChangeBoxRoll(Transform PTrs, int type)
    {
        PTrs.SetParent(transform);
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

        //ブロックの親からブロックを解除
        var rootTrs = transform.root;
        transform.SetParent(null);
        //解除した元ブロックの親を初期位置へ戻す
        rootTrs.eulerAngles = nowrot;
        //再びブロックの親に指定
        transform.SetParent(rootTrs);


        //プレイヤーとの親子関係解除
        PSc.transform.SetParent(null);
        //プレイヤー回転ズレ防止
        PSc.transform.up = Vector3.up;
        PSc.transform.forward = Vector3.forward;

        //プレイヤーのｚ座標を箱前面と統一、箱範囲内に収めさせる

        SetAria(PSc);
        //Debug.Log("LT:"+F_LeftTop + "RB:" + F_RightBottom + "PP" + PSc.transform.position);

        yield return new WaitForEndOfFrame();
        //行動許可・移動範囲計算
        PSc.Moving = true;
    }
    //==================================================================
    // 箱前面の位置を記録・プレイヤーZ座標を箱前面と統一
    //==================================================================
    //this.SetStartPos(Box_PlayerController PSc)->
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

        var pos = PSc.transform.position;
        pos.z = -mesh.bounds.extents.z;
        PSc.transform.position = pos;

        PSc.Front_LeftTop = F_LeftTop = FLT;
        PSc.Front_RightBottom = F_RightBottom = BRB;


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
    // ゲームスタート時の設定用
    //=======================================================================
    public Transform SetStartPos(Box_PlayerController bPSc)
    {
        bPSc.transform.position = transform.position;
        SetAria(bPSc);

        return transform;
    }
    //=======================================================================
    // 箱移動後の設定用
    //=======================================================================
    public void SetBoxPos(Box_PlayerController bPSc)
    {
        SetAria(bPSc);
    }
}
