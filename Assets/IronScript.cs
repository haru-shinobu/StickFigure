using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronScript : MonoBehaviour
{
    Vector3 startPos,endPos;
    IEnumerator corutine = null;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        endPos = Vector3.zero;

    }
    public Vector3 GSStartPos
    {
        set { startPos = value; }
        get { return startPos; }
    }
    public Vector3 GSEndPos
    {
        set { endPos = value; }
        get { return endPos; }
    }

    // Update is called once per frame
    void Update()
    {
        if (corutine == null)
        {
            corutine = PushMove();
            StartCoroutine(corutine);
        }
        
        if (corutine!=null)
        {
            corutine = PushMove();
        }
        
        {
            corutine = FinishMove();
        }

        
        

    }
    IEnumerator PushMove()
    {
        float timer = 0;
        while (timer > 1)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
    }
    IEnumerator FinishMove()
    {
        float timer = 0;
        while (timer > 1)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
    }
}
