using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearScript : MonoBehaviour
{
    public Vector3 StartPos;
    public Vector3 hantennPos;
    public float time;
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
        {
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
            if (Input.GetMouseButtonDown(0))
            {
                SceneManager.LoadScene("");
            }
        }
    }
}
