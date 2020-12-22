using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ClearScript : MonoBehaviour
{
    public Vector3 StartPos;
    public Vector3 hantennPos;
    public float time;
    private Vector3 deltaPos;
    private float elapsedTime;
    private bool bStartToEnd = true;
    GameObject ManageObject;
    SceneFadeManager scenefademanager;
    Image img;
    float alpha = 0;
   void Start()
    {
        transform.position = StartPos;
        deltaPos = (hantennPos - StartPos) / time;
        ManageObject = GameObject.Find("ManageObject");
        scenefademanager = GetComponent<SceneFadeManager>();
        //SceneFadeManager.FadeOut("GameMainScene");
        img = transform.GetComponent<Image>();
        alpha = img.color.a;
    }
    void Update()
    {
        {

            transform.position += deltaPos * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            
            if (!bStartToEnd)
            {
                if (time * 0.9f > elapsedTime)
                {
                    var imgAlpha = img.color;
                    imgAlpha.a *= 0.9f;
                    img.color = imgAlpha;
                }
            }
            else
            {
                var imgAlpha = img.color;
                imgAlpha.a = 1;
                img.color = imgAlpha;
            }
            if (elapsedTime >= time)
            {
                elapsedTime = 0;
                bStartToEnd = !bStartToEnd;
                if (bStartToEnd)
                    transform.position = StartPos;
            }



            if (Input.GetMouseButtonDown(0))
            {
                //SceneFadeManager.FadeIn();
                SceneManager.LoadScene("Title");
            }
        }
    }
}
