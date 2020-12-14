using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearCube : MonoBehaviour
{

    CameraManager camera;
    public GameObject goalText;
    Vector3 Ppos;
    public ParticleSystem Particle_kami;
    public ParticleSystem Particle_kura1;
    public ParticleSystem Particle_kura2;

    //public void Awake()
    //{
    //    GameObject.Find("goalText").SetActive(true);
    //}

    public void Start()
    {
        goalText.gameObject.SetActive(false);
        Particle_kami.Stop();
        Particle_kura1.Stop();
        Particle_kura2.Stop();
        //GameObject.Find("goalText").SetActive(false);
        camera = GetComponent<CameraManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="PlayerBase")
        {
            other.transform.GetChild(0).SendMessage("SceneEndBridgeBreak");
            StartCoroutine(clear());
        }
    }
    //private void OnCollisionStay(Collision collision)
    //{
    //    Debug.Log(collision.gameObject.name);
    //    if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
    //    {
    //        StartCoroutine(clear());
    //    }
    //}

    private IEnumerator clear()
    {
         //camera.transform.position= new goalText.transform();
        //goalText.gameObject.SetActive(true);
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
        yield return new WaitForSeconds(5f);
    }
}