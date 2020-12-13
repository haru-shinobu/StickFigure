﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pinScript : MonoBehaviour
{
    void Start()
    {
        transform.localPosition -= transform.forward * 12;
        StartCoroutine("Sting");
        Destroy(gameObject, 1.3f);
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
    }
}