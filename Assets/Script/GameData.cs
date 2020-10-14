using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    GameObject Wind;
    public GameObject WindPrefab
    {
        get { return Wind; }
        set { Wind = value; }
    }

    GameObject Player_On_NowBox;
    public GameObject P_Now_Box
    {
        get { return Player_On_NowBox; }
        set { Player_On_NowBox = value; }
    }

    float Box_Offset;
    public float RedLine
    {
        get { return Box_Offset; }
        set { Box_Offset = value; }
    }

    GameObject[] BridgeBases;
    public GameObject[] Bases
    {
        get { return BridgeBases; }
        set { BridgeBases = value; }
    }

    float SW_Offset;
    /// <summary>
    /// SideWall_Offset half
    /// </summary>
    public float SideWall_Offset
    {
        get { return SW_Offset; }
        set { SW_Offset = value; }
    }
}
