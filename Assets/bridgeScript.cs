using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bridgeScript : MonoBehaviour
{
    GameObject BridgeBasePrev;
    public GameObject SetBasePrev
    {
        set { BridgeBasePrev = value; }
    }
    GameObject BridgeBaseNext;
    public GameObject SetBaseNext
    {
        set { BridgeBaseNext = value; }
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
}
