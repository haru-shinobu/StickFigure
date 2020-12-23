using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearScript : MonoBehaviour
{
    GameObject ManageObject;
    SceneFadeManager scenefademanager;

    void Start()
    {
        ManageObject = GameObject.Find("ManageObject");
        scenefademanager = GetComponent<SceneFadeManager>();
        //SceneFadeManager.FadeOut("GameMainScene");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
        {
            //SceneFadeManager.FadeIn();
            SceneManager.LoadScene("Title");
        }
    }
}
