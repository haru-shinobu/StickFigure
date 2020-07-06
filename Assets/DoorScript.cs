using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{

    [SerializeField, Header("設置した壁")]
    GameObject FrontWall;
    [SerializeField, Header("移動先のドア")]
    GameObject DoorAdress;
    [SerializeField, Header("3Dドアか否か 3D = ✓")]
    bool _bDimention;
    [SerializeField, Header("テレサドア = TRUE")]
    bool _bFakeDoor = false;
    [SerializeField,Header("テレサ移動先次元 : 2D = FALSE,3D = TRUE")]
    bool _bTargetDimention = false;
    StageManager stage;
    void Start()
    {
        tag = "Door";
        if (_bDimention)
            LayerMask.NameToLayer("3D");
        else
            LayerMask.NameToLayer("2D");

        stage = GameObject.FindWithTag("Manager").GetComponent<StageManager>();
        //テレサドアの場合
        if (_bFakeDoor)
        {
            //テレサドアの移動先が3Dの場合 移動先をランダム取得
            if (_bTargetDimention)
            {
                GameObject[] objs = GameObject.FindGameObjectsWithTag("Door");
                var num = Random.Range(0, objs.Length);
                DoorAdress = objs[num].GetComponent<DoorScript>().GetDoorAdress();
            }
            //テレサドアの移動先が2Dの場合 移動先をランダム取得
            else
            {
                GameObject[] objs = GameObject.FindGameObjectsWithTag("Door3D");
                var num = Random.Range(0, objs.Length);
                DoorAdress = objs[num].GetComponent<DoorScript>().GetDoorAdress();
            }
        }
        //テレサドアでない場合
        else
        {
            if (DoorAdress)
            {
                //相手側ドアに対応ドアセットし忘れ対策
                var backroll = DoorAdress.GetComponent<DoorScript>().GetDoorAdress();
                if (backroll == null)
                {
                    DoorAdress.GetComponent<DoorScript>().SetDoorAdress(this.gameObject);
                }
            }
        }
        transform.forward = FrontWall.transform.forward;
    }

    public void DoorAccess(bool bDimention)
    {
        stage.SetNowWallAcsess(bDimention,DoorAdress);
    }

    //==================================================================
    //以下設定受け渡し
    //==================================================================
    //対になるドアをセット
    public void SetDoorAdress(GameObject target)
    {
        DoorAdress = target;
    }
    //対のドアを返す
    public GameObject GetDoorAdress()
    {
        return DoorAdress;
    }
    //対応先の壁
    public GameObject GetWall()
    {
        return FrontWall;
    }
}
