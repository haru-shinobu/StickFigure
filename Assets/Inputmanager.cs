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
        input[0] = Input.GetAxis("Horizontal");
        input[1] = Input.GetAxis("Vertical");
        player_move_input = input;
        player_jump_input = Input.GetButton("Jump");
    }
}
