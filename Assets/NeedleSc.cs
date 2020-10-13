using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedleSc : MonoBehaviour
{
    void Start()
    {
        transform.position += transform.up * 8;
        StartCoroutine("Sting");
        Destroy(gameObject, 1.3f);
    }

    IEnumerator Sting()
    {
        Vector3 pos = transform.position;
        Vector3 stingpos = transform.position - transform.up * 8;

        float timer = 0;
        while (timer <= 1)
        {
            timer += Time.deltaTime * 6;
            Debug.Log(timer);
            yield return new WaitForEndOfFrame();
            transform.position = Vector3.Lerp(pos, stingpos, timer);
        }
    }
}
