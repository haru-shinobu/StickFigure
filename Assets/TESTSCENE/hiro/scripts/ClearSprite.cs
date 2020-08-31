using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearSprite : MonoBehaviour
{
    public Vector3 StartPos;
    public Vector3 hantennPos;
    public float time;
    public ClearCube goal;
    private Vector3 deltaPos;
    private float elapsedTime;
    private bool bStartToEnd = true;

    void Start()
    {
        transform.position = StartPos;
        deltaPos = (hantennPos - StartPos) / time;
    }
    void Update()
    {
        goal.GetComponent<ClearCube>();
        if (goal == true) { 
        transform.position += deltaPos * Time.deltaTime;
        elapsedTime += Time.deltaTime;
            if (elapsedTime > time)
            {
                if (bStartToEnd)
                {
                    deltaPos = (hantennPos - StartPos) / time;

                    transform.position = StartPos;
                }
                bStartToEnd = !bStartToEnd;
                elapsedTime = 0;
            }
        }
    }
}
