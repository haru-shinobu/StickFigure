using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    //private void OnTriggerStay(Collider other)
    {

        Debug.Log("WALL");
    }
}
