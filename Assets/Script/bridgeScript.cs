﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bridgeScript : MonoBehaviour
{
    //渡した側の橋ベース
    GameObject BridgeBasePrev;
    public GameObject BasePrev
    {
        set { BridgeBasePrev = value; }
        get { return BridgeBasePrev; }
    }
    //渡された側の橋ベース
    GameObject BridgeBaseNext;
    public GameObject BaseNext
    {
        set { BridgeBaseNext = value; }
        get { return BridgeBaseNext; }
    }
    void Start()
    {
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(false);

    }
    public void second_NoCollider()
    {
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(false);
        BasePrev.GetComponent<BridgeLineScript>().DownBridge();
        BaseNext.GetComponent<BridgeLineScript>().DownBridge();
        Invoke("ReActiveCollider", 0.5f);
    }
    void ReActiveCollider()
    {
        if (transform.right == Vector3.right || transform.right == -Vector3.right)
            transform.GetChild(0).gameObject.SetActive(true);
        else
            transform.GetChild(1).gameObject.SetActive(true);
    }
    /// <summary>
    /// true = 渡した側橋ベースのコライダを止める
    /// fasle = 渡された側橋ベースのコライダを止める
    /// </summary>
    /// <param name="flag"></param>
    // Box_PlayerController IEnumerator InBridge()->
    public void BridgeCross(bool flag)
    {

        var boxsc = BridgeBasePrev.transform.parent.GetComponent<SideColorBoxScript>();
        var boxsc2 = BridgeBaseNext.transform.parent.GetComponent<SideColorBoxScript>();
        if (flag)
        {
            for (int i = 0; i < boxsc.GetBridgeLine.Length; i++)
            {
                boxsc.GetBridgeLine[i].GetComponent<CapsuleCollider>().enabled = false;
                boxsc.GetBridgeLine[i].SendMessage("enable",false);
            }
            
            for (int i = 0; i < boxsc.BoxInGround.Length; i++)
                boxsc.BoxInGround[i].GetComponent<CapsuleCollider>().enabled = false;


            for (int i = 0; i < boxsc2.GetBridgeLine.Length; i++)
                if (BridgeBaseNext != boxsc2.GetBridgeLine[i])
                {
                    boxsc2.GetBridgeLine[i].GetComponent<CapsuleCollider>().enabled = true;
                    boxsc2.GetBridgeLine[i].SendMessage("enable", true);
                }
            
            for (int i = 0; i < boxsc2.BoxInGround.Length; i++)
                boxsc2.BoxInGround[i].GetComponent<CapsuleCollider>().enabled = true;
        }
        else
        {
            for (int i = 0; i < boxsc.GetBridgeLine.Length; i++)
                if (BridgeBasePrev != boxsc.GetBridgeLine[i])
                {
                    boxsc.GetBridgeLine[i].GetComponent<CapsuleCollider>().enabled = true;
                    boxsc.GetBridgeLine[i].SendMessage("enable", true);
                }
            for (int i = 0; i < boxsc.BoxInGround.Length; i++)
                boxsc.BoxInGround[i].GetComponent<CapsuleCollider>().enabled = true;


            for (int i = 0; i < boxsc2.GetBridgeLine.Length; i++)
            {
                boxsc2.GetBridgeLine[i].GetComponent<CapsuleCollider>().enabled = false;
                boxsc2.GetBridgeLine[i].SendMessage("enable", false);
            }
            for (int i = 0; i < boxsc2.BoxInGround.Length; i++)
                boxsc2.BoxInGround[i].GetComponent<CapsuleCollider>().enabled = false;
        }

        //BridgeBasePrev.GetComponent<BridgeLineScript>().SlipdroundLine();
        //BridgeBaseNext.GetComponent<BridgeLineScript>().SlipdroundLine();
    }
    public void OnBridgeCollider()
    {
        if (transform.right == Vector3.right || transform.right == -Vector3.right)
            transform.GetChild(0).gameObject.SetActive(true);
        else
            transform.GetChild(1).gameObject.SetActive(true);
    }

    public void RollDestroy(int value)
    {
        if (this.gameObject)
        {
            Rigidbody rb = transform.gameObject.AddComponent<Rigidbody>();
            if (rb)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.AddForce(-Vector3.forward * 2, ForceMode.Impulse);
                rb.AddTorque(Vector3.right * 20, ForceMode.Impulse);
                rb.AddTorque(Vector3.forward * 10, ForceMode.Impulse);
            }
            Destroy(this.gameObject, value);
        }
    }
    //消去時呼び出し
    void OnDestroy()
    {
        if (this.gameObject)
            BasePrev.GetComponent<BridgeLineScript>().Antienable();
        if (this.gameObject)
            BaseNext.GetComponent<BridgeLineScript>().Antienable();
    }
}
