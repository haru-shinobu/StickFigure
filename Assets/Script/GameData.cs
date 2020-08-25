using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
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
}
