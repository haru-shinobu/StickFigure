using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearCube : MonoBehaviour
{
    public GameObject player;
    CameraManager camera;
    public GameObject fadeObject;
    public SpriteRenderer goalText;
    Vector3 Ppos;
    public ParticleSystem Particle_kami;
    public ParticleSystem Particle_kura1;
    public ParticleSystem Particle_kura2;
    public GameObject star;
    //[SerializeField]
    //float fFadeSpeed;
    SceneFadeManager fadeI;
    //public void Awake()
    //{
    //    GameObject.Find("goalText").SetActive(true);
    //}

    public void Start()
    {
        if (fadeObject)
            fadeI = fadeObject.GetComponent<SceneFadeManager>();
        goalText.enabled = false;
        Particle_kami.Stop();
        Particle_kura1.Stop();
        Particle_kura2.Stop();
        camera = GetComponent<CameraManager>();

    }

    void OnTriggerEnter(Collider other)
    {
    
        if (other.gameObject.tag == "PlayerBase")
        {
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
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Clear");
        yield return new WaitForSeconds(4f);
    }
   }