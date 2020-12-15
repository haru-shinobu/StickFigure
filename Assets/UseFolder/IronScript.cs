using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronScript : MonoBehaviour
{
    Vector3 startPos = Vector3.zero, endPos = Vector3.zero;
    IEnumerator corutine = null;
    
    //受け渡し部分
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
    public float Speed;
    public bool MoveOK = false;
    public GameObject arrow;
    enum State
    {
        stay,
        push,
        wait,
        finish,
    }
    State state = State.push;

    // Update is called once per frame
    void Update()
    {
        if (MoveOK)
        {
            Debug.Log(state);
            switch (state)
            {
                case State.stay:
                    break;
                case State.push:
                    corutine = PushMove();
                    StartCoroutine(corutine);
                    state = State.stay;
                    break;
                case State.wait:
                    if (corutine == PushMove())
                        StopCoroutine(corutine);
                    corutine = Steam();
                    StartCoroutine(corutine);
                    state = State.stay;
                    break;
                case State.finish:
                    if (corutine == Steam())
                        StopCoroutine(corutine);
                    corutine = FinishMove();
                    StartCoroutine(corutine);
                    state = State.stay;
                    break;
            }
        }
    }
    //=======================================
    //アニメーション部分
    //=======================================
    IEnumerator PushMove()
    {
        float timer = 0;
        while (timer <= 1)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime * Speed;
            transform.position = Vector3.Lerp(startPos, endPos, timer);
        }
        transform.position = endPos;
        state = State.wait;
    }
    IEnumerator Steam()
    {
        var part = transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>().emission;
        part.rateOverTime = 1000;
        arrow.SetActive(true);
        float timer = 0;
        while (timer <= 1)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime * 2f * Speed;
        }
        part.rateOverTime = 30;
        state = State.finish;
    }
    IEnumerator FinishMove()
    {
        float timer = 0;
        while (timer <= 1)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime * Speed;
            transform.position = Vector3.Lerp(endPos, startPos, timer);
            if (timer > 0.25f)
            {
                Camera.main.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
        transform.position = startPos;
        state = State.stay;
        Destroy(gameObject, 1);
    }


}
