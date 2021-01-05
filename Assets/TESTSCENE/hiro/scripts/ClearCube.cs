using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearCube : MonoBehaviour
{
    CameraManager camera;
    public GameObject fadeObject;
    public SpriteRenderer goalText;
    public ParticleSystem Particle_kami;
    public ParticleSystem Particle_kura1;
    public ParticleSystem Particle_kura2;
    public GameObject star;
    SoundManager SoundObj;
    //[SerializeField]
    //float fFadeSpeed;
    SceneFadeManager fadeI;
    Transform PlayerObj;
    //public void Awake()
    //{
    //    GameObject.Find("goalText").SetActive(true);
    //}
    public bool nDCount_CountEnd = false;
    public void Start()
    {
        if (fadeObject)
            fadeI = fadeObject.GetComponent<SceneFadeManager>();
        goalText.enabled = false;
        Particle_kami.Stop();
        Particle_kura1.Stop();
        Particle_kura2.Stop();
        camera = GetComponent<CameraManager>();
        GameObject soundtarget = GameObject.Find("SoundObj");
        if (soundtarget)
            SoundObj = soundtarget.GetComponent<SoundManager>();
        PlayerObj = GameObject.FindWithTag("Player").transform;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerBase")
        {
            nDCount_CountEnd = true;
            other.transform.GetChild(0).SendMessage("SceneEndBridgeFall");
            StartCoroutine(clear());
        }
    }

    IEnumerator clear()
    {

        Mesh star = this.gameObject.GetComponent<Mesh>();
        //camera.transform.position= new goalText.transform();
        goalText.gameObject.SetActive(true);
        //transform.position = Particle.transform.position + offset;
        Particle_kami.gameObject.SetActive(true);
        Particle_kura1.gameObject.SetActive(true);
        Particle_kura2.gameObject.SetActive(true);
        //camera =Particle;
        Particle_kami.Play();
        Particle_kura1.Play();
        Particle_kura2.Play();
        if (SoundObj)
        {
            SoundObj.PoperSE();
            SoundObj.PoperSE();
        }
        StartCoroutine("ClearBoxPlayerMove");
        yield return new WaitForSeconds(2f);
        StopCoroutine("ClearBoxPlayerMove");
        SceneManager.LoadScene("Clear");
        yield return new WaitForSeconds(4f);
    }
    IEnumerator ClearBoxPlayerMove()
    {
        Debug.Log("PMOVE");
        PlayerObj.GetComponent<Box_PlayerController>().InClearBox(transform.position);
        var ppos = PlayerObj.parent.position;
        var targetpos = transform.GetChild(0).GetChild(1).transform.position;//GoalFlagger
        float timer = 0;
        while (true)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime / 2;
            PlayerObj.parent.position = PlayerObj.position = Vector3.Lerp(ppos, targetpos, timer);
        }
    }
}