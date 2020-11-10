using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputmanager : MonoBehaviour
{
    float[] input = new float[2];
    public float[] player_move_input
    {
        get { return input; }
        set { input = value; }
    }
    bool inputSpace = false;
    public bool player_jump_input
    {
        get { return inputSpace; }
        set { inputSpace = value; }
    }
    
    void Update()
    {
        input[0] = (Input.GetAxis("Horizontal")!=0)? Input.GetAxis("Horizontal") : (Input.GetButton("Horizontal") == false) ? 0 : 1;
        input[1] = (Input.GetAxis("Vertical")  !=0)? Input.GetAxis("Vertical") : (Input.GetButton("Vertical") == false) ? 0 : 1;
        player_move_input = input;
        player_jump_input = Input.GetButton("Jump");
    }
}
