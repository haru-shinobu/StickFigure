using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    GameObject Player_On_NowBox;
    public GameObject P_on_Box
    {
        get { return Player_On_NowBox; }
        set { Player_On_NowBox = value; }
    }

}
