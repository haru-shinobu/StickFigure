using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testcamroll : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        Invoke("Camtest", 3.0f);
    }

    // Update is called once per frame
    void Camtest()
    {
        Camera.main.gameObject.GetComponent<CameraScript>().UpdateTargetWall(gameObject);
    }
}
