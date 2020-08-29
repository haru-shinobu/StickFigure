using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundScript : MonoBehaviour
{
    public void SlipDown()
    {
        GetComponent<CapsuleCollider>().enabled = false;
        Invoke("Slipout", 1);
    }
    void Slipout()
    {
        GetComponent<CapsuleCollider>().enabled = true;
    }
}
