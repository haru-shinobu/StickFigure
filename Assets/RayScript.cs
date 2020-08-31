using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayScript : MonoBehaviour
{
    public RaycastHit Ray(Vector3 trs,Vector3 targetway,int distance,int layer)
    {
        RaycastHit hit;
        Physics.Raycast(new Ray(trs, targetway), out hit, distance, layer);
        return hit;
    }
}
