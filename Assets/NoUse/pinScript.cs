using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pinScript : MonoBehaviour
{
    private SoundManager SoundObj;
    void Start()
    {
        GameObject soundtarget = GameObject.Find("SoundObj");
        if (soundtarget)
            SoundObj = soundtarget.GetComponent<SoundManager>();
        transform.localPosition -= transform.forward * 12;
        StartCoroutine("Sting");
        Invoke("pinSE", 1);
        Destroy(gameObject, 1.35f);
    }

    IEnumerator Sting()
    {
        Vector3 pos = transform.localPosition;
        Vector3 stingpos = transform.localPosition + transform.forward * 10;

        float timer = 0;
        while (timer <= 1)
        {
            timer += Time.deltaTime * 6;
            yield return new WaitForEndOfFrame();
            transform.localPosition = Vector3.Lerp(pos, stingpos, timer);
        }
        timer /= 6;
        while (timer <= 1.2f)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        timer--;
        pos += transform.forward * 6;
        while (timer <= 1)
        {
            timer += Time.deltaTime * 6;
            yield return new WaitForEndOfFrame();
            transform.localPosition = Vector3.Lerp(stingpos, pos, timer);
        }
    }
    void pinSE()
    {
        if (SoundObj)
            SoundObj.NeedleOutSE();
    }
}
