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
        if (_bDimention)
        {
            LayerMask.NameToLayer("3D");
        }
        else
        {   
            LayerMask.NameToLayer("2D");
        }

        stage = GameObject.FindWithTag("Manager").GetComponent<StageManager>();
        //テレサドアの場合
        if (_bFakeDoor)
        {
            //テレサドアの移動先が3Dの場合 移動先をランダム取得
            //※現在移動先次元判別は行っていない
            if (_bTargetDimention)
            {
                GameObject[] objs = GameObject.FindGameObjectsWithTag("Door");
                do
                {
                    var num = Random.Range(0, objs.Length);
                    DoorAdress = objs[num].GetComponent<DoorScript>().GetDoorAdress();
                } while (DoorAdress == null && objs.Length > 1);
            }
            //テレサドアの移動先が2Dの場合 移動先をランダム取得
            else
            {
                GameObject[] objs = GameObject.FindGameObjectsWithTag("Door");
                do
                {
                    var num = Random.Range(0, objs.Length);
                    DoorAdress = objs[num].GetComponent<DoorScript>().GetDoorAdress();
                } while (DoorAdress == null && objs.Length > 1);
            }

            if (DoorAdress.layer == LayerMask.NameToLayer("3D"))
                _bTargetDimention = true;
            else
                _bTargetDimention = false;
        }
        //テレサドアでない場合
        else
        {
            _bTargetDimention = !_bDimention;
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
        if (_bDimention == _bTargetDimention)
            bDimention = !bDimention;

        stage.SetNowWallAcsess(bDimention,DoorAdress);
        //テレサ再配置
        if (_bFakeDoor)
        {
            RePositionTERESA();
        }
    }
    //==================================================================
    // 再配置・再設定
    // 
    //==================================================================
    void RePositionTERESA()
    {
        var walls = GameObject.FindGameObjectsWithTag("Wall");
        var oldwall = FrontWall;
        do
        {
            var num = Random.Range(0, walls.Length);
            FrontWall = walls[num];

        } while (FrontWall == oldwall);

        
        var pos = FrontWall.transform.position;
        pos.y = transform.position.y;
        transform.position = pos;
        transform.forward = FrontWall.transform.forward;
        Start();
        if (_bDimention)
            transform.position -= FrontWall.transform.forward*FrontWall.GetComponent<RelayWallScript>().GetDepth();

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
