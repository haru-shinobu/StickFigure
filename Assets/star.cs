using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class star : MonoBehaviour
{
    [SerializeField]
    GameObject[] Effect;
    // Start is called before the first frame update
    void Start()
    {
        for(int i  =0;i< Effect.Length; i++)
        {
            Effect[i].SetActive(false);
        }
        StartCoroutine(Active());
    }
    IEnumerator Active()
    {
        float limit = 10;
        float timer = 0;
        yield return new WaitForEndOfFrame();
        Effect[0].SetActive(true);
        while (timer < limit) 
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        timer = 0;
        Effect[0].SetActive(false);
        Effect[1].SetActive(true);
        while (timer < limit)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        timer = 0;
        Effect[1].SetActive(false);
        Effect[2].SetActive(true);
        while (timer < limit)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
    }
    
}
