using System.Collections;
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
        BridgeBasePrev.GetComponent<BridgeLineScript>().DownBridge();
        BridgeBaseNext.GetComponent<BridgeLineScript>().DownBridge();
        Invoke("ReActiveCollider", 0.5f);
    }
    void ReActiveCollider()
    {
        if (transform.right == Vector3.right || transform.right == -transform.right)
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
        if (flag)
        {
            //BridgeBasePrev.GetComponent<CapsuleCollider>().enabled = false;
            var boxsc = BridgeBasePrev.transform.parent.GetComponent<SideColorBoxScript>();
            var boxsc2 = BridgeBaseNext.transform.parent.GetComponent<SideColorBoxScript>();
            for (int i = 0; i < boxsc.GetBridgeLine.Length; i++)
            {
                boxsc.GetBridgeLine[i].GetComponent<CapsuleCollider>().enabled = false;
                boxsc2.GetBridgeLine[i].GetComponent<CapsuleCollider>().enabled = true;
            }
            for (int i = 0; i < boxsc.BoxInGround.Length; i++)
            {
                boxsc.BoxInGround[i].GetComponent<CapsuleCollider>().enabled = false;
                boxsc2.BoxInGround[i].GetComponent<CapsuleCollider>().enabled = true;
            }

        }
        else
        {
            //BridgeBaseNext.GetComponent<CapsuleCollider>().enabled = false;
            var boxsc = BridgeBasePrev.transform.parent.GetComponent<SideColorBoxScript>();
            var boxsc2 = BridgeBaseNext.transform.parent.GetComponent<SideColorBoxScript>();
            for (int i = 0; i < boxsc.GetBridgeLine.Length; i++)
            {
                boxsc.GetBridgeLine[i].GetComponent<CapsuleCollider>().enabled = false;
                boxsc2.GetBridgeLine[i].GetComponent<CapsuleCollider>().enabled = true;
            }
            for (int i = 0; i < boxsc.BoxInGround.Length; i++)
            {
                boxsc.BoxInGround[i].GetComponent<CapsuleCollider>().enabled = false;
                boxsc2.BoxInGround[i].GetComponent<CapsuleCollider>().enabled = true;
            }
        }
    }
}
