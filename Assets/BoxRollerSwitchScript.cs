using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxRollerSwitchScript : MonoBehaviour
{
    [SerializeField, Header("右回転TRUE")]
    bool bRollWay = true;
    public bool GetRollWay
    {
        get { return bRollWay; }
    }
    GameData G_Data;
    void Start()
    {
        G_Data = GameObject.FindWithTag("BoxManager").GetComponent<GameData>();

        var ren = transform.parent.GetComponent<MeshRenderer>().bounds.extents;
        var pos = transform.position;
        if (pos.x == ren.x) pos.x += 0.001f;
        else
        if (pos.x == -ren.x) pos.x -= 0.001f;
        else
        if (pos.y == ren.y) pos.y += 0.001f;
        else
        if (pos.y == -ren.y) pos.y -= 0.001f;
        else
        if (pos.z == ren.z) pos.z += 0.001f;
        else
        if (pos.z == -ren.z) pos.z -= 0.001f;
        transform.position = pos;
        var scs = transform.parent.GetComponent<SideColorBoxScript>();
        if (scs.CollRollSwitch1 == null)
            scs.CollRollSwitch1 = this.gameObject;
        else
        if (scs.CollRollSwitch1 == null)
            scs.CollRollSwitch2 = this.gameObject;

    }

    
    //舌か何かで動作させたとき
    public void OnRoll()
    {
        G_Data.P_Now_Box.GetComponent<SideColorBoxScript>().On_RollerSwitch(bRollWay);
        StartCoroutine("Switch_Roller");
    }
    IEnumerator Switch_Roller()
    {
        float timer = 0;
        while (true)
        {
            transform.Rotate(0, 0, 1);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            if (timer >= 1)
                break;
        }
    }
}
